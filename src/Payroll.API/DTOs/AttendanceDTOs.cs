namespace Payroll.API.DTOs;

/// <summary>
/// Data Transfer Objects for Attendance API
/// </summary>
/// 
public class CreateAttendanceRequest
{
    public Guid EmployeeId { get; set; }
    public DateTime AttendanceDate { get; set; }
    public DateTime? TimeIn { get; set; }
    public DateTime? TimeOut { get; set; }
    public decimal RegularHours { get; set; }
    public decimal OvertimeHours { get; set; }
    public int LateMinutes { get; set; }
    public int UndertimeMinutes { get; set; }
    public string? Remarks { get; set; }
}

public class UpdateAttendanceRequest
{
    public DateTime? TimeIn { get; set; }
    public DateTime? TimeOut { get; set; }
    public decimal RegularHours { get; set; }
    public decimal OvertimeHours { get; set; }
    public int LateMinutes { get; set; }
    public int UndertimeMinutes { get; set; }
    public string? Remarks { get; set; }
}

public class AttendanceResponse
{
    public int AttendanceId { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
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

public class CreateLeaveRequest
{
    public Guid EmployeeId { get; set; }
    public int LeaveTypeId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal NumberOfDays { get; set; }
    public string? Reason { get; set; }
}

public class ApproveLeaveRequest
{
    public string Status { get; set; } = "Approved"; // Approved or Rejected
    public string? ApprovalRemarks { get; set; }
}

public class LeaveResponse
{
    public int LeaveId { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public int LeaveTypeId { get; set; }
    public string LeaveTypeName { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal NumberOfDays { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? ApprovalRemarks { get; set; }
    public DateTime CreatedAt { get; set; }
}
