namespace Payroll.Application.Services.Calculators;

/// <summary>
/// Result of SSS contribution calculation
/// </summary>
public class SssContributionResult
{
    public decimal EmployeeContribution { get; set; }
    public decimal EmployerContribution { get; set; }
    public decimal EcContribution { get; set; }
    public decimal TotalContribution { get; set; }
    public decimal MonthlySalaryCredit { get; set; }
}

/// <summary>
/// Calculates SSS (Social Security System) contributions based on monthly salary
/// </summary>
public interface ISssContributionCalculator
{
    Task<SssContributionResult> CalculateAsync(decimal monthlySalary, DateTime effectiveDate);
}
