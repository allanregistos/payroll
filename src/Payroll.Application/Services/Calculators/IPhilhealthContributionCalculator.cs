namespace Payroll.Application.Services.Calculators;

/// <summary>
/// Result of PhilHealth premium calculation
/// </summary>
public class PhilhealthContributionResult
{
    public decimal EmployeeShare { get; set; }
    public decimal EmployerShare { get; set; }
    public decimal TotalPremium { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal PremiumRate { get; set; }
}

/// <summary>
/// Calculates PhilHealth premium based on monthly basic salary
/// </summary>
public interface IPhilhealthContributionCalculator
{
    Task<PhilhealthContributionResult> CalculateAsync(decimal monthlyBasicSalary, DateTime effectiveDate);
}
