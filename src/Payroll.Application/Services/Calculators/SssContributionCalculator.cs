using Microsoft.EntityFrameworkCore;
using Payroll.Core.Entities;

namespace Payroll.Application.Services.Calculators;

/// <summary>
/// SSS (Social Security System) contribution calculator
/// Calculates employee and employer contributions based on monthly salary credit
/// </summary>
public class SssContributionCalculator : ISssContributionCalculator
{
    private readonly DbContext _dbContext;

    public SssContributionCalculator(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Calculate SSS contributions based on monthly salary
    /// </summary>
    /// <param name="monthlySalary">Monthly salary of the employee</param>
    /// <param name="effectiveDate">Date for which to calculate (to get correct rate table)</param>
    /// <returns>SSS contribution breakdown</returns>
    public async Task<SssContributionResult> CalculateAsync(decimal monthlySalary, DateTime effectiveDate)
    {
        // Get the appropriate SSS contribution table entry
        var sssTable = await _dbContext.Set<SssContributionTable>()
            .Where(t => t.IsActive &&
                       t.EffectiveDate <= effectiveDate &&
                       (t.EndDate == null || t.EndDate >= effectiveDate) &&
                       monthlySalary >= t.MinSalary &&
                       monthlySalary <= t.MaxSalary)
            .OrderBy(t => t.EffectiveDate)
            .FirstOrDefaultAsync();

        if (sssTable == null)
        {
            throw new InvalidOperationException(
                $"No SSS contribution table found for salary {monthlySalary:N2} on {effectiveDate:yyyy-MM-dd}. " +
                "Please ensure the SSS contribution tables are properly loaded in the database.");
        }

        return new SssContributionResult
        {
            EmployeeContribution = sssTable.EmployeeContribution,
            EmployerContribution = sssTable.EmployerContribution,
            EcContribution = sssTable.EcContribution,
            TotalContribution = sssTable.TotalContribution,
            MonthlySalaryCredit = (sssTable.MinSalary + sssTable.MaxSalary) / 2
        };
    }
}
