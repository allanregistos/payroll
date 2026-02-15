namespace PayrollWeb.Models;

public class AttendanceDto
{
    public int AttendanceId { get; set; }
    public Guid EmployeeId { get; set; }
    public string? EmployeeCode { get; set; }
    public string? EmployeeName { get; set; }
    public DateTime AttendanceDate { get; set; }
    public DateTime? TimeIn { get; set; }
    public DateTime? TimeOut { get; set; }
    public decimal RegularHours { get; set; }
    public decimal OvertimeHours { get; set; }
    public int LateMinutes { get; set; }
    public int UndertimeMinutes { get; set; }
    public bool IsHoliday { get; set; }
    public string? Remarks { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AttendanceCreateDto
{
    public Guid EmployeeId { get; set; }
    public DateOnly AttendanceDate { get; set; }
    public TimeOnly? TimeIn { get; set; }
    public TimeOnly? TimeOut { get; set; }
    public decimal RegularHours { get; set; }
    public decimal OvertimeHours { get; set; }
    public int LateMinutes { get; set; }
    public int UndertimeMinutes { get; set; }
    public string? Remarks { get; set; }
}

public class AttendanceUpdateDto
{
    public TimeOnly? TimeIn { get; set; }
    public TimeOnly? TimeOut { get; set; }
    public decimal RegularHours { get; set; }
    public decimal OvertimeHours { get; set; }
    public int LateMinutes { get; set; }
    public int UndertimeMinutes { get; set; }
    public string? Remarks { get; set; }
}
