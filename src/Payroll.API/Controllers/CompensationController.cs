using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payroll.Core.Entities;
using Payroll.Infrastructure.Data;

namespace Payroll.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,HR")]
public class CompensationController : ControllerBase
{
    private readonly PayrollDbContext _context;
    private readonly ILogger<CompensationController> _logger;

    public CompensationController(PayrollDbContext context, ILogger<CompensationController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all compensation records for an employee
    /// </summary>
    [HttpGet("employee/{employeeId}")]
    public async Task<ActionResult<List<CompensationResponse>>> GetByEmployee(Guid employeeId)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        if (employee == null)
            return NotFound(new { message = $"Employee with ID {employeeId} not found" });

        var compensations = await _context.EmployeeCompensations
            .Where(c => c.EmployeeId == employeeId)
            .OrderByDescending(c => c.EffectiveDate)
            .Select(c => new CompensationResponse
            {
                CompensationId = c.CompensationId,
                EmployeeId = c.EmployeeId,
                EmployeeName = employee.FirstName + " " + employee.LastName,
                BasicSalary = c.BasicSalary,
                DailyRate = c.DailyRate,
                HourlyRate = c.HourlyRate,
                PayFrequency = c.PayFrequency,
                Cola = c.Cola,
                EffectiveDate = c.EffectiveDate,
                EndDate = c.EndDate,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        return Ok(compensations);
    }

    /// <summary>
    /// Get all compensation records (with employee info)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<CompensationResponse>>> GetAll()
    {
        var compensations = await _context.EmployeeCompensations
            .Include(c => c.Employee)
            .OrderByDescending(c => c.EffectiveDate)
            .Select(c => new CompensationResponse
            {
                CompensationId = c.CompensationId,
                EmployeeId = c.EmployeeId,
                EmployeeName = c.Employee.FirstName + " " + c.Employee.LastName,
                EmployeeCode = c.Employee.EmployeeCode,
                BasicSalary = c.BasicSalary,
                DailyRate = c.DailyRate,
                HourlyRate = c.HourlyRate,
                PayFrequency = c.PayFrequency,
                Cola = c.Cola,
                EffectiveDate = c.EffectiveDate,
                EndDate = c.EndDate,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        return Ok(compensations);
    }

    /// <summary>
    /// Create new compensation record
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CompensationResponse>> Create(CreateCompensationRequest request)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == request.EmployeeId);
        if (employee == null)
            return NotFound(new { message = $"Employee with ID {request.EmployeeId} not found" });

        // Deactivate previous active compensation for this employee
        var activeComps = await _context.EmployeeCompensations
            .Where(c => c.EmployeeId == request.EmployeeId && c.IsActive)
            .ToListAsync();

        foreach (var comp in activeComps)
        {
            comp.IsActive = false;
            comp.EndDate = request.EffectiveDate.AddDays(-1);
        }

        var compensation = new EmployeeCompensation
        {
            EmployeeId = request.EmployeeId,
            BasicSalary = request.BasicSalary,
            DailyRate = request.DailyRate ?? (request.BasicSalary / 22), // 22 working days default
            HourlyRate = request.HourlyRate ?? (request.BasicSalary / 22 / 8), // 8 hours default
            PayFrequency = request.PayFrequency ?? "Monthly",
            Cola = request.Cola,
            EffectiveDate = DateTime.SpecifyKind(request.EffectiveDate, DateTimeKind.Utc),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.EmployeeCompensations.Add(compensation);

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created compensation for employee {EmployeeId}, salary: {Salary}",
                request.EmployeeId, request.BasicSalary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating compensation");
            return StatusCode(500, new { message = "Error creating compensation record", error = ex.Message });
        }

        return Ok(new CompensationResponse
        {
            CompensationId = compensation.CompensationId,
            EmployeeId = compensation.EmployeeId,
            EmployeeName = employee.FirstName + " " + employee.LastName,
            EmployeeCode = employee.EmployeeCode,
            BasicSalary = compensation.BasicSalary,
            DailyRate = compensation.DailyRate,
            HourlyRate = compensation.HourlyRate,
            PayFrequency = compensation.PayFrequency,
            Cola = compensation.Cola,
            EffectiveDate = compensation.EffectiveDate,
            EndDate = compensation.EndDate,
            IsActive = compensation.IsActive,
            CreatedAt = compensation.CreatedAt
        });
    }

    /// <summary>
    /// Update compensation record
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateCompensationRequest request)
    {
        var compensation = await _context.EmployeeCompensations.FindAsync(id);
        if (compensation == null)
            return NotFound(new { message = $"Compensation with ID {id} not found" });

        compensation.BasicSalary = request.BasicSalary;
        compensation.DailyRate = request.DailyRate ?? (request.BasicSalary / 22);
        compensation.HourlyRate = request.HourlyRate ?? (request.BasicSalary / 22 / 8);
        compensation.PayFrequency = request.PayFrequency ?? compensation.PayFrequency;
        compensation.Cola = request.Cola;

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated compensation {CompId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating compensation {CompId}", id);
            return StatusCode(500, new { message = "Error updating compensation", error = ex.Message });
        }

        return Ok(new { message = "Compensation updated successfully" });
    }
}

// DTOs
public class CompensationResponse
{
    public int CompensationId { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public decimal BasicSalary { get; set; }
    public decimal? DailyRate { get; set; }
    public decimal? HourlyRate { get; set; }
    public string PayFrequency { get; set; } = string.Empty;
    public decimal Cola { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCompensationRequest
{
    public Guid EmployeeId { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal? DailyRate { get; set; }
    public decimal? HourlyRate { get; set; }
    public string? PayFrequency { get; set; }
    public decimal Cola { get; set; }
    public DateTime EffectiveDate { get; set; }
}

public class UpdateCompensationRequest
{
    public decimal BasicSalary { get; set; }
    public decimal? DailyRate { get; set; }
    public decimal? HourlyRate { get; set; }
    public string? PayFrequency { get; set; }
    public decimal Cola { get; set; }
}
