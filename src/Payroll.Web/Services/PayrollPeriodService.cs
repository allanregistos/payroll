using Blazored.LocalStorage;
using PayrollWeb.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PayrollWeb.Services;

public interface IPayrollPeriodService
{
    Task<List<PayrollPeriodDto>> GetAllAsync();
    Task<PayrollPeriodDto?> GetByIdAsync(int id);
    Task<PayrollPeriodDto?> CreateAsync(CreatePayrollPeriodDto dto);
    Task<bool> UpdateStatusAsync(int id, string status);
}

public class PayrollPeriodService : IPayrollPeriodService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public PayrollPeriodService(HttpClient httpClient, ILocalStorageService localStorage)
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

    public async Task<List<PayrollPeriodDto>> GetAllAsync()
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetFromJsonAsync<List<PayrollPeriodDto>>("api/payroll/periods");
            return response ?? new List<PayrollPeriodDto>();
        }
        catch
        {
            return new List<PayrollPeriodDto>();
        }
    }

    public async Task<PayrollPeriodDto?> GetByIdAsync(int id)
    {
        try
        {
            await SetAuthHeaderAsync();
            return await _httpClient.GetFromJsonAsync<PayrollPeriodDto>($"api/payroll/periods/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<PayrollPeriodDto?> CreateAsync(CreatePayrollPeriodDto dto)
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.PostAsJsonAsync("api/payroll/periods", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PayrollPeriodDto>();
            }
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Create period failed: {response.StatusCode} - {error}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Create period error: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateStatusAsync(int id, string status)
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.PutAsJsonAsync($"api/payroll/periods/{id}/status", new { Status = status });
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
