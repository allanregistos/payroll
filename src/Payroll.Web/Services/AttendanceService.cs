using Blazored.LocalStorage;
using PayrollWeb.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

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
            var response = await _httpClient.GetFromJsonAsync<List<AttendanceDto>>("api/attendance");
            return response ?? new List<AttendanceDto>();
        }
        catch
        {
            return new List<AttendanceDto>();
        }
    }

    public async Task<List<AttendanceDto>> GetByEmployeeIdAsync(Guid employeeId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<AttendanceDto>>($"api/attendance/employee/{employeeId}");
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
            var response = await _httpClient.GetFromJsonAsync<List<AttendanceDto>>(
                $"api/attendance/date-range?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
            return response ?? new List<AttendanceDto>();
        }
        catch
        {
            return new List<AttendanceDto>();
        }
    }

    public async Task<AttendanceDto?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<AttendanceDto>($"api/attendance/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<AttendanceDto?> CreateAsync(AttendanceCreateDto attendance)
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
                hoursWorked = attendance.HoursWorked,
                overtimeHours = attendance.OvertimeHours,
                isLate = attendance.IsLate,
                isAbsent = attendance.IsAbsent,
                remarks = attendance.Remarks
            };

            var response = await _httpClient.PostAsJsonAsync("api/attendance", apiRequest);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AttendanceDto>();
            }
            
            // Log error for debugging
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error creating attendance: {response.StatusCode} - {errorContent}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception creating attendance: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateAsync(Guid id, AttendanceUpdateDto attendance)
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
                hoursWorked = attendance.HoursWorked,
                overtimeHours = attendance.OvertimeHours,
                isLate = attendance.IsLate,
                isAbsent = attendance.IsAbsent,
                remarks = attendance.Remarks
            };

            var response = await _httpClient.PutAsJsonAsync($"api/attendance/{id}", apiRequest);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error updating attendance: {response.StatusCode} - {errorContent}");
            }
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception updating attendance: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
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
