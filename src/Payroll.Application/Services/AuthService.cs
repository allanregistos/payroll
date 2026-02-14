using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Payroll.Core.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Payroll.Application.Services;

public class AuthService : IAuthService
{
    private readonly DbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(DbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<(bool Success, string Token, User? User)> LoginAsync(string username, string password)
    {
        var user = await _context.Set<User>()
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

        if (user == null)
            return (false, string.Empty, null);

        if (!VerifyPassword(password, user.PasswordHash))
            return (false, string.Empty, null);

        var token = GenerateJwtToken(user);
        return (true, token, user);
    }

    public async Task<(bool Success, string Message, User? User)> RegisterAsync(
        string username, string email, string password, string firstName, 
        string lastName, string role, Guid? employeeId)
    {
        // Validate role
        var validRoles = new[] { "Admin", "HR", "Employee" };
        if (!validRoles.Contains(role))
            return (false, "Invalid role. Must be Admin, HR, or Employee.", null);

        // Check if username already exists
        var existingUser = await _context.Set<User>()
            .AnyAsync(u => u.Username == username);
        if (existingUser)
            return (false, "Username already exists.", null);

        // Check if email already exists
        var existingEmail = await _context.Set<User>()
            .AnyAsync(u => u.Email == email);
        if (existingEmail)
            return (false, "Email already exists.", null);

        // If employeeId is provided, verify it exists
        if (employeeId.HasValue)
        {
            var employeeExists = await _context.Set<Employee>()
                .AnyAsync(e => e.EmployeeId == employeeId.Value);
            if (!employeeExists)
                return (false, "Employee not found.", null);
        }

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = username,
            Email = email,
            PasswordHash = HashPassword(password),
            FirstName = firstName,
            LastName = lastName,
            Role = role,
            IsActive = true,
            EmployeeId = employeeId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Set<User>().Add(user);
        await _context.SaveChangesAsync();

        return (true, "User registered successfully.", user);
    }

    public async Task<(bool Success, string Message)> ChangePasswordAsync(
        Guid userId, string currentPassword, string newPassword)
    {
        var user = await _context.Set<User>()
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            return (false, "User not found.");

        if (!VerifyPassword(currentPassword, user.PasswordHash))
            return (false, "Current password is incorrect.");

        user.PasswordHash = HashPassword(newPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return (true, "Password changed successfully.");
    }

    public string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        var issuer = jwtSettings["Issuer"] ?? "PayrollAPI";
        var audience = jwtSettings["Audience"] ?? "PayrollAPIUsers";
        var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("EmployeeId", user.EmployeeId?.ToString() ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        var hashedPassword = HashPassword(password);
        return hashedPassword == passwordHash;
    }
}
