namespace PayrollWeb.Models;

public class CompensationDto
{
    public int CompensationId { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public decimal BasicSalary { get; set; }
    public decimal? DailyRate { get; set; }
    public decimal? HourlyRate { get; set; }
    public string PayFrequency { get; set; } = "Monthly";
    public decimal Cola { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCompensationDto
{
    public Guid EmployeeId { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal? DailyRate { get; set; }
    public decimal? HourlyRate { get; set; }
    public string? PayFrequency { get; set; } = "Monthly";
    public decimal Cola { get; set; }
    public DateTime EffectiveDate { get; set; } = DateTime.Today;
}
