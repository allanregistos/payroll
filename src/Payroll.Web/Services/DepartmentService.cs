using Blazored.LocalStorage;
using PayrollWeb.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PayrollWeb.Services;

public interface IDepartmentService
{
    Task<List<DepartmentDto>> GetAllAsync();
}

public class DepartmentService : IDepartmentService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public DepartmentService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public async Task<List<DepartmentDto>> GetAllAsync()
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            
            var departments = await _httpClient.GetFromJsonAsync<List<DepartmentDto>>("api/Departments");
            return departments ?? new List<DepartmentDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading departments: {ex.Message}");
            return new List<DepartmentDto>();
        }
    }
}
