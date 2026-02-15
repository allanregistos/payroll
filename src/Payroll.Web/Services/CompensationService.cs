using Blazored.LocalStorage;
using PayrollWeb.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PayrollWeb.Services;

public interface ICompensationService
{
    Task<List<CompensationDto>> GetAllAsync();
    Task<List<CompensationDto>> GetByEmployeeAsync(Guid employeeId);
    Task<CompensationDto?> CreateAsync(CreateCompensationDto dto);
    Task<bool> UpdateAsync(int id, CreateCompensationDto dto);
}

public class CompensationService : ICompensationService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public CompensationService(HttpClient httpClient, ILocalStorageService localStorage)
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

    public async Task<List<CompensationDto>> GetAllAsync()
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetFromJsonAsync<List<CompensationDto>>("api/compensation");
            return response ?? new List<CompensationDto>();
        }
        catch
        {
            return new List<CompensationDto>();
        }
    }

    public async Task<List<CompensationDto>> GetByEmployeeAsync(Guid employeeId)
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetFromJsonAsync<List<CompensationDto>>($"api/compensation/employee/{employeeId}");
            return response ?? new List<CompensationDto>();
        }
        catch
        {
            return new List<CompensationDto>();
        }
    }

    public async Task<CompensationDto?> CreateAsync(CreateCompensationDto dto)
    {
        try
        {
            await SetAuthHeaderAsync();

            if (dto.EffectiveDate.Kind == DateTimeKind.Unspecified)
                dto.EffectiveDate = DateTime.SpecifyKind(dto.EffectiveDate, DateTimeKind.Utc);

            var response = await _httpClient.PostAsJsonAsync("api/compensation", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CompensationDto>();
            }

            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Create compensation failed: {response.StatusCode} - {error}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Create compensation error: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateAsync(int id, CreateCompensationDto dto)
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.PutAsJsonAsync($"api/compensation/{id}", new
            {
                dto.BasicSalary,
                dto.DailyRate,
                dto.HourlyRate,
                dto.PayFrequency,
                dto.Cola
            });
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
