using Microsoft.EntityFrameworkCore;
using Payroll.Application.Services.Calculators;
using Payroll.Core.Entities;

namespace Payroll.Application.Services;

/// <summary>
/// Service for computing complete payroll including earnings, deductions, and net pay
/// Orchestrates all government contribution calculators
/// </summary>
public class PayrollComputationService : IPayrollComputationService
{
    private readonly DbContext _dbContext;
    private readonly ISssContributionCalculator _sssCalculator;
    private readonly IPhilhealthContributionCalculator _philhealthCalculator;
    private readonly IPagibigContributionCalculator _pagibigCalculator;
    private readonly IWithholdingTaxCalculator _taxCalculator;

    public PayrollComputationService(
        DbContext dbContext,
        ISssContributionCalculator sssCalculator,
        IPhilhealthContributionCalculator philhealthCalculator,
        IPagibigContributionCalculator pagibigCalculator,
        IWithholdingTaxCalculator taxCalculator)
    {
        _dbContext = dbContext;
        _sssCalculator = sssCalculator;
        _philhealthCalculator = philhealthCalculator;
        _pagibigCalculator = pagibigCalculator;
        _taxCalculator = taxCalculator;
    }

    /// <summary>
    /// Compute payroll for an employee for a specific period
    /// </summary>
    public async Task<PayrollComputationResult> ComputePayrollAsync(Guid employeeId, int payrollPeriodId, DateTime effectiveDate)
    {
        // Get employee details with compensation
        var employee = await _dbContext.Set<Employee>()
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

        if (employee == null)
        {
            throw new InvalidOperationException($"Employee with ID {employeeId} not found or inactive.");
        }

        // Get payroll period
        var payrollPeriod = await _dbContext.Set<PayrollPeriod>()
            .FirstOrDefaultAsync(p => p.PayrollPeriodId == payrollPeriodId);

        if (payrollPeriod == null)
        {
            throw new InvalidOperationException($"Payroll period with ID {payrollPeriodId} not found.");
        }

        // Get employee compensation
        var compensation = await _dbContext.Set<EmployeeCompensation>()
            .Where(c => c.EmployeeId == employeeId && c.IsActive)
            .OrderByDescending(c => c.EffectiveDate)
            .FirstOrDefaultAsync();

        var basicSalary = compensation?.BasicSalary ?? 0;

        // Get allowances for the employee
        var allowances = await _dbContext.Set<EmployeeAllowance>()
            .Where(a => a.EmployeeId == employeeId && a.IsActive)
            .SumAsync(a => a.Amount);

        var dailyRate = compensation?.DailyRate ?? (basicSalary / 22);
        var hourlyRate = compensation?.HourlyRate ?? (basicSalary / 22 / 8);

        // Get attendance records for this pay period
        var periodStart = payrollPeriod.StartDate.ToDateTime(TimeOnly.MinValue);
        var periodEnd = payrollPeriod.EndDate.ToDateTime(TimeOnly.MaxValue);

        var attendanceRecords = await _dbContext.Set<Attendance>()
            .Where(a => a.EmployeeId == employeeId
                && a.AttendanceDate >= periodStart
                && a.AttendanceDate <= periodEnd)
            .ToListAsync();

        // Calculate attendance-based earnings
        var daysWorked = attendanceRecords.Count(a => !a.IsAbsent);
        var daysAbsent = attendanceRecords.Count(a => a.IsAbsent);
        var totalRegularHours = attendanceRecords.Sum(a => a.RegularHours);
        var totalOvertimeHours = attendanceRecords.Sum(a => a.OvertimeHours);
        var totalLateMinutes = attendanceRecords.Sum(a => a.LateMinutes);
        var totalUndertimeMinutes = attendanceRecords.Sum(a => a.UndertimeMinutes);

        // Overtime pay: hourly rate × 1.25 × overtime hours (Philippine labor code standard OT rate)
        var overtimePay = hourlyRate * 1.25m * totalOvertimeHours;

        // Holiday pay: employees who worked on holidays get 100% premium
        var holidayAttendance = attendanceRecords.Where(a => a.IsHoliday && !a.IsAbsent).ToList();
        var holidayPay = holidayAttendance.Sum(a => dailyRate); // 100% premium for holiday work

        // Rest day pay: employees who worked on rest days get 30% premium
        var restDayAttendance = attendanceRecords.Where(a => a.IsRestDay && !a.IsAbsent && !a.IsHoliday).ToList();
        var restDayPay = restDayAttendance.Sum(a => dailyRate * 0.30m);

        // Late/undertime deductions (deduct per minute from hourly rate)
        var perMinuteRate = hourlyRate / 60m;
        var lateDeduction = perMinuteRate * totalLateMinutes;
        var undertimeDeduction = perMinuteRate * totalUndertimeMinutes;
        var otherEarnings = restDayPay;

        // Basic pay: if attendance exists, compute proportionally; otherwise use full salary
        decimal basicPay;
        if (attendanceRecords.Any())
        {
            basicPay = dailyRate * daysWorked;
        }
        else
        {
            // No attendance records - use full basic salary (common for salaried employees)
            basicPay = basicSalary;
        }

        // Calculate gross pay
        var grossPay = basicPay + allowances + overtimePay + holidayPay + otherEarnings - lateDeduction - undertimeDeduction;

        // Calculate government contributions
        var sssResult = await _sssCalculator.CalculateAsync(basicSalary, effectiveDate);
        var philhealthResult = await _philhealthCalculator.CalculateAsync(basicSalary, effectiveDate);
        var pagibigResult = await _pagibigCalculator.CalculateAsync(basicSalary, effectiveDate);

        // Calculate taxable income
        // Taxable income = Gross - Non-taxable allowances - SSS - PhilHealth - Pag-IBIG
        var taxableIncome = grossPay - sssResult.EmployeeContribution - philhealthResult.EmployeeShare - pagibigResult.EmployeeContribution;

        // For annual tax calculation, multiply by 12 and use the monthly method
        var annualTaxableIncome = taxableIncome * 12;
        var taxResult = await _taxCalculator.CalculateMonthlyAsync(annualTaxableIncome, effectiveDate);

        // Total government deductions
        var totalGovernmentDeductions = sssResult.EmployeeContribution +
                                       philhealthResult.EmployeeShare +
                                       pagibigResult.EmployeeContribution +
                                       taxResult.WithholdingTax;

        // Get active loan deductions for this employee
        var periodEndDate = payrollPeriod.EndDate;
        var loanDeductions = await _dbContext.Set<EmployeeLoan>()
            .Where(l => l.EmployeeId == employeeId
                && l.Status == "Active"
                && l.FirstDeductionDate <= periodEndDate)
            .SumAsync(l => l.MonthlyAmortization);

        var otherDeductions = 0m;
        var totalOtherDeductions = loanDeductions + otherDeductions;

        // Calculate net pay
        var totalDeductions = totalGovernmentDeductions + totalOtherDeductions;
        var netPay = grossPay - totalDeductions;

        // Employer contributions
        var totalEmployerContributions = sssResult.EmployerContribution +
                                        sssResult.EcContribution +
                                        philhealthResult.EmployerShare +
                                        pagibigResult.EmployerContribution;

        return new PayrollComputationResult
        {
            EmployeeId = employee.EmployeeId,
            EmployeeCode = employee.EmployeeCode,
            EmployeeName = $"{employee.FirstName} {employee.LastName}",
            PayrollPeriodId = payrollPeriodId,

            // Earnings
            BasicSalary = basicSalary,
            Allowances = allowances,
            OvertimePay = overtimePay,
            HolidayPay = holidayPay,
            OtherEarnings = otherEarnings,
            GrossPay = grossPay,

            // Government Deductions
            SssEmployeeContribution = sssResult.EmployeeContribution,
            PhilhealthEmployeeContribution = philhealthResult.EmployeeShare,
            PagibigEmployeeContribution = pagibigResult.EmployeeContribution,
            WithholdingTax = taxResult.WithholdingTax,
            TotalGovernmentDeductions = totalGovernmentDeductions,

            // Other Deductions
            LoanDeductions = loanDeductions,
            OtherDeductions = otherDeductions,
            TotalOtherDeductions = totalOtherDeductions,

            // Summary
            TotalDeductions = totalDeductions,
            NetPay = netPay,

            // Employer Share
            SssEmployerContribution = sssResult.EmployerContribution,
            SssEcContribution = sssResult.EcContribution,
            PhilhealthEmployerContribution = philhealthResult.EmployerShare,
            PagibigEmployerContribution = pagibigResult.EmployerContribution,
            TotalEmployerContributions = totalEmployerContributions
        };
    }

    /// <summary>
    /// Compute payroll for multiple employees in a period
    /// </summary>
    public async Task<List<PayrollComputationResult>> ComputePayrollForPeriodAsync(int payrollPeriodId, DateTime effectiveDate)
    {
        // Get all employees (no IsActive property on Employee entity)
        var employees = await _dbContext.Set<Employee>()
            .Select(e => e.EmployeeId)
            .ToListAsync();

        var results = new List<PayrollComputationResult>();

        foreach (var employeeId in employees)
        {
            try
            {
                var result = await ComputePayrollAsync(employeeId, payrollPeriodId, effectiveDate);
                results.Add(result);
            }
            catch (Exception ex)
            {
                // Log error and continue with next employee
                // In production, use proper logging framework
                Console.WriteLine($"Error computing payroll for employee {employeeId}: {ex.Message}");
            }
        }

        return results;
    }
}
