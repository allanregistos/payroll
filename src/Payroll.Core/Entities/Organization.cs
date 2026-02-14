namespace Payroll.Core.Entities;

public class Department
{
    public int DepartmentId { get; set; }
    public string DepartmentCode { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<EmployeeAssignment> EmployeeAssignments { get; set; } = new List<EmployeeAssignment>();
}

public class Position
{
    public int PositionId { get; set; }
    public string PositionCode { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<EmployeeAssignment> EmployeeAssignments { get; set; } = new List<EmployeeAssignment>();
}

public class EmployeeAssignment
{
    public int AssignmentId { get; set; }
    public Guid EmployeeId { get; set; }
    public int DepartmentId { get; set; }
    public int PositionId { get; set; }
    public bool IsPrimary { get; set; } = true;
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Employee Employee { get; set; } = null!;
    public Department Department { get; set; } = null!;
    public Position Position { get; set; } = null!;
}
