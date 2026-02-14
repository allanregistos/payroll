namespace Payroll.Core.Entities;

public class SalaryGrade
{
    public int SalaryGradeId { get; set; }
    public string GradeCode { get; set; } = string.Empty;
    public string GradeName { get; set; } = string.Empty;
    public decimal MinSalary { get; set; }
    public decimal MaxSalary { get; set; }
    public bool IsActive { get; set; } = true;
    
    public ICollection<EmployeeCompensation> EmployeeCompensations { get; set; } = new List<EmployeeCompensation>();
}

public class EmployeeCompensation
{
    public int CompensationId { get; set; }
    public Guid EmployeeId { get; set; }
    public int? SalaryGradeId { get; set; }
    
    // Basic Pay
    public decimal BasicSalary { get; set; }
    public decimal? DailyRate { get; set; }
    public decimal? HourlyRate { get; set; }
    
    // Pay Frequency
    public string PayFrequency { get; set; } = "Monthly";
    
    // Other Regular Pay
    public decimal Cola { get; set; }
    
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    
    public Employee Employee { get; set; } = null!;
    public SalaryGrade? SalaryGrade { get; set; }
}

public class Allowance
{
    public int AllowanceId { get; set; }
    public string AllowanceCode { get; set; } = string.Empty;
    public string AllowanceName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsTaxable { get; set; } = true;
    public bool IsActive { get; set; } = true;
    
    public ICollection<EmployeeAllowance> EmployeeAllowances { get; set; } = new List<EmployeeAllowance>();
}

public class EmployeeAllowance
{
    public int EmployeeAllowanceId { get; set; }
    public Guid EmployeeId { get; set; }
    public int AllowanceId { get; set; }
    
    public decimal Amount { get; set; }
    public string Frequency { get; set; } = "Monthly";
    
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    
    public Employee Employee { get; set; } = null!;
    public Allowance Allowance { get; set; } = null!;
}
