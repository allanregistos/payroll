using Blazored.LocalStorage;
using PayrollWeb.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace PayrollWeb.Services;

public class AttendanceService : IAttendanceService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public AttendanceService(HttpClient httpClient, ILocalStorageService localStorage)
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

    public async Task<List<AttendanceDto>> GetAllAsync()
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetFromJsonAsync<List<AttendanceDto>>("api/attendance");
            return response ?? new List<AttendanceDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading attendance: {ex.Message}");
            return new List<AttendanceDto>();
        }
    }

    public async Task<List<AttendanceDto>> GetByEmployeeIdAsync(Guid employeeId)
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetFromJsonAsync<List<AttendanceDto>>($"api/attendance?employeeId={employeeId}");
            return response ?? new List<AttendanceDto>();
        }
        catch
        {
            return new List<AttendanceDto>();
        }
    }

    public async Task<List<AttendanceDto>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetFromJsonAsync<List<AttendanceDto>>(
                $"api/attendance?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
            return response ?? new List<AttendanceDto>();
        }
        catch
        {
            return new List<AttendanceDto>();
        }
    }

    public async Task<AttendanceDto?> GetByIdAsync(int id)
    {
        try
        {
            await SetAuthHeaderAsync();
            return await _httpClient.GetFromJsonAsync<AttendanceDto>($"api/attendance/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<(AttendanceDto? Result, string? Error)> CreateAsync(AttendanceCreateDto attendance)
    {
        try
        {
            await SetAuthHeaderAsync();

            // Convert DateOnly and TimeOnly to DateTime for API
            var apiRequest = new
            {
                employeeId = attendance.EmployeeId,
                attendanceDate = DateTime.SpecifyKind(
                    attendance.AttendanceDate.ToDateTime(TimeOnly.MinValue), 
                    DateTimeKind.Utc),
                timeIn = attendance.TimeIn.HasValue 
                    ? DateTime.SpecifyKind(
                        attendance.AttendanceDate.ToDateTime(attendance.TimeIn.Value), 
                        DateTimeKind.Utc)
                    : (DateTime?)null,
                timeOut = attendance.TimeOut.HasValue 
                    ? DateTime.SpecifyKind(
                        attendance.AttendanceDate.ToDateTime(attendance.TimeOut.Value), 
                        DateTimeKind.Utc)
                    : (DateTime?)null,
                regularHours = attendance.RegularHours,
                overtimeHours = attendance.OvertimeHours,
                lateMinutes = attendance.LateMinutes,
                undertimeMinutes = attendance.UndertimeMinutes,
                remarks = attendance.Remarks
            };

            var response = await _httpClient.PostAsJsonAsync("api/attendance", apiRequest);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AttendanceDto>();
                return (result, null);
            }
            
            // Extract error message from API response
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error creating attendance: {response.StatusCode} - {errorContent}");
            
            // Try to parse the error message from JSON
            try
            {
                var errorObj = JsonSerializer.Deserialize<JsonElement>(errorContent);
                if (errorObj.TryGetProperty("message", out var messageElement))
                {
                    return (null, messageElement.GetString());
                }
            }
            catch { }
            
            return (null, $"Server error: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception creating attendance: {ex.Message}");
            return (null, $"Error: {ex.Message}");
        }
    }

    public async Task<(bool Success, string? Error)> UpdateAsync(int id, AttendanceUpdateDto attendance)
    {
        try
        {
            await SetAuthHeaderAsync();

            // Convert TimeOnly to DateTime for API
            var apiRequest = new
            {
                timeIn = attendance.TimeIn.HasValue 
                    ? DateTime.SpecifyKind(
                        DateTime.Today.Add(attendance.TimeIn.Value.ToTimeSpan()), 
                        DateTimeKind.Utc)
                    : (DateTime?)null,
                timeOut = attendance.TimeOut.HasValue 
                    ? DateTime.SpecifyKind(
                        DateTime.Today.Add(attendance.TimeOut.Value.ToTimeSpan()), 
                        DateTimeKind.Utc)
                    : (DateTime?)null,
                regularHours = attendance.RegularHours,
                overtimeHours = attendance.OvertimeHours,
                lateMinutes = attendance.LateMinutes,
                undertimeMinutes = attendance.UndertimeMinutes,
                remarks = attendance.Remarks
            };

            var response = await _httpClient.PutAsJsonAsync($"api/attendance/{id}", apiRequest);
            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error updating attendance: {response.StatusCode} - {errorContent}");
            
            try
            {
                var errorObj = JsonSerializer.Deserialize<JsonElement>(errorContent);
                if (errorObj.TryGetProperty("message", out var messageElement))
                {
                    return (false, messageElement.GetString());
                }
            }
            catch { }
            
            return (false, $"Server error: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception updating attendance: {ex.Message}");
            return (false, $"Error: {ex.Message}");
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.DeleteAsync($"api/attendance/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
