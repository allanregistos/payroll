using Blazored.LocalStorage;
using PayrollWeb.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PayrollWeb.Services;

public interface IPositionService
{
    Task<List<PositionDto>> GetAllAsync();
}

public class PositionService : IPositionService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public PositionService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public async Task<List<PositionDto>> GetAllAsync()
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            
            var positions = await _httpClient.GetFromJsonAsync<List<PositionDto>>("api/Positions");
            return positions ?? new List<PositionDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading positions: {ex.Message}");
            return new List<PositionDto>();
        }
    }
}
