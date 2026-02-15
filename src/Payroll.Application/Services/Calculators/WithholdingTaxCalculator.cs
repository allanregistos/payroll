using Microsoft.EntityFrameworkCore;
using Payroll.Core.Entities;

namespace Payroll.Application.Services.Calculators;

/// <summary>
/// Withholding tax calculator following TRAIN Law (Tax Reform for Acceleration and Inclusion)
/// Calculates tax based on taxable income using progressive tax brackets
/// </summary>
public class WithholdingTaxCalculator : IWithholdingTaxCalculator
{
    private readonly DbContext _dbContext;

    public WithholdingTaxCalculator(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Calculate withholding tax based on taxable income
    /// Uses TRAIN Law tax brackets and rates
    /// </summary>
    /// <param name="taxableIncome">Taxable income (gross - non-taxable - government contributions)</param>
    /// <param name="effectiveDate">Date for which to calculate (to get correct tax table)</param>
    /// <returns>Withholding tax calculation result</returns>
    public async Task<WithholdingTaxResult> CalculateAsync(decimal taxableIncome, DateTime effectiveDate)
    {
        // Get all active tax brackets for the effective date
        var taxBrackets = await _dbContext.Set<TaxTable>()
            .Where(t => t.IsActive &&
                       t.EffectiveDate <= effectiveDate &&
                       (t.EndDate == null || t.EndDate >= effectiveDate))
            .OrderBy(t => t.MinCompensation)
            .ToListAsync();

        if (!taxBrackets.Any())
        {
            throw new InvalidOperationException(
                $"No tax table found for effective date {effectiveDate:yyyy-MM-dd}. " +
                "Please ensure the tax tables are properly loaded in the database.");
        }

        // Find the appropriate tax bracket
        var applicableBracket = taxBrackets
            .Where(t => taxableIncome >= t.MinCompensation && 
                       (t.MaxCompensation == null || taxableIncome <= t.MaxCompensation))
            .FirstOrDefault();

        if (applicableBracket == null)
        {
            // Income exceeds highest bracket - use the highest bracket
            applicableBracket = taxBrackets
                .OrderByDescending(t => t.MinCompensation)
                .First();
        }

        // Calculate tax: Tax = BaseTax + (TaxableIncome - ExcessOver) × TaxRate
        decimal withholdingTax = 0;

        if (taxableIncome > applicableBracket.ExcessOver)
        {
            var excessIncome = taxableIncome - applicableBracket.ExcessOver;
            withholdingTax = applicableBracket.BaseTax + (excessIncome * applicableBracket.TaxRate);
        }
        else
        {
            withholdingTax = applicableBracket.BaseTax;
        }

        // Round to 2 decimal places
        withholdingTax = Math.Round(withholdingTax, 2, MidpointRounding.AwayFromZero);

        return new WithholdingTaxResult
        {
            TaxableIncome = taxableIncome,
            WithholdingTax = withholdingTax,
            TaxBracket = $"{applicableBracket.MinCompensation:N2} - {applicableBracket.MaxCompensation:N2}",
            TaxRate = applicableBracket.TaxRate,
            BaseTax = applicableBracket.BaseTax,
            ExcessOverMinimum = taxableIncome > applicableBracket.ExcessOver 
                ? taxableIncome - applicableBracket.ExcessOver 
                : 0
        };
    }

    /// <summary>
    /// Calculate monthly withholding tax from annual taxable income
    /// </summary>
    /// <param name="annualTaxableIncome">Annual taxable income</param>
    /// <param name="effectiveDate">Date for which to calculate</param>
    /// <returns>Monthly withholding tax breakdown</returns>
    public async Task<WithholdingTaxResult> CalculateMonthlyAsync(decimal annualTaxableIncome, DateTime effectiveDate)
    {
        var annualTaxResult = await CalculateAsync(annualTaxableIncome, effectiveDate);
        
        var monthlyTax = Math.Round(annualTaxResult.WithholdingTax / 12, 2, MidpointRounding.AwayFromZero);

        return new WithholdingTaxResult
        {
            TaxableIncome = Math.Round(annualTaxableIncome / 12, 2, MidpointRounding.AwayFromZero),
            WithholdingTax = monthlyTax,
            TaxBracket = annualTaxResult.TaxBracket,
            TaxRate = annualTaxResult.TaxRate,
            BaseTax = annualTaxResult.BaseTax,
            ExcessOverMinimum = annualTaxResult.ExcessOverMinimum
        };
    }
}
