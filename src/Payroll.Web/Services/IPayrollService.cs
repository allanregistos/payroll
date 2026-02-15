using PayrollWeb.Models;

namespace PayrollWeb.Services;

public interface IPayrollService
{
    Task<List<PayrollDto>> GetAllAsync();
    Task<List<PayrollDto>> GetByEmployeeIdAsync(Guid employeeId);
    Task<PayrollDto?> GetByIdAsync(Guid id);
    Task<(PayrollDto? Result, string? Error)> ComputePayrollAsync(PayrollComputeRequest request);
    Task<bool> ApprovePayrollAsync(Guid id);
}
