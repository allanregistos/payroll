namespace Payroll.API.DTOs;

public record LoginRequest(
    string Username,
    string Password
);

public record LoginResponse(
    string Token,
    UserResponse User
);

public record UserResponse(
    Guid UserId,
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    bool IsActive,
    Guid? EmployeeId
);

public record RegisterRequest(
    string Username,
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Role, // Admin, HR, Employee
    Guid? EmployeeId
);

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword
);

