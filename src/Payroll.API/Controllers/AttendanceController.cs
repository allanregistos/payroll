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
public class AttendanceController : ControllerBase
{
    private readonly PayrollDbContext _context;
    private readonly ILogger<AttendanceController> _logger;

    public AttendanceController(PayrollDbContext context, ILogger<AttendanceController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get attendance records
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AttendanceResponse>>> GetAttendance(
        [FromQuery] Guid? employeeId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var query = _context.Set<Attendance>()
            .Include(a => a.Employee)
            .AsQueryable();

        if (employeeId.HasValue)
        {
            query = query.Where(a => a.EmployeeId == employeeId.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(a => a.AttendanceDate.Date >= startDate.Value.Date);
        }

        if (endDate.HasValue)
        {
            query = query.Where(a => a.AttendanceDate.Date <= endDate.Value.Date);
        }

        var attendances = await query
            .OrderByDescending(a => a.AttendanceDate)
            .Select(a => new AttendanceResponse
            {
                AttendanceId = a.AttendanceId,
                EmployeeId = a.EmployeeId,
                EmployeeCode = a.Employee.EmployeeCode,
                EmployeeName = a.Employee.FirstName + " " + a.Employee.LastName,
                AttendanceDate = a.AttendanceDate,
                TimeIn = a.TimeIn,
                TimeOut = a.TimeOut,
                RegularHours = a.RegularHours,
                OvertimeHours = a.OvertimeHours,
                LateMinutes = a.LateMinutes,
                UndertimeMinutes = a.UndertimeMinutes,
                IsHoliday = a.IsHoliday,
                Remarks = a.Remarks,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();

        return Ok(attendances);
    }

    /// <summary>
    /// Get attendance by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AttendanceResponse>> GetAttendanceById(int id)
    {
        var attendance = await _context.Set<Attendance>()
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.AttendanceId == id);

        if (attendance == null)
        {
            return NotFound(new { message = $"Attendance record with ID {id} not found" });
        }

        var response = new AttendanceResponse
        {
            AttendanceId = attendance.AttendanceId,
            EmployeeId = attendance.EmployeeId,
            EmployeeCode = attendance.Employee.EmployeeCode,
            EmployeeName = $"{attendance.Employee.FirstName} {attendance.Employee.LastName}",
            AttendanceDate = attendance.AttendanceDate,
            TimeIn = attendance.TimeIn,
            TimeOut = attendance.TimeOut,
            RegularHours = attendance.RegularHours,
            OvertimeHours = attendance.OvertimeHours,
            LateMinutes = attendance.LateMinutes,
            UndertimeMinutes = attendance.UndertimeMinutes,
            IsHoliday = attendance.IsHoliday,
            Remarks = attendance.Remarks,
            CreatedAt = attendance.CreatedAt
        };

        return Ok(response);
    }

    /// <summary>
    /// Create attendance record
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AttendanceResponse>> CreateAttendance(CreateAttendanceRequest request)
    {
        // Verify employee exists
        if (!await _context.Employees.AnyAsync(e => e.EmployeeId == request.EmployeeId))
        {
            return BadRequest(new { message = "Employee not found" });
        }

        // Check if attendance already exists for this date
        if (await _context.Set<Attendance>()
            .AnyAsync(a => a.EmployeeId == request.EmployeeId && a.AttendanceDate == request.AttendanceDate))
        {
            return BadRequest(new { message = "Attendance record already exists for this date" });
        }

        // Check if date is a holiday
        var isHoliday = await _context.Set<Holiday>()
            .AnyAsync(h => h.HolidayDate == request.AttendanceDate && h.IsActive);

        var attendance = new Attendance
        {
            EmployeeId = request.EmployeeId,
            AttendanceDate = request.AttendanceDate,
            TimeIn = request.TimeIn,
            TimeOut = request.TimeOut,
            RegularHours = request.RegularHours,
            OvertimeHours = request.OvertimeHours,
            LateMinutes = request.LateMinutes,
            UndertimeMinutes = request.UndertimeMinutes,
            IsHoliday = isHoliday,
            Remarks = request.Remarks,
            CreatedAt = DateTime.UtcNow
        };

        _context.Set<Attendance>().Add(attendance);

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created attendance record for employee {EmployeeId} on {Date}", 
                request.EmployeeId, request.AttendanceDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating attendance record");
            return StatusCode(500, new { message = "Error creating attendance record", error = ex.Message });
        }

        // Load employee for response
        await _context.Entry(attendance).Reference(a => a.Employee).LoadAsync();

        var response = new AttendanceResponse
        {
            AttendanceId = attendance.AttendanceId,
            EmployeeId = attendance.EmployeeId,
            EmployeeCode = attendance.Employee.EmployeeCode,
            EmployeeName = $"{attendance.Employee.FirstName} {attendance.Employee.LastName}",
            AttendanceDate = attendance.AttendanceDate,
            TimeIn = attendance.TimeIn,
            TimeOut = attendance.TimeOut,
            RegularHours = attendance.RegularHours,
            OvertimeHours = attendance.OvertimeHours,
            LateMinutes = attendance.LateMinutes,
            UndertimeMinutes = attendance.UndertimeMinutes,
            IsHoliday = attendance.IsHoliday,
            Remarks = attendance.Remarks,
            CreatedAt = attendance.CreatedAt
        };

        return CreatedAtAction(nameof(GetAttendanceById), new { id = attendance.AttendanceId }, response);
    }

    /// <summary>
    /// Update attendance record
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<AttendanceResponse>> UpdateAttendance(int id, UpdateAttendanceRequest request)
    {
        var attendance = await _context.Set<Attendance>()
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.AttendanceId == id);

        if (attendance == null)
        {
            return NotFound(new { message = $"Attendance record with ID {id} not found" });
        }

        attendance.TimeIn = request.TimeIn;
        attendance.TimeOut = request.TimeOut;
        attendance.RegularHours = request.RegularHours;
        attendance.OvertimeHours = request.OvertimeHours;
        attendance.LateMinutes = request.LateMinutes;
        attendance.UndertimeMinutes = request.UndertimeMinutes;
        attendance.Remarks = request.Remarks;
        attendance.UpdatedAt = DateTime.UtcNow;
        attendance.UpdatedBy = null;

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated attendance record {AttendanceId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating attendance record {AttendanceId}", id);
            return StatusCode(500, new { message = "Error updating attendance record", error = ex.Message });
        }

        var response = new AttendanceResponse
        {
            AttendanceId = attendance.AttendanceId,
            EmployeeId = attendance.EmployeeId,
            EmployeeCode = attendance.Employee.EmployeeCode,
            EmployeeName = $"{attendance.Employee.FirstName} {attendance.Employee.LastName}",
            AttendanceDate = attendance.AttendanceDate,
            TimeIn = attendance.TimeIn,
            TimeOut = attendance.TimeOut,
            RegularHours = attendance.RegularHours,
            OvertimeHours = attendance.OvertimeHours,
            LateMinutes = attendance.LateMinutes,
            UndertimeMinutes = attendance.UndertimeMinutes,
            IsHoliday = attendance.IsHoliday,
            Remarks = attendance.Remarks,
            CreatedAt = attendance.CreatedAt
        };

        return Ok(response);
    }

    /// <summary>
    /// Delete attendance record
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAttendance(int id)
    {
        var attendance = await _context.Set<Attendance>().FindAsync(id);

        if (attendance == null)
        {
            return NotFound(new { message = $"Attendance record with ID {id} not found" });
        }

        _context.Set<Attendance>().Remove(attendance);

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted attendance record {AttendanceId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting attendance record {AttendanceId}", id);
            return StatusCode(500, new { message = "Error deleting attendance record", error = ex.Message });
        }

        return NoContent();
    }
}
