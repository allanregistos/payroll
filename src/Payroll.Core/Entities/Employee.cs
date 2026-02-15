namespace Payroll.Core.Entities;

public class Employee
{
    public Guid EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? Suffix { get; set; }
    
    public DateTime DateOfBirth { get; set; }
    public char? Gender { get; set; }
    public string? CivilStatus { get; set; }
    
    // Contact Information
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? PostalCode { get; set; }
    
    // Employment Details
    public DateTime HireDate { get; set; }
    public DateTime? DateRegularized { get; set; }
    public DateTime? DateSeparated { get; set; }
    public string EmploymentStatus { get; set; } = "Active";
    public string EmployeeType { get; set; } = "Regular";
    
    // Government IDs
    public string? SssNumber { get; set; }
    public string? PhilhealthNumber { get; set; }
    public string? PagibigNumber { get; set; }
    public string? TinNumber { get; set; }
    
    // Bank Account Information
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? BankAccountName { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    // Navigation properties
    public ICollection<EmployeeAssignment> Assignments { get; set; } = new List<EmployeeAssignment>();
    public ICollection<EmployeeCompensation> Compensations { get; set; } = new List<EmployeeCompensation>();
    public EmployeeCompensation? CurrentCompensation { get; set; }
    public ICollection<EmployeeAllowance> Allowances { get; set; } = new List<EmployeeAllowance>();
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public ICollection<EmployeeLeave> Leaves { get; set; } = new List<EmployeeLeave>();
    public ICollection<PayrollHeader> PayrollHeaders { get; set; } = new List<PayrollHeader>();
}
