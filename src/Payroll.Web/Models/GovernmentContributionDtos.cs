using System.Text.Json.Serialization;

namespace PayrollWeb.Models;

public class SssTableDto
{
    [JsonPropertyName("sssTableId")]
    public int SssTableId { get; set; }
    [JsonPropertyName("minSalary")]
    public decimal MinSalary { get; set; }
    [JsonPropertyName("maxSalary")]
    public decimal MaxSalary { get; set; }
    [JsonPropertyName("employeeContribution")]
    public decimal EmployeeContribution { get; set; }
    [JsonPropertyName("employerContribution")]
    public decimal EmployerContribution { get; set; }
    [JsonPropertyName("totalContribution")]
    public decimal TotalContribution { get; set; }
    [JsonPropertyName("ecContribution")]
    public decimal EcContribution { get; set; }
    [JsonPropertyName("effectiveDate")]
    public DateTime EffectiveDate { get; set; }
    [JsonPropertyName("endDate")]
    public DateTime? EndDate { get; set; }
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; } = true;
}

public class PhilhealthTableDto
{
    [JsonPropertyName("philhealthTableId")]
    public int PhilhealthTableId { get; set; }
    [JsonPropertyName("minSalary")]
    public decimal MinSalary { get; set; }
    [JsonPropertyName("maxSalary")]
    public decimal? MaxSalary { get; set; }
    [JsonPropertyName("premiumRate")]
    public decimal PremiumRate { get; set; }
    [JsonPropertyName("employeeShare")]
    public decimal EmployeeShare { get; set; }
    [JsonPropertyName("employerShare")]
    public decimal EmployerShare { get; set; }
    [JsonPropertyName("effectiveDate")]
    public DateTime EffectiveDate { get; set; }
    [JsonPropertyName("endDate")]
    public DateTime? EndDate { get; set; }
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; } = true;
}

public class PagibigTableDto
{
    [JsonPropertyName("pagibigTableId")]
    public int PagibigTableId { get; set; }
    [JsonPropertyName("minSalary")]
    public decimal MinSalary { get; set; }
    [JsonPropertyName("maxSalary")]
    public decimal? MaxSalary { get; set; }
    [JsonPropertyName("employeeRate")]
    public decimal EmployeeRate { get; set; }
    [JsonPropertyName("employerRate")]
    public decimal EmployerRate { get; set; }
    [JsonPropertyName("maxEmployeeContribution")]
    public decimal? MaxEmployeeContribution { get; set; }
    [JsonPropertyName("maxEmployerContribution")]
    public decimal? MaxEmployerContribution { get; set; }
    [JsonPropertyName("effectiveDate")]
    public DateTime EffectiveDate { get; set; }
    [JsonPropertyName("endDate")]
    public DateTime? EndDate { get; set; }
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; } = true;
}

public class TaxTableDto
{
    [JsonPropertyName("taxTableId")]
    public int TaxTableId { get; set; }
    [JsonPropertyName("minCompensation")]
    public decimal MinCompensation { get; set; }
    [JsonPropertyName("maxCompensation")]
    public decimal? MaxCompensation { get; set; }
    [JsonPropertyName("baseTax")]
    public decimal BaseTax { get; set; }
    [JsonPropertyName("taxRate")]
    public decimal TaxRate { get; set; }
    [JsonPropertyName("excessOver")]
    public decimal ExcessOver { get; set; }
    [JsonPropertyName("periodType")]
    public string PeriodType { get; set; } = "Monthly";
    [JsonPropertyName("effectiveDate")]
    public DateTime EffectiveDate { get; set; }
    [JsonPropertyName("endDate")]
    public DateTime? EndDate { get; set; }
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; } = true;
}
