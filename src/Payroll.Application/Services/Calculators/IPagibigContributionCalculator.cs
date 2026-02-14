namespace Payroll.Application.Services.Calculators;

/// <summary>
/// Result of Pag-IBIG contribution calculation
/// </summary>
public class PagibigContributionResult
{
    public decimal EmployeeContribution { get; set; }
    public decimal EmployerContribution { get; set; }
    public decimal TotalContribution { get; set; }
    public decimal MonthlySalary { get; set; }
    public decimal EmployeeRate { get; set; }
    public decimal EmployerRate { get; set; }
}

/// <summary>
/// Calculates Pag-IBIG (HDMF) contributions based on monthly salary
/// </summary>
public interface IPagibigContributionCalculator
{
    Task<PagibigContributionResult> CalculateAsync(decimal monthlySalary, DateTime effectiveDate);
}
