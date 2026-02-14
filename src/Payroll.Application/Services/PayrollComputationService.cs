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

        // Get overtime, holiday pay, and other earnings from attendance/payroll
        // For now, we'll use zero, but this should be calculated from attendance records
        var overtimePay = 0m;
        var holidayPay = 0m;
        var otherEarnings = 0m;

        // Calculate gross pay
        var grossPay = basicSalary + allowances + overtimePay + holidayPay + otherEarnings;

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

        // Get other deductions (loans, etc.)
        // For now, using zero, but this should come from loan deductions table
        var loanDeductions = 0m;
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
