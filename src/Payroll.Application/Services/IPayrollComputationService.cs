namespace Payroll.Application.Services;

/// <summary>
/// Service for computing complete payroll including earnings, deductions, and net pay
/// </summary>
public interface IPayrollComputationService
{
    /// <summary>
    /// Compute payroll for an employee for a specific period
    /// </summary>
    /// <param name="employeeId">Employee ID</param>
    /// <param name="payrollPeriodId">Payroll period ID</param>
    /// <param name="effectiveDate">Effective date for government contributions</param>
    /// <returns>Complete payroll computation result</returns>
    Task<PayrollComputationResult> ComputePayrollAsync(Guid employeeId, int payrollPeriodId, DateTime effectiveDate);

    /// <summary>
    /// Compute payroll for multiple employees in a period
    /// </summary>
    /// <param name="payrollPeriodId">Payroll period ID</param>
    /// <param name="effectiveDate">Effective date for government contributions</param>
    /// <returns>List of payroll computation results</returns>
    Task<List<PayrollComputationResult>> ComputePayrollForPeriodAsync(int payrollPeriodId, DateTime effectiveDate);
}

/// <summary>
/// Result of payroll computation
/// </summary>
public class PayrollComputationResult
{
    public Guid EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public int PayrollPeriodId { get; set; }

    // Earnings
    public decimal BasicSalary { get; set; }
    public decimal Allowances { get; set; }
    public decimal OvertimePay { get; set; }
    public decimal HolidayPay { get; set; }
    public decimal OtherEarnings { get; set; }
    public decimal GrossPay { get; set; }

    // Government Deductions
    public decimal SssEmployeeContribution { get; set; }
    public decimal PhilhealthEmployeeContribution { get; set; }
    public decimal PagibigEmployeeContribution { get; set; }
    public decimal WithholdingTax { get; set; }
    public decimal TotalGovernmentDeductions { get; set; }

    // Other Deductions
    public decimal LoanDeductions { get; set; }
    public decimal OtherDeductions { get; set; }
    public decimal TotalOtherDeductions { get; set; }

    // Summary
    public decimal TotalDeductions { get; set; }
    public decimal NetPay { get; set; }

    // Employer Share (for reference)
    public decimal SssEmployerContribution { get; set; }
    public decimal SssEcContribution { get; set; }
    public decimal PhilhealthEmployerContribution { get; set; }
    public decimal PagibigEmployerContribution { get; set; }
    public decimal TotalEmployerContributions { get; set; }
}
