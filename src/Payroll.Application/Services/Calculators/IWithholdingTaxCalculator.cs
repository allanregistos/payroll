namespace Payroll.Application.Services.Calculators;

/// <summary>
/// Result of withholding tax calculation
/// </summary>
public class WithholdingTaxResult
{
    public decimal TaxableIncome { get; set; }
    public decimal WithholdingTax { get; set; }
    public string TaxBracket { get; set; } = string.Empty;
    public decimal TaxRate { get; set; }
    public decimal BaseTax { get; set; }
    public decimal ExcessOverMinimum { get; set; }
}

/// <summary>
/// Calculates withholding tax based on TRAIN Law (RA 10963)
/// </summary>
public interface IWithholdingTaxCalculator
{
    Task<WithholdingTaxResult> CalculateAsync(decimal taxableIncome, DateTime effectiveDate);
    Task<WithholdingTaxResult> CalculateMonthlyAsync(decimal annualTaxableIncome, DateTime effectiveDate);
}
