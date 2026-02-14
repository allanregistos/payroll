using PayrollWeb.Models;

namespace PayrollWeb.Services;

public interface IAuthenticationService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task LogoutAsync();
    Task<UserResponse?> GetCurrentUserAsync();
    Task<bool> IsAuthenticatedAsync();
}
