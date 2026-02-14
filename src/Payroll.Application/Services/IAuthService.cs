using Payroll.Core.Entities;

namespace Payroll.Application.Services;

public interface IAuthService
{
    Task<(bool Success, string Token, User? User)> LoginAsync(string username, string password);
    Task<(bool Success, string Message, User? User)> RegisterAsync(string username, string email, string password, string firstName, string lastName, string role, Guid? employeeId);
    Task<(bool Success, string Message)> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
    string GenerateJwtToken(User user);
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
