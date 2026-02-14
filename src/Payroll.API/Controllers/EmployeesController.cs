using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payroll.API.DTOs;
using Payroll.Core.Entities;
using Payroll.Infrastructure.Data;

namespace Payroll.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,HR")]
public class EmployeesController : ControllerBase
{
    private readonly PayrollDbContext _context;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(PayrollDbContext context, ILogger<EmployeesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all employees
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeListResponse>>> GetEmployees(
        [FromQuery] bool includeInactive = false)
    {
        var query = _context.Employees
            .Include(e => e.Compensations)
            .AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(e => e.IsActive);
        }

        var employees = await query
            .OrderBy(e => e.EmployeeCode)
            .Select(e => new EmployeeListResponse
            {
                EmployeeId = e.EmployeeId,
                EmployeeCode = e.EmployeeCode,
                FullName = e.FirstName + " " + e.LastName,
                Email = e.Email,
                EmploymentStatus = e.EmploymentStatus,
                BasicSalary = e.Compensations.OrderByDescending(c => c.EffectiveDate).FirstOrDefault() != null ? e.Compensations.OrderByDescending(c => c.EffectiveDate).FirstOrDefault().BasicSalary : 0,
                IsActive = e.IsActive
            })
            .ToListAsync();

        return Ok(employees);
    }

    /// <summary>
    /// Get employee by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeResponse>> GetEmployee(Guid id)
    {
        var employee = await _context.Employees
            .Include(e => e.Compensations)
            .FirstOrDefaultAsync(e => e.EmployeeId == id);

        if (employee == null)
        {
            return NotFound(new { message = $"Employee with ID {id} not found" });
        }

        var currentCompensation = employee.Compensations
            .OrderByDescending(c => c.EffectiveDate)
            .FirstOrDefault();

        var response = new EmployeeResponse
        {
            EmployeeId = employee.EmployeeId,
            EmployeeCode = employee.EmployeeCode,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            MiddleName = employee.MiddleName,
            FullName = $"{employee.FirstName} {employee.MiddleName} {employee.LastName}".Replace("  ", " "),
            DateOfBirth = employee.DateOfBirth,
            Email = employee.Email,
            PhoneNumber = employee.PhoneNumber,
            Address = employee.Address,
            SssNumber = employee.SssNumber,
            PhilhealthNumber = employee.PhilhealthNumber,
            PagibigNumber = employee.PagibigNumber,
            TinNumber = employee.TinNumber,
            HireDate = employee.HireDate,
            EmploymentStatus = employee.EmploymentStatus,
            BasicSalary = currentCompensation?.BasicSalary ?? 0,
            IsActive = employee.IsActive,
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt
        };

        return Ok(response);
    }

