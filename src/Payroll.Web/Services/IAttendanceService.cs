using PayrollWeb.Models;

namespace PayrollWeb.Services;

public interface IAttendanceService
{
    Task<List<AttendanceDto>> GetAllAsync();
    Task<List<AttendanceDto>> GetByEmployeeIdAsync(Guid employeeId);
    Task<List<AttendanceDto>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate);
    Task<AttendanceDto?> GetByIdAsync(int id);
    Task<(AttendanceDto? Result, string? Error)> CreateAsync(AttendanceCreateDto attendance);
    Task<(bool Success, string? Error)> UpdateAsync(int id, AttendanceUpdateDto attendance);
    Task<bool> DeleteAsync(int id);
}
