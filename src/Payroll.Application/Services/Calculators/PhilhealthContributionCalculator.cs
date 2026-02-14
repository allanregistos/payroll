using Microsoft.EntityFrameworkCore;
using Payroll.Core.Entities;

namespace Payroll.Application.Services.Calculators;

/// <summary>
/// PhilHealth premium calculator
/// Calculates employee and employer shares based on monthly basic salary
/// </summary>
public class PhilhealthContributionCalculator : IPhilhealthContributionCalculator
{
    private readonly DbContext _dbContext;

    public PhilhealthContributionCalculator(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Calculate PhilHealth premium based on monthly basic salary
    /// Current implementation: 5% of monthly basic salary (max ceiling applies)
    /// Split 50/50 between employee and employer
    /// </summary>
    /// <param name="monthlyBasicSalary">Monthly basic salary of the employee</param>
    /// <param name="effectiveDate">Date for which to calculate (to get correct rate)</param>
    /// <returns>PhilHealth premium breakdown</returns>
    public async Task<PhilhealthContributionResult> CalculateAsync(decimal monthlyBasicSalary, DateTime effectiveDate)
    {
        // Get the appropriate PhilHealth contribution table entry
        var philhealthTable = await _dbContext.Set<PhilhealthContributionTable>()
            .Where(t => t.IsActive &&
                       t.EffectiveDate <= effectiveDate &&
                       (t.EndDate == null || t.EndDate >= effectiveDate) &&
                       monthlyBasicSalary >= t.MinSalary &&
                       (t.MaxSalary == null || monthlyBasicSalary <= t.MaxSalary))
            .OrderByDescending(t => t.EffectiveDate)
            .FirstOrDefaultAsync();

        if (philhealthTable == null)
        {
            throw new InvalidOperationException(
                $"No PhilHealth contribution table found for salary {monthlyBasicSalary:N2} on {effectiveDate:yyyy-MM-dd}. " +
                "Please ensure the PhilHealth contribution tables are properly loaded in the database.");
        }

        // Apply salary ceiling if specified
        var salaryForComputation = monthlyBasicSalary;
        if (philhealthTable.MaxSalary.HasValue && monthlyBasicSalary > philhealthTable.MaxSalary.Value)
        {
            salaryForComputation = philhealthTable.MaxSalary.Value;
        }

        // Calculate total premium
        var totalPremium = salaryForComputation * philhealthTable.PremiumRate;

        // Split between employee and employer
        var employeeShare = totalPremium * philhealthTable.EmployeeShare;
        var employerShare = totalPremium * philhealthTable.EmployerShare;

        // Round to 2 decimal places
        employeeShare = Math.Round(employeeShare, 2, MidpointRounding.AwayFromZero);
        employerShare = Math.Round(employerShare, 2, MidpointRounding.AwayFromZero);
        totalPremium = Math.Round(totalPremium, 2, MidpointRounding.AwayFromZero);

        return new PhilhealthContributionResult
        {
            EmployeeShare = employeeShare,
            EmployerShare = employerShare,
            TotalPremium = totalPremium,
            BasicSalary = salaryForComputation,
            PremiumRate = philhealthTable.PremiumRate
        };
    }
}
