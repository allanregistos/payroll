using PayrollWeb.Models;
using System.Net.Http.Json;

namespace PayrollWeb.Services;

public class PayrollService : IPayrollService
{
    private readonly HttpClient _httpClient;

    public PayrollService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<PayrollDto>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<PayrollDto>>("api/payroll");
            return response ?? new List<PayrollDto>();
        }
        catch
        {
            return new List<PayrollDto>();
        }
    }

    public async Task<List<PayrollDto>> GetByEmployeeIdAsync(Guid employeeId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<PayrollDto>>($"api/payroll/employee/{employeeId}");
            return response ?? new List<PayrollDto>();
        }
        catch
        {
            return new List<PayrollDto>();
        }
    }

    public async Task<PayrollDto?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PayrollDto>($"api/payroll/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<(PayrollDto? Result, string? Error)> ComputePayrollAsync(PayrollComputeRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/payroll/compute", request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PayrollDto>();
                return (result, null);
            }
            
            // Try to extract error message from API response
            try
            {
                var errorBody = await response.Content.ReadFromJsonAsync<ApiError>();
                return (null, errorBody?.Message ?? $"Failed with status {response.StatusCode}");
            }
            catch
            {
                return (null, $"Payroll computation failed ({response.StatusCode})");
            }
        }
        catch (Exception ex)
        {
            return (null, $"Error: {ex.Message}");
        }
    }

    public async Task<bool> ApprovePayrollAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.PostAsync($"api/payroll/{id}/approve", null);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