    /// <summary>
    /// Create new employee
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<EmployeeResponse>> CreateEmployee(CreateEmployeeRequest request)
    {
        // Check if employee code already exists
        if (await _context.Employees.AnyAsync(e => e.EmployeeCode == request.EmployeeCode))
        {
            return BadRequest(new { message = $"Employee code {request.EmployeeCode} already exists" });
        }

        var employee = new Employee
        {
            EmployeeId = Guid.NewGuid(),
            EmployeeCode = request.EmployeeCode,
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            DateOfBirth = request.DateOfBirth,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            SssNumber = request.SssNumber,
            PhilhealthNumber = request.PhilhealthNumber,
            PagibigNumber = request.PagibigNumber,
            TinNumber = request.TinNumber,
            HireDate = request.HireDate,
            EmploymentStatus = request.EmploymentStatus,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Employees.Add(employee);

        // Create initial compensation record
        var compensation = new EmployeeCompensation
        {
            EmployeeId = employee.EmployeeId,
            BasicSalary = request.BasicSalary,
            EffectiveDate = request.HireDate,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Set<EmployeeCompensation>().Add(compensation);

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created employee {EmployeeCode} with ID {EmployeeId}", 
                employee.EmployeeCode, employee.EmployeeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating employee {EmployeeCode}", request.EmployeeCode);
            return StatusCode(500, new { message = "Error creating employee", error = ex.Message, innerError = ex.InnerException?.Message });
        }

        var response = new EmployeeResponse
        {
            EmployeeId = employee.EmployeeId,
            EmployeeCode = employee.EmployeeCode,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            MiddleName = employee.MiddleName,
            FullName = $"{employee.FirstName} {employee.MiddleName} {employee.LastName}".Replace("  ", " "),
            DateOfBirth = employee.DateOfBirth,
            Email = employee.Email,
            PhoneNumber = employee.PhoneNumber,
            Address = employee.Address,
            SssNumber = employee.SssNumber,
            PhilhealthNumber = employee.PhilhealthNumber,
            PagibigNumber = employee.PagibigNumber,
            TinNumber = employee.TinNumber,
            HireDate = employee.HireDate,
            EmploymentStatus = employee.EmploymentStatus,
            BasicSalary = compensation.BasicSalary,
            IsActive = employee.IsActive,
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt
        };

        return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeId }, response);
    }

    /// <summary>
    /// Update employee
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<EmployeeResponse>> UpdateEmployee(Guid id, UpdateEmployeeRequest request)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            return NotFound(new { message = $"Employee with ID {id} not found" });
        }

        employee.FirstName = request.FirstName;
        employee.LastName = request.LastName;
        employee.MiddleName = request.MiddleName;
        employee.DateOfBirth = request.DateOfBirth;
        employee.Email = request.Email;
        employee.PhoneNumber = request.PhoneNumber;
        employee.Address = request.Address;
        employee.SssNumber = request.SssNumber;
        employee.PhilhealthNumber = request.PhilhealthNumber;
        employee.PagibigNumber = request.PagibigNumber;
        employee.TinNumber = request.TinNumber;
        employee.EmploymentStatus = request.EmploymentStatus;
        employee.UpdatedAt = DateTime.UtcNow;
        employee.UpdatedBy = null; // TODO: Get from authenticated user

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated employee {EmployeeCode}", employee.EmployeeCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating employee {EmployeeId}", id);
            return StatusCode(500, new { message = "Error updating employee", error = ex.Message });
        }

        // Reload with compensation
        await _context.Entry(employee).Reference(e => e.CurrentCompensation).LoadAsync();

        var response = new EmployeeResponse
        {
            EmployeeId = employee.EmployeeId,
            EmployeeCode = employee.EmployeeCode,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            MiddleName = employee.MiddleName,
            FullName = $"{employee.FirstName} {employee.MiddleName} {employee.LastName}".Replace("  ", " "),
            DateOfBirth = employee.DateOfBirth,
            Email = employee.Email,
            PhoneNumber = employee.PhoneNumber,
            Address = employee.Address,
            SssNumber = employee.SssNumber,
            PhilhealthNumber = employee.PhilhealthNumber,
            PagibigNumber = employee.PagibigNumber,
            TinNumber = employee.TinNumber,
            HireDate = employee.HireDate,
            EmploymentStatus = employee.EmploymentStatus,
            BasicSalary = employee.CurrentCompensation?.BasicSalary ?? 0,
            IsActive = employee.IsActive,
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt
        };

        return Ok(response);
    }

    /// <summary>
    /// Deactivate employee (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeactivateEmployee(Guid id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            return NotFound(new { message = $"Employee with ID {id} not found" });
        }

        employee.IsActive = false;
        employee.UpdatedAt = DateTime.UtcNow;
        employee.UpdatedBy = null; // TODO: Get from authenticated user

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deactivated employee {EmployeeCode}", employee.EmployeeCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating employee {EmployeeId}", id);
            return StatusCode(500, new { message = "Error deactivating employee", error = ex.Message });
        }

        return NoContent();
    }
}
