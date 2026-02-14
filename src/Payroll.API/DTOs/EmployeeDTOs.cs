namespace Payroll.API.DTOs;

/// <summary>
/// Data Transfer Objects for Employee API
/// </summary>
/// 
public class CreateEmployeeRequest
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    
    // Government IDs
    public string? SssNumber { get; set; }
    public string? PhilhealthNumber { get; set; }
    public string? PagibigNumber { get; set; }
    public string? TinNumber { get; set; }
    
    // Employment Details
    public DateTime HireDate { get; set; }
    public string EmploymentStatus { get; set; } = "Regular"; // Regular, Probationary, Contractual
    public decimal BasicSalary { get; set; }
}

public class UpdateEmployeeRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    
    // Government IDs
    public string? SssNumber { get; set; }
    public string? PhilhealthNumber { get; set; }
    public string? PagibigNumber { get; set; }
    public string? TinNumber { get; set; }
    
    public string EmploymentStatus { get; set; } = "Regular";
}

public class EmployeeResponse
{
    public Guid EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    
    public string? SssNumber { get; set; }
    public string? PhilhealthNumber { get; set; }
    public string? PagibigNumber { get; set; }
    public string? TinNumber { get; set; }
    
    public DateTime HireDate { get; set; }
    public string EmploymentStatus { get; set; } = string.Empty;
    public decimal BasicSalary { get; set; }
    public bool IsActive { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class EmployeeListResponse
{
    public Guid EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string EmploymentStatus { get; set; } = string.Empty;
    public decimal BasicSalary { get; set; }
    public bool IsActive { get; set; }
}
