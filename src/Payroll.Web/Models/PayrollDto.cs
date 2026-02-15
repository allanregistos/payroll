using System.Text.Json.Serialization;

namespace PayrollWeb.Models;

public class ApiError
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

public class PayrollDto
{
    public Guid PayrollId { get; set; }
    public Guid EmployeeId { get; set; }
    public DateOnly PayPeriodStart { get; set; }
    public DateOnly PayPeriodEnd { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal SssContribution { get; set; }
    public decimal PhilhealthContribution { get; set; }
    public decimal PagibigContribution { get; set; }
    public decimal WithholdingTax { get; set; }
    public decimal OvertimePay { get; set; }
    public decimal GrossPay { get; set; }
    public decimal NetPay { get; set; }
    public string PayrollStatus { get; set; } = string.Empty;
    
    // Navigation properties
    public string? EmployeeName { get; set; }
    public string? EmployeeNumber { get; set; }
}

public class PayrollComputeRequest
{
    public Guid EmployeeId { get; set; }
    public DateOnly PayPeriodStart { get; set; }
    public DateOnly PayPeriodEnd { get; set; }
}
