using PayrollWeb.Models;

namespace PayrollWeb.Services;

public interface ILeaveService
{
    Task<List<LeaveDto>> GetAllAsync();
    Task<List<LeaveDto>> GetByEmployeeIdAsync(Guid employeeId);
    Task<LeaveDto?> GetByIdAsync(Guid id);
    Task<LeaveDto?> CreateAsync(LeaveCreateDto leave);
    Task<bool> UpdateAsync(Guid id, LeaveUpdateDto leave);
    Task<bool> DeleteAsync(Guid id);
}
