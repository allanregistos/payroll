namespace PayrollWeb.Models;

public class LeaveDto
{
    public Guid LeaveId { get; set; }
    public Guid EmployeeId { get; set; }
    public string LeaveType { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal TotalDays { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string? Remarks { get; set; }
    public DateTime RequestedDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public Guid? ApprovedBy { get; set; }
    
    // Navigation properties
    public string? EmployeeName { get; set; }
    public string? EmployeeNumber { get; set; }
    public string? ApproverName { get; set; }
}

public class LeaveCreateDto
{
    public Guid EmployeeId { get; set; }
    public string LeaveType { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal TotalDays { get; set; }
    public string? Reason { get; set; }
}

public class LeaveUpdateDto
{
    public string Status { get; set; } = string.Empty;
    public string? Remarks { get; set; }
}
