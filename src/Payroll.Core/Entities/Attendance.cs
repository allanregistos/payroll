namespace Payroll.Core.Entities;

public class Attendance
{
    public int AttendanceId { get; set; }
    public Guid EmployeeId { get; set; }
    public DateTime AttendanceDate { get; set; }
    
    // Time In/Out
    public DateTime? TimeIn { get; set; }
    public DateTime? TimeOut { get; set; }
    
    // Break Times
    public DateTime? BreakOut { get; set; }
    public DateTime? BreakIn { get; set; }
    
    // Computed Hours
    public decimal RegularHours { get; set; }
    public decimal OvertimeHours { get; set; }
    public int LateMinutes { get; set; }
    public int UndertimeMinutes { get; set; }
    
    // Status
    public bool IsHoliday { get; set; }
    public bool IsRestDay { get; set; }
    public bool IsAbsent { get; set; }
    
    public string? Remarks { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    public Employee Employee { get; set; } = null!;
}

public class LeaveType
{
    public int LeaveTypeId { get; set; }
    public string LeaveCode { get; set; } = string.Empty;
    public string LeaveTypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPaid { get; set; } = true;
    public bool IsActive { get; set; } = true;
    
    public ICollection<EmployeeLeave> EmployeeLeaves { get; set; } = new List<EmployeeLeave>();
}

public class EmployeeLeave
{
    public int LeaveId { get; set; }
    public Guid EmployeeId { get; set; }
    public int LeaveTypeId { get; set; }
    
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal NumberOfDays { get; set; }
    
    public string? Reason { get; set; }
    public string Status { get; set; } = "Pending";
    public string? ApprovalRemarks { get; set; }
    
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    public Employee Employee { get; set; } = null!;
    public LeaveType LeaveType { get; set; } = null!;
}
