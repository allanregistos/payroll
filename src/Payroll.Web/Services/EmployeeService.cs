using Blazored.LocalStorage;
using PayrollWeb.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PayrollWeb.Services;

public interface IEmployeeService
{
    Task<List<EmployeeListDto>> GetAllAsync();
    Task<EmployeeDto?> GetByIdAsync(Guid id);
    Task<EmployeeDto?> CreateAsync(EmployeeDto employee);
    Task<bool> UpdateAsync(Guid id, EmployeeDto employee);
    Task<bool> DeleteAsync(Guid id);
}

public class EmployeeService : IEmployeeService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public EmployeeService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    private async Task SetAuthHeaderAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<List<EmployeeListDto>> GetAllAsync()
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetFromJsonAsync<List<EmployeeListDto>>("api/employees");
            return response ?? new List<EmployeeListDto>();
        }
        catch
        {
            return new List<EmployeeListDto>();
        }
    }

    public async Task<EmployeeDto?> GetByIdAsync(Guid id)
    {
        try
        {
            await SetAuthHeaderAsync();
            return await _httpClient.GetFromJsonAsync<EmployeeDto>($"api/employees/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<EmployeeDto?> CreateAsync(EmployeeDto employee)
    {
        try
        {
            await SetAuthHeaderAsync();
            
            // Convert dates to UTC for PostgreSQL
            if (employee.DateOfBirth.Kind == DateTimeKind.Unspecified)
                employee.DateOfBirth = DateTime.SpecifyKind(employee.DateOfBirth, DateTimeKind.Utc);
            if (employee.HireDate.Kind == DateTimeKind.Unspecified)
                employee.HireDate = DateTime.SpecifyKind(employee.HireDate, DateTimeKind.Utc);
            
            var response = await _httpClient.PostAsJsonAsync("api/employees", employee);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<EmployeeDto>();
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Create employee failed: {response.StatusCode} - {errorContent}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Create employee error: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateAsync(Guid id, EmployeeDto employee)
    {
        try
        {
            await SetAuthHeaderAsync();
            
            // Convert dates to UTC for PostgreSQL
            if (employee.DateOfBirth.Kind == DateTimeKind.Unspecified)
                employee.DateOfBirth = DateTime.SpecifyKind(employee.DateOfBirth, DateTimeKind.Utc);
            if (employee.HireDate.Kind == DateTimeKind.Unspecified)
                employee.HireDate = DateTime.SpecifyKind(employee.HireDate, DateTimeKind.Utc);
            
            var response = await _httpClient.PutAsJsonAsync($"api/employees/{id}", employee);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Update employee failed: {response.StatusCode} - {errorContent}");
            }
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Update employee error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.DeleteAsync($"api/employees/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
