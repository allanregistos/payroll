using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payroll.API.DTOs;
using Payroll.Core.Entities;
using Payroll.Infrastructure.Data;

namespace Payroll.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeavesController : ControllerBase
{
    private readonly PayrollDbContext _context;
    private readonly ILogger<LeavesController> _logger;

    public LeavesController(PayrollDbContext context, ILogger<LeavesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get leave requests
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LeaveResponse>>> GetLeaves(
        [FromQuery] Guid? employeeId,
        [FromQuery] string? status)
    {
        var query = _context.Set<EmployeeLeave>()
            .Include(l => l.Employee)
            .Include(l => l.LeaveType)
            .AsQueryable();

        if (employeeId.HasValue)
        {
            query = query.Where(l => l.EmployeeId == employeeId.Value);
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(l => l.Status == status);
        }

        var leaves = await query
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new LeaveResponse
            {
                LeaveId = l.LeaveId,
                EmployeeId = l.EmployeeId,
                EmployeeCode = l.Employee.EmployeeCode,
                EmployeeName = l.Employee.FirstName + " " + l.Employee.LastName,
                LeaveTypeId = l.LeaveTypeId,
                LeaveTypeName = l.LeaveType.LeaveTypeName,
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                NumberOfDays = l.NumberOfDays,
                Status = l.Status,
                Reason = l.Reason,
                ApprovedBy = l.ApprovedBy,
                ApprovedDate = l.ApprovedDate,
                ApprovalRemarks = l.ApprovalRemarks,
                CreatedAt = l.CreatedAt
            })
            .ToListAsync();

        return Ok(leaves);
    }

    /// <summary>
    /// Get leave request by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<LeaveResponse>> GetLeave(int id)
    {
        var leave = await _context.Set<EmployeeLeave>()
            .Include(l => l.Employee)
            .Include(l => l.LeaveType)
            .FirstOrDefaultAsync(l => l.LeaveId == id);

        if (leave == null)
        {
            return NotFound(new { message = $"Leave request with ID {id} not found" });
        }

        var response = new LeaveResponse
        {
            LeaveId = leave.LeaveId,
            EmployeeId = leave.EmployeeId,
            EmployeeCode = leave.Employee.EmployeeCode,
            EmployeeName = $"{leave.Employee.FirstName} {leave.Employee.LastName}",
            LeaveTypeId = leave.LeaveTypeId,
            LeaveTypeName = leave.LeaveType.LeaveTypeName,
            StartDate = leave.StartDate,
            EndDate = leave.EndDate,
            NumberOfDays = leave.NumberOfDays,
            Status = leave.Status,
            Reason = leave.Reason,
            ApprovedBy = leave.ApprovedBy,
            ApprovedDate = leave.ApprovedDate,
            ApprovalRemarks = leave.ApprovalRemarks,
            CreatedAt = leave.CreatedAt
        };

        return Ok(response);
    }

    /// <summary>
    /// Create leave request
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<LeaveResponse>> CreateLeave(CreateLeaveRequest request)
    {
        // Verify employee exists
        if (!await _context.Employees.AnyAsync(e => e.EmployeeId == request.EmployeeId))
        {
            return BadRequest(new { message = "Employee not found" });
        }

        // Verify leave type exists
        if (!await _context.Set<LeaveType>().AnyAsync(lt => lt.LeaveTypeId == request.LeaveTypeId))
        {
            return BadRequest(new { message = "Leave type not found" });
        }

        var leave = new EmployeeLeave
        {
            EmployeeId = request.EmployeeId,
            LeaveTypeId = request.LeaveTypeId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            NumberOfDays = request.NumberOfDays,
            Reason = request.Reason,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.Set<EmployeeLeave>().Add(leave);

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created leave request for employee {EmployeeId}", request.EmployeeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating leave request");
            return StatusCode(500, new { message = "Error creating leave request", error = ex.Message });
        }

        // Load navigation properties
        await _context.Entry(leave).Reference(l => l.Employee).LoadAsync();
        await _context.Entry(leave).Reference(l => l.LeaveType).LoadAsync();

        var response = new LeaveResponse
        {
            LeaveId = leave.LeaveId,
            EmployeeId = leave.EmployeeId,
            EmployeeCode = leave.Employee.EmployeeCode,
            EmployeeName = $"{leave.Employee.FirstName} {leave.Employee.LastName}",
            LeaveTypeId = leave.LeaveTypeId,
            LeaveTypeName = leave.LeaveType.LeaveTypeName,
            StartDate = leave.StartDate,
            EndDate = leave.EndDate,
            NumberOfDays = leave.NumberOfDays,
            Status = leave.Status,
            Reason = leave.Reason,
            ApprovedBy = leave.ApprovedBy,
            ApprovedDate = leave.ApprovedDate,
            ApprovalRemarks = leave.ApprovalRemarks,
            CreatedAt = leave.CreatedAt
        };

        return CreatedAtAction(nameof(GetLeave), new { id = leave.LeaveId }, response);
    }

    /// <summary>
    /// Approve or reject leave request
    /// </summary>
    [HttpPut("{id}/approve")]
    public async Task<ActionResult<LeaveResponse>> ApproveLeave(int id, ApproveLeaveRequest request)
    {
        var leave = await _context.Set<EmployeeLeave>()
            .Include(l => l.Employee)
            .Include(l => l.LeaveType)
            .FirstOrDefaultAsync(l => l.LeaveId == id);

        if (leave == null)
        {
            return NotFound(new { message = $"Leave request with ID {id} not found" });
        }

        if (leave.Status != "Pending")
        {
            return BadRequest(new { message = "Only pending leave requests can be approved/rejected" });
        }

        leave.Status = request.Status;
        leave.ApprovalRemarks = request.ApprovalRemarks;
        leave.ApprovedDate = DateTime.UtcNow;
        // TODO: Set ApprovedBy from authenticated user
        leave.UpdatedAt = DateTime.UtcNow;
        leave.UpdatedBy = null;

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("{Status} leave request {LeaveId}", request.Status, id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving leave request {LeaveId}", id);
            return StatusCode(500, new { message = "Error processing leave request", error = ex.Message });
        }

        var response = new LeaveResponse
        {
            LeaveId = leave.LeaveId,
            EmployeeId = leave.EmployeeId,
            EmployeeCode = leave.Employee.EmployeeCode,
            EmployeeName = $"{leave.Employee.FirstName} {leave.Employee.LastName}",
            LeaveTypeId = leave.LeaveTypeId,
            LeaveTypeName = leave.LeaveType.LeaveTypeName,
            StartDate = leave.StartDate,
            EndDate = leave.EndDate,
            NumberOfDays = leave.NumberOfDays,
            Status = leave.Status,
            Reason = leave.Reason,
            ApprovedBy = leave.ApprovedBy,
            ApprovedDate = leave.ApprovedDate,
            ApprovalRemarks = leave.ApprovalRemarks,
            CreatedAt = leave.CreatedAt
        };

        return Ok(response);
    }
}
