using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payroll.Infrastructure.Data;

namespace Payroll.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PositionsController : ControllerBase
{
    private readonly PayrollDbContext _context;

    public PositionsController(PayrollDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PositionDto>>> GetPositions()
    {
        var positions = await _context.Positions
            .Where(p => p.IsActive)
            .OrderBy(p => p.PositionTitle)
            .Select(p => new PositionDto
            {
                PositionId = p.PositionId,
                PositionCode = p.PositionCode,
                PositionTitle = p.PositionTitle,
                Description = p.Description
            })
            .ToListAsync();

        return Ok(positions);
    }
}

public record PositionDto
{
    public int PositionId { get; set; }
    public string PositionCode { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public string? Description { get; set; }
}
