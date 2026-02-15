using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payroll.Core.Entities;
using Payroll.Infrastructure.Data;

namespace Payroll.API.Controllers;

[ApiController]
[Route("api/government")]
[Authorize(Roles = "Admin,HR")]
public class GovernmentContributionsController : ControllerBase
{
    private readonly PayrollDbContext _context;
    private readonly ILogger<GovernmentContributionsController> _logger;

    public GovernmentContributionsController(PayrollDbContext context, ILogger<GovernmentContributionsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // ───────────────────── SSS ─────────────────────

    [HttpGet("sss")]
    public async Task<ActionResult<List<SssContributionTable>>> GetSssTable()
    {
        var data = await _context.Set<SssContributionTable>()
            .OrderBy(t => t.MinSalary)
            .ToListAsync();
        return Ok(data);
    }

    [HttpPost("sss")]
    public async Task<ActionResult<SssContributionTable>> CreateSss(SssContributionTable entry)
    {
        entry.SssTableId = 0;
        _context.Set<SssContributionTable>().Add(entry);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created SSS bracket {Min}-{Max}", entry.MinSalary, entry.MaxSalary);
        return CreatedAtAction(nameof(GetSssTable), entry);
    }

    [HttpPut("sss/{id}")]
    public async Task<ActionResult> UpdateSss(int id, SssContributionTable entry)
    {
        var existing = await _context.Set<SssContributionTable>().FindAsync(id);
        if (existing == null) return NotFound(new { message = "SSS entry not found" });

        existing.MinSalary = entry.MinSalary;
        existing.MaxSalary = entry.MaxSalary;
        existing.EmployeeContribution = entry.EmployeeContribution;
        existing.EmployerContribution = entry.EmployerContribution;
        existing.TotalContribution = entry.TotalContribution;
        existing.EcContribution = entry.EcContribution;
        existing.EffectiveDate = entry.EffectiveDate;
        existing.EndDate = entry.EndDate;
        existing.IsActive = entry.IsActive;

        await _context.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpDelete("sss/{id}")]
    public async Task<ActionResult> DeleteSss(int id)
    {
        var existing = await _context.Set<SssContributionTable>().FindAsync(id);
        if (existing == null) return NotFound(new { message = "SSS entry not found" });
        _context.Set<SssContributionTable>().Remove(existing);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Deleted" });
    }

    // ───────────────────── PhilHealth ─────────────────────

    [HttpGet("philhealth")]
    public async Task<ActionResult<List<PhilhealthContributionTable>>> GetPhilhealthTable()
    {
        var data = await _context.Set<PhilhealthContributionTable>()
            .OrderBy(t => t.MinSalary)
            .ToListAsync();
        return Ok(data);
    }

    [HttpPost("philhealth")]
    public async Task<ActionResult<PhilhealthContributionTable>> CreatePhilhealth(PhilhealthContributionTable entry)
    {
        entry.PhilhealthTableId = 0;
        _context.Set<PhilhealthContributionTable>().Add(entry);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created PhilHealth entry {Min}-{Max}", entry.MinSalary, entry.MaxSalary);
        return CreatedAtAction(nameof(GetPhilhealthTable), entry);
    }

    [HttpPut("philhealth/{id}")]
    public async Task<ActionResult> UpdatePhilhealth(int id, PhilhealthContributionTable entry)
    {
        var existing = await _context.Set<PhilhealthContributionTable>().FindAsync(id);
        if (existing == null) return NotFound(new { message = "PhilHealth entry not found" });

        existing.MinSalary = entry.MinSalary;
        existing.MaxSalary = entry.MaxSalary;
        existing.PremiumRate = entry.PremiumRate;
        existing.EmployeeShare = entry.EmployeeShare;
        existing.EmployerShare = entry.EmployerShare;
        existing.EffectiveDate = entry.EffectiveDate;
        existing.EndDate = entry.EndDate;
        existing.IsActive = entry.IsActive;

        await _context.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpDelete("philhealth/{id}")]
    public async Task<ActionResult> DeletePhilhealth(int id)
    {
        var existing = await _context.Set<PhilhealthContributionTable>().FindAsync(id);
        if (existing == null) return NotFound(new { message = "PhilHealth entry not found" });
        _context.Set<PhilhealthContributionTable>().Remove(existing);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Deleted" });
    }

    // ───────────────────── Pag-IBIG ─────────────────────

    [HttpGet("pagibig")]
    public async Task<ActionResult<List<PagibigContributionTable>>> GetPagibigTable()
    {
        var data = await _context.Set<PagibigContributionTable>()
            .OrderBy(t => t.MinSalary)
            .ToListAsync();
        return Ok(data);
    }

    [HttpPost("pagibig")]
    public async Task<ActionResult<PagibigContributionTable>> CreatePagibig(PagibigContributionTable entry)
    {
        entry.PagibigTableId = 0;
        _context.Set<PagibigContributionTable>().Add(entry);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created Pag-IBIG entry {Min}-{Max}", entry.MinSalary, entry.MaxSalary);
        return CreatedAtAction(nameof(GetPagibigTable), entry);
    }

    [HttpPut("pagibig/{id}")]
    public async Task<ActionResult> UpdatePagibig(int id, PagibigContributionTable entry)
    {
        var existing = await _context.Set<PagibigContributionTable>().FindAsync(id);
        if (existing == null) return NotFound(new { message = "Pag-IBIG entry not found" });

        existing.MinSalary = entry.MinSalary;
        existing.MaxSalary = entry.MaxSalary;
        existing.EmployeeRate = entry.EmployeeRate;
        existing.EmployerRate = entry.EmployerRate;
        existing.MaxEmployeeContribution = entry.MaxEmployeeContribution;
        existing.MaxEmployerContribution = entry.MaxEmployerContribution;
        existing.EffectiveDate = entry.EffectiveDate;
        existing.EndDate = entry.EndDate;
        existing.IsActive = entry.IsActive;

        await _context.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpDelete("pagibig/{id}")]
    public async Task<ActionResult> DeletePagibig(int id)
    {
        var existing = await _context.Set<PagibigContributionTable>().FindAsync(id);
        if (existing == null) return NotFound(new { message = "Pag-IBIG entry not found" });
        _context.Set<PagibigContributionTable>().Remove(existing);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Deleted" });
    }

    // ───────────────────── Tax ─────────────────────

    [HttpGet("tax")]
    public async Task<ActionResult<List<TaxTable>>> GetTaxTable()
    {
        var data = await _context.Set<TaxTable>()
            .OrderBy(t => t.MinCompensation)
            .ToListAsync();
        return Ok(data);
    }

    [HttpPost("tax")]
    public async Task<ActionResult<TaxTable>> CreateTax(TaxTable entry)
    {
        entry.TaxTableId = 0;
        _context.Set<TaxTable>().Add(entry);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created Tax bracket {Min}-{Max}", entry.MinCompensation, entry.MaxCompensation);
        return CreatedAtAction(nameof(GetTaxTable), entry);
    }

    [HttpPut("tax/{id}")]
    public async Task<ActionResult> UpdateTax(int id, TaxTable entry)
    {
        var existing = await _context.Set<TaxTable>().FindAsync(id);
        if (existing == null) return NotFound(new { message = "Tax entry not found" });

        existing.MinCompensation = entry.MinCompensation;
        existing.MaxCompensation = entry.MaxCompensation;
        existing.BaseTax = entry.BaseTax;
        existing.TaxRate = entry.TaxRate;
        existing.ExcessOver = entry.ExcessOver;
        existing.PeriodType = entry.PeriodType;
        existing.EffectiveDate = entry.EffectiveDate;
        existing.EndDate = entry.EndDate;
        existing.IsActive = entry.IsActive;

        await _context.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpDelete("tax/{id}")]
    public async Task<ActionResult> DeleteTax(int id)
    {
        var existing = await _context.Set<TaxTable>().FindAsync(id);
        if (existing == null) return NotFound(new { message = "Tax entry not found" });
        _context.Set<TaxTable>().Remove(existing);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Deleted" });
    }
}
