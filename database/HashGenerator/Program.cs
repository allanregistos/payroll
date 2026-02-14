using System.Security.Cryptography;
using System.Text;

var password = "Admin123";
using var sha256 = SHA256.Create();
var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
var hash = Convert.ToBase64String(hashedBytes);

Console.WriteLine($"Password: {password}");
Console.WriteLine($"SHA256 Hash: {hash}");
Console.WriteLine();
Console.WriteLine("Run this SQL in pgAdmin:");
Console.WriteLine($"UPDATE payroll.users SET password_hash = '{hash}' WHERE username = 'admin';");
