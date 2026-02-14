namespace PayrollWeb.Models;

public class AttendanceDto
{
    public Guid AttendanceId { get; set; }
    public Guid EmployeeId { get; set; }
    public DateOnly AttendanceDate { get; set; }
    public TimeOnly? TimeIn { get; set; }
    public TimeOnly? TimeOut { get; set; }
    public decimal HoursWorked { get; set; }
    public decimal OvertimeHours { get; set; }
    public bool IsLate { get; set; }
    public bool IsAbsent { get; set; }
    public string? Remarks { get; set; }
    
    // Navigation properties
    public string? EmployeeName { get; set; }
    public string? EmployeeNumber { get; set; }
}

public class AttendanceCreateDto
{
    public Guid EmployeeId { get; set; }
    public DateOnly AttendanceDate { get; set; }
    public TimeOnly? TimeIn { get; set; }
    public TimeOnly? TimeOut { get; set; }
    public decimal HoursWorked { get; set; }
    public decimal OvertimeHours { get; set; }
    public bool IsLate { get; set; }
    public bool IsAbsent { get; set; }
    public string? Remarks { get; set; }
}

public class AttendanceUpdateDto
{
    public TimeOnly? TimeIn { get; set; }
    public TimeOnly? TimeOut { get; set; }
    public decimal HoursWorked { get; set; }
    public decimal OvertimeHours { get; set; }
    public bool IsLate { get; set; }
    public bool IsAbsent { get; set; }
    public string? Remarks { get; set; }
}
