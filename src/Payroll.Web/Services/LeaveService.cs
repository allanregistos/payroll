using PayrollWeb.Models;
using System.Net.Http.Json;

namespace PayrollWeb.Services;

public class LeaveService : ILeaveService
{
    private readonly HttpClient _httpClient;

    public LeaveService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<LeaveDto>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<LeaveDto>>("api/leaves");
            return response ?? new List<LeaveDto>();
        }
        catch
        {
            return new List<LeaveDto>();
        }
    }

    public async Task<List<LeaveDto>> GetByEmployeeIdAsync(Guid employeeId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<LeaveDto>>($"api/leaves/employee/{employeeId}");
            return response ?? new List<LeaveDto>();
        }
        catch
        {
            return new List<LeaveDto>();
        }
    }

    public async Task<LeaveDto?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<LeaveDto>($"api/leaves/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<LeaveDto?> CreateAsync(LeaveCreateDto leave)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/leaves", leave);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<LeaveDto>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateAsync(Guid id, LeaveUpdateDto leave)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/leaves/{id}", leave);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/leaves/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
