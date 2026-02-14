using Microsoft.EntityFrameworkCore;
using Payroll.Core.Entities;

namespace Payroll.Application.Services.Calculators;

/// <summary>
/// Pag-IBIG contribution calculator
/// Calculates employee and employer shares based on monthly compensation
/// </summary>
public class PagibigContributionCalculator : IPagibigContributionCalculator
{
    private readonly DbContext _dbContext;

    public PagibigContributionCalculator(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Calculate Pag-IBIG contribution based on monthly compensation
    /// Rate: 2% of monthly compensation (1% employee, 1% employer)
    /// Maximum contribution: PHP 100.00 per share
    /// </summary>
    /// <param name="monthlyCompensation">Monthly compensation of the employee</param>
    /// <param name="effectiveDate">Date for which to calculate (to get correct rate)</param>
    /// <returns>Pag-IBIG contribution breakdown</returns>
    public async Task<PagibigContributionResult> CalculateAsync(decimal monthlyCompensation, DateTime effectiveDate)
    {
        // Get the appropriate Pag-IBIG contribution table entry
        var pagibigTable = await _dbContext.Set<PagibigContributionTable>()
            .Where(t => t.IsActive &&
                       t.EffectiveDate <= effectiveDate &&
                       (t.EndDate == null || t.EndDate >= effectiveDate) &&
                       monthlyCompensation >= t.MinSalary &&
                       (t.MaxSalary == null || monthlyCompensation <= t.MaxSalary))
            .OrderByDescending(t => t.EffectiveDate)
            .FirstOrDefaultAsync();

        if (pagibigTable == null)
        {
            throw new InvalidOperationException(
                $"No Pag-IBIG contribution table found for compensation {monthlyCompensation:N2} on {effectiveDate:yyyy-MM-dd}. " +
                "Please ensure the Pag-IBIG contribution tables are properly loaded in the database.");
        }

        // Calculate contributions based on rates
        var employeeContribution = monthlyCompensation * pagibigTable.EmployeeRate;
        var employerContribution = monthlyCompensation * pagibigTable.EmployerRate;

        // Apply maximum contribution cap if specified
        if (pagibigTable.MaxEmployeeContribution.HasValue)
        {
            employeeContribution = Math.Min(employeeContribution, pagibigTable.MaxEmployeeContribution.Value);
        }

        if (pagibigTable.MaxEmployerContribution.HasValue)
        {
            employerContribution = Math.Min(employerContribution, pagibigTable.MaxEmployerContribution.Value);
        }

        // Round to 2 decimal places
        employeeContribution = Math.Round(employeeContribution, 2, MidpointRounding.AwayFromZero);
        employerContribution = Math.Round(employerContribution, 2, MidpointRounding.AwayFromZero);

        var totalContribution = employeeContribution + employerContribution;

        return new PagibigContributionResult
        {
            EmployeeContribution = employeeContribution,
            EmployerContribution = employerContribution,
            TotalContribution = totalContribution,
            MonthlySalary = monthlyCompensation,
            EmployeeRate = pagibigTable.EmployeeRate,
            EmployerRate = pagibigTable.EmployerRate
        };
    }
}
