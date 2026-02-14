namespace Payroll.Core.Entities;

public class SssContributionTable
{
    public int SssTableId { get; set; }
    public decimal MinSalary { get; set; }
    public decimal MaxSalary { get; set; }
    public decimal EmployeeContribution { get; set; }
    public decimal EmployerContribution { get; set; }
    public decimal TotalContribution { get; set; }
    public decimal EcContribution { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
}

public class PhilhealthContributionTable
{
    public int PhilhealthTableId { get; set; }
    public decimal MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public decimal PremiumRate { get; set; }
    public decimal EmployeeShare { get; set; }
    public decimal EmployerShare { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
}

public class PagibigContributionTable
{
    public int PagibigTableId { get; set; }
    public decimal MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public decimal EmployeeRate { get; set; }
    public decimal EmployerRate { get; set; }
    public decimal? MaxEmployeeContribution { get; set; }
    public decimal? MaxEmployerContribution { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
}

public class TaxTable
{
    public int TaxTableId { get; set; }
    public decimal MinIncome { get; set; }
    public decimal? MaxIncome { get; set; }
    public decimal BaseTax { get; set; }
    public decimal TaxRate { get; set; }
    public string? BracketName { get; set; }
    
    // Tax period
    public string PeriodType { get; set; } = "Annual";
    
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
}

public class Holiday
{
    public int HolidayId { get; set; }
    public string HolidayName { get; set; } = string.Empty;
    public DateTime HolidayDate { get; set; }
    public string HolidayType { get; set; } = "Regular";
    
    // Premium rates
    public decimal RegularDayPremium { get; set; } = 2.0m;
    public decimal RestDayPremium { get; set; } = 2.6m;
    
    public int Year { get; set; }
    public bool IsRecurring { get; set; }
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
