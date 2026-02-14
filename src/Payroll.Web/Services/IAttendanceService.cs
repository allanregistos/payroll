using PayrollWeb.Models;

namespace PayrollWeb.Services;

public interface IAttendanceService
{
    Task<List<AttendanceDto>> GetAllAsync();
    Task<List<AttendanceDto>> GetByEmployeeIdAsync(Guid employeeId);
    Task<List<AttendanceDto>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate);
    Task<AttendanceDto?> GetByIdAsync(Guid id);
    Task<AttendanceDto?> CreateAsync(AttendanceCreateDto attendance);
    Task<bool> UpdateAsync(Guid id, AttendanceUpdateDto attendance);
    Task<bool> DeleteAsync(Guid id);
}
