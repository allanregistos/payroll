using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payroll.API.DTOs;
using Payroll.Application.Services;
using Payroll.Core.Entities;
using Payroll.Infrastructure.Data;

namespace Payroll.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,HR")]
public class PayrollController : ControllerBase
{
    private readonly PayrollDbContext _context;
    private readonly IPayrollComputationService _payrollService;
    private readonly ILogger<PayrollController> _logger;

    public PayrollController(
        PayrollDbContext context,
        IPayrollComputationService payrollService,
        ILogger<PayrollController> logger)
    {
        _context = context;
        _payrollService = payrollService;
        _logger = logger;
    }

    /// <summary>
    /// Get all payroll records
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ComputePayrollResponse>>> GetAllPayroll()
    {
        var payrolls = await _context.Set<PayrollHeader>()
            .Include(p => p.Employee)
            .Include(p => p.PayrollPeriod)
            .Include(p => p.Earnings)
            .Include(p => p.Deductions)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ComputePayrollResponse
            {
                PayrollId = p.PayrollHeaderId,
                EmployeeId = p.EmployeeId,
                EmployeeName = p.Employee.FirstName + " " + p.Employee.LastName,
                EmployeeNumber = p.Employee.EmployeeCode,
                PayPeriodStart = p.PayrollPeriod.StartDate,
                PayPeriodEnd = p.PayrollPeriod.EndDate,
                BasicSalary = p.BasicSalary,
                GrossPay = p.GrossPay,
                TotalDeductions = p.TotalDeductions,
                TotalAllowances = p.Earnings.Where(e => e.EarningType == "Allowances").Sum(e => e.Amount),
                SssContribution = p.Deductions.Where(d => d.DeductionType == "SSS Contribution").Sum(d => d.Amount),
                PhilhealthContribution = p.Deductions.Where(d => d.DeductionType == "PhilHealth Contribution").Sum(d => d.Amount),
                PagibigContribution = p.Deductions.Where(d => d.DeductionType == "Pag-IBIG Contribution").Sum(d => d.Amount),
                WithholdingTax = p.Deductions.Where(d => d.DeductionType == "Withholding Tax").Sum(d => d.Amount),
                OvertimePay = p.Earnings.Where(e => e.EarningType == "Overtime Pay").Sum(e => e.Amount),
                NetPay = p.NetPay,
                PayrollStatus = p.Status
            })
            .ToListAsync();

        return Ok(payrolls);
    }

    /// <summary>
    /// Get all payroll periods
    /// </summary>
    [HttpGet("periods")]
    public async Task<ActionResult<IEnumerable<PayrollPeriodResponse>>> GetPayrollPeriods()
    {
        var periods = await _context.Set<PayrollPeriod>()
            .OrderByDescending(p => p.StartDate)
            .Select(p => new PayrollPeriodResponse
            {
                PayrollPeriodId = p.PayrollPeriodId,
                PeriodName = p.PeriodName,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                PayDate = p.PayDate,
                PeriodType = p.PeriodType,
                Status = p.Status,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        return Ok(periods);
    }

    /// <summary>
    /// Get payroll period by ID
    /// </summary>
    [HttpGet("periods/{id}")]
    public async Task<ActionResult<PayrollPeriodResponse>> GetPayrollPeriod(int id)
    {
        var period = await _context.Set<PayrollPeriod>()
            .FirstOrDefaultAsync(p => p.PayrollPeriodId == id);

        if (period == null)
        {
            return NotFound(new { message = $"Payroll period with ID {id} not found" });
        }

        var response = new PayrollPeriodResponse
        {
            PayrollPeriodId = period.PayrollPeriodId,
            PeriodName = period.PeriodName,
            StartDate = period.StartDate,
            EndDate = period.EndDate,
            PayDate = period.PayDate,
            PeriodType = period.PeriodType,
            Status = period.Status,
            CreatedAt = period.CreatedAt
        };

        return Ok(response);
    }

    /// <summary>
    /// Create new payroll period
    /// </summary>
    [HttpPost("periods")]
    public async Task<ActionResult<PayrollPeriodResponse>> CreatePayrollPeriod(CreatePayrollPeriodRequest request)
    {
        // Check for overlapping periods
        var overlapping = await _context.Set<PayrollPeriod>()
            .AnyAsync(p => p.StartDate <= request.EndDate && p.EndDate >= request.StartDate);

        if (overlapping)
        {
            return BadRequest(new { message = "Payroll period overlaps with an existing period" });
        }

        var period = new PayrollPeriod
        {
            PeriodName = request.PeriodName,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            PayDate = request.PayDate,
            PeriodType = request.PeriodType,
            Status = "Draft",
            CreatedAt = DateTime.UtcNow
        };

        _context.Set<PayrollPeriod>().Add(period);

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created payroll period {PeriodName}", request.PeriodName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payroll period");
            return StatusCode(500, new { message = "Error creating payroll period", error = ex.Message });
        }

        var response = new PayrollPeriodResponse
        {
            PayrollPeriodId = period.PayrollPeriodId,
            PeriodName = period.PeriodName,
            StartDate = period.StartDate,
            EndDate = period.EndDate,
            PayDate = period.PayDate,
            PeriodType = period.PeriodType,
            Status = period.Status,
            CreatedAt = period.CreatedAt
        };

        return CreatedAtAction(nameof(GetPayrollPeriod), new { id = period.PayrollPeriodId }, response);
    }

    /// <summary>
    /// Update payroll period status
    /// </summary>
    [HttpPut("periods/{id}/status")]
    public async Task<ActionResult> UpdatePeriodStatus(int id, [FromBody] UpdatePeriodStatusRequest request)
    {
        var period = await _context.Set<PayrollPeriod>().FindAsync(id);
        if (period == null)
            return NotFound(new { message = $"Payroll period with ID {id} not found" });

        var allowedTransitions = new Dictionary<string, string[]>
        {
            { "Draft", new[] { "Processing", "Cancelled" } },
            { "Processing", new[] { "Completed", "Cancelled" } },
            { "Completed", new[] { "Posted", "Cancelled" } },
            { "Posted", new string[] { } }
        };

        if (allowedTransitions.ContainsKey(period.Status) && 
            !allowedTransitions[period.Status].Contains(request.Status))
        {
            return BadRequest(new { message = $"Cannot transition from '{period.Status}' to '{request.Status}'" });
        }

        period.Status = request.Status;
        if (request.Status == "Processing")
        {
            period.ProcessedAt = DateTime.UtcNow;
        }

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated period {PeriodId} status to {Status}", id, request.Status);
            return Ok(new { message = $"Period status updated to {request.Status}" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating period status");
            return StatusCode(500, new { message = "Error updating period status", error = ex.Message });
        }
    }

    /// <summary>
    /// Compute payroll for a single employee for a given date range
    /// </summary>
    [HttpPost("compute")]
    public async Task<ActionResult<ComputePayrollResponse>> ComputePayroll(ComputePayrollRequest request)
    {
        if (request.EmployeeId == Guid.Empty)
        {
            return BadRequest(new { message = "Employee ID is required" });
        }

        if (request.PayPeriodStart >= request.PayPeriodEnd)
        {
            return BadRequest(new { message = "Pay period start must be before pay period end" });
        }

        try
        {
            // Find or create a payroll period matching the date range
            var period = await _context.Set<PayrollPeriod>()
                .FirstOrDefaultAsync(p => p.StartDate == request.PayPeriodStart && p.EndDate == request.PayPeriodEnd);

            if (period == null)
            {
                period = new PayrollPeriod
                {
                    PeriodName = $"{request.PayPeriodStart:MMM dd} - {request.PayPeriodEnd:MMM dd, yyyy}",
                    StartDate = request.PayPeriodStart,
                    EndDate = request.PayPeriodEnd,
                    PayDate = request.PayPeriodEnd,
                    PeriodType = "Semi-Monthly",
                    Status = "Draft",
                    CreatedAt = DateTime.UtcNow
                };
                _context.Set<PayrollPeriod>().Add(period);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Auto-created payroll period {PeriodName} (ID: {PeriodId})", period.PeriodName, period.PayrollPeriodId);
            }

            // Compute payroll for the employee
            // Check for existing payroll for this employee + period
            var existingPayroll = await _context.Set<PayrollHeader>()
                .AnyAsync(ph => ph.EmployeeId == request.EmployeeId && ph.PayrollPeriodId == period.PayrollPeriodId);

            if (existingPayroll)
            {
                return Conflict(new { message = $"Payroll has already been computed for this employee for the period '{period.PeriodName}'. Please delete the existing record first." });
            }

            var result = await _payrollService.ComputePayrollAsync(
                request.EmployeeId, period.PayrollPeriodId, DateTime.UtcNow);

            // Save payroll header
            var payrollHeader = new PayrollHeader
            {
                PayrollHeaderId = Guid.NewGuid(),
                EmployeeId = result.EmployeeId,
                PayrollPeriodId = period.PayrollPeriodId,
                BasicSalary = result.BasicSalary,
                GrossPay = result.GrossPay,
                TotalDeductions = result.TotalDeductions,
                NetPay = result.NetPay,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };
            _context.Set<PayrollHeader>().Add(payrollHeader);

            // Save earnings
            if (result.BasicSalary > 0)
                _context.Set<PayrollEarning>().Add(new PayrollEarning { PayrollHeaderId = payrollHeader.PayrollHeaderId, EarningType = "Basic Salary", Amount = result.BasicSalary });
            if (result.Allowances > 0)
                _context.Set<PayrollEarning>().Add(new PayrollEarning { PayrollHeaderId = payrollHeader.PayrollHeaderId, EarningType = "Allowances", Amount = result.Allowances });
            if (result.OvertimePay > 0)
                _context.Set<PayrollEarning>().Add(new PayrollEarning { PayrollHeaderId = payrollHeader.PayrollHeaderId, EarningType = "Overtime Pay", Amount = result.OvertimePay });

            // Save deductions
            if (result.SssEmployeeContribution > 0)
                _context.Set<PayrollDeduction>().Add(new PayrollDeduction { PayrollHeaderId = payrollHeader.PayrollHeaderId, DeductionType = "SSS Contribution", Amount = result.SssEmployeeContribution });
            if (result.PhilhealthEmployeeContribution > 0)
                _context.Set<PayrollDeduction>().Add(new PayrollDeduction { PayrollHeaderId = payrollHeader.PayrollHeaderId, DeductionType = "PhilHealth Contribution", Amount = result.PhilhealthEmployeeContribution });
            if (result.PagibigEmployeeContribution > 0)
                _context.Set<PayrollDeduction>().Add(new PayrollDeduction { PayrollHeaderId = payrollHeader.PayrollHeaderId, DeductionType = "Pag-IBIG Contribution", Amount = result.PagibigEmployeeContribution });
            if (result.WithholdingTax > 0)
                _context.Set<PayrollDeduction>().Add(new PayrollDeduction { PayrollHeaderId = payrollHeader.PayrollHeaderId, DeductionType = "Withholding Tax", Amount = result.WithholdingTax });

            await _context.SaveChangesAsync();

            _logger.LogInformation("Computed payroll for employee {EmployeeId} in period {PeriodId}", request.EmployeeId, period.PayrollPeriodId);

            return Ok(new ComputePayrollResponse
            {
                PayrollId = payrollHeader.PayrollHeaderId,
                EmployeeId = result.EmployeeId,
                PayPeriodStart = request.PayPeriodStart,
                PayPeriodEnd = request.PayPeriodEnd,
                BasicSalary = result.BasicSalary,
                TotalDeductions = result.TotalDeductions,
                TotalAllowances = result.Allowances,
                SssContribution = result.SssEmployeeContribution,
                PhilhealthContribution = result.PhilhealthEmployeeContribution,
                PagibigContribution = result.PagibigEmployeeContribution,
                WithholdingTax = result.WithholdingTax,
                OvertimePay = result.OvertimePay,
                GrossPay = result.GrossPay,
                NetPay = result.NetPay,
                PayrollStatus = "Pending",
                EmployeeName = result.EmployeeName,
                EmployeeNumber = result.EmployeeCode
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error computing payroll for employee {EmployeeId}", request.EmployeeId);
            return StatusCode(500, new { message = "Error computing payroll", error = ex.Message });
        }
    }

    /// <summary>
    /// Process payroll for a period
    /// </summary>
    [HttpPost("process")]
    public async Task<ActionResult<List<PayrollResponse>>> ProcessPayroll(ProcessPayrollRequest request)
    {
        var period = await _context.Set<PayrollPeriod>()
            .FirstOrDefaultAsync(p => p.PayrollPeriodId == request.PayrollPeriodId);

        if (period == null)
        {
            return NotFound(new { message = $"Payroll period with ID {request.PayrollPeriodId} not found" });
        }

        if (period.Status == "Closed")
        {
            return BadRequest(new { message = "Cannot process payroll for a closed period" });
        }

        var effectiveDate = request.EffectiveDate ?? DateTime.UtcNow;

        try
        {
            // Compute payroll for all employees
            var computationResults = await _payrollService.ComputePayrollForPeriodAsync(
                request.PayrollPeriodId, effectiveDate);

            var payrollResponses = new List<PayrollResponse>();

            // Save payroll records to database
            foreach (var result in computationResults)
            {
                // Create payroll header
                var payrollHeader = new PayrollHeader
                {
                    PayrollHeaderId = Guid.NewGuid(),
                    EmployeeId = result.EmployeeId,
                    PayrollPeriodId = request.PayrollPeriodId,
                    BasicSalary = result.BasicSalary,
                    GrossPay = result.GrossPay,
                    TotalDeductions = result.TotalDeductions,
                    NetPay = result.NetPay,
                    Status = "Draft",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Set<PayrollHeader>().Add(payrollHeader);

                // Create earnings
                if (result.BasicSalary > 0)
                {
                    _context.Set<PayrollEarning>().Add(new PayrollEarning
                    {
                        PayrollHeaderId = payrollHeader.PayrollHeaderId,
                        EarningType = "Basic Salary",
                        Amount = result.BasicSalary
                    });
                }

                if (result.Allowances > 0)
                {
                    _context.Set<PayrollEarning>().Add(new PayrollEarning
                    {
                        PayrollHeaderId = payrollHeader.PayrollHeaderId,
                        EarningType = "Allowances",
                        Amount = result.Allowances
                    });
                }

                if (result.OvertimePay > 0)
                {
                    _context.Set<PayrollEarning>().Add(new PayrollEarning
                    {
                        PayrollHeaderId = payrollHeader.PayrollHeaderId,
                        EarningType = "Overtime Pay",
                        Amount = result.OvertimePay
                    });
                }

                if (result.HolidayPay > 0)
                {
                    _context.Set<PayrollEarning>().Add(new PayrollEarning
                    {
                        PayrollHeaderId = payrollHeader.PayrollHeaderId,
                        EarningType = "Holiday Pay",
                        Amount = result.HolidayPay
                    });
                }

                // Create deductions
                if (result.SssEmployeeContribution > 0)
                {
                    _context.Set<PayrollDeduction>().Add(new PayrollDeduction
                    {
                        PayrollHeaderId = payrollHeader.PayrollHeaderId,
                        DeductionType = "SSS Contribution",
                        Amount = result.SssEmployeeContribution
                    });
                }

                if (result.PhilhealthEmployeeContribution > 0)
                {
                    _context.Set<PayrollDeduction>().Add(new PayrollDeduction
                    {
                        PayrollHeaderId = payrollHeader.PayrollHeaderId,
                        DeductionType = "PhilHealth Contribution",
                        Amount = result.PhilhealthEmployeeContribution
                    });
                }

                if (result.PagibigEmployeeContribution > 0)
                {
                    _context.Set<PayrollDeduction>().Add(new PayrollDeduction
                    {
                        PayrollHeaderId = payrollHeader.PayrollHeaderId,
                        DeductionType = "Pag-IBIG Contribution",
                        Amount = result.PagibigEmployeeContribution
                    });
                }

                if (result.WithholdingTax > 0)
                {
                    _context.Set<PayrollDeduction>().Add(new PayrollDeduction
                    {
                        PayrollHeaderId = payrollHeader.PayrollHeaderId,
                        DeductionType = "Withholding Tax",
                        Amount = result.WithholdingTax
                    });
                }

                payrollResponses.Add(new PayrollResponse
                {
                    PayrollHeaderId = payrollHeader.PayrollHeaderId,
                    EmployeeId = result.EmployeeId,
                    EmployeeCode = result.EmployeeCode,
                    EmployeeName = result.EmployeeName,
                    PayrollPeriodId = request.PayrollPeriodId,
                    PeriodName = period.PeriodName,
                    BasicSalary = result.BasicSalary,
                    Allowances = result.Allowances,
                    OvertimePay = result.OvertimePay,
                    HolidayPay = result.HolidayPay,
                    OtherEarnings = result.OtherEarnings,
                    GrossPay = result.GrossPay,
                    SssContribution = result.SssEmployeeContribution,
                    PhilhealthContribution = result.PhilhealthEmployeeContribution,
                    PagibigContribution = result.PagibigEmployeeContribution,
                    WithholdingTax = result.WithholdingTax,
                    LoanDeductions = result.LoanDeductions,
                    OtherDeductions = result.OtherDeductions,
                    TotalDeductions = result.TotalDeductions,
                    NetPay = result.NetPay,
                    Status = "Draft",
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Processed payroll for period {PeriodId} with {Count} employees",
                request.PayrollPeriodId, computationResults.Count);

            return Ok(payrollResponses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payroll for period {PeriodId}", request.PayrollPeriodId);
            return StatusCode(500, new { message = "Error processing payroll", error = ex.Message });
        }
    }

    /// <summary>
    /// Get payroll records for a period
    /// </summary>
    [HttpGet("period/{periodId}")]
    public async Task<ActionResult<List<PayrollResponse>>> GetPayrollByPeriod(int periodId)
    {
        var period = await _context.Set<PayrollPeriod>()
            .FirstOrDefaultAsync(p => p.PayrollPeriodId == periodId);

        if (period == null)
        {
            return NotFound(new { message = $"Payroll period with ID {periodId} not found" });
        }

        var payrolls = await _context.Set<PayrollHeader>()
            .Include(p => p.Employee)
            .Where(p => p.PayrollPeriodId == periodId)
            .OrderBy(p => p.Employee.EmployeeCode)
            .Select(p => new PayrollResponse
            {
                PayrollHeaderId = p.PayrollHeaderId,
                EmployeeId = p.EmployeeId,
                EmployeeCode = p.Employee.EmployeeCode,
                EmployeeName = p.Employee.FirstName + " " + p.Employee.LastName,
                PayrollPeriodId = periodId,
                PeriodName = period.PeriodName,
                BasicSalary = p.BasicSalary,
                GrossPay = p.GrossPay,
                TotalDeductions = p.TotalDeductions,
                NetPay = p.NetPay,
                Status = p.Status,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        return Ok(payrolls);
    }

    /// <summary>
    /// Get payroll details by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PayrollResponse>> GetPayroll(Guid id)
    {
        var payroll = await _context.Set<PayrollHeader>()
            .Include(p => p.Employee)
            .Include(p => p.PayrollPeriod)
            .Include(p => p.Earnings)
            .Include(p => p.Deductions)
            .FirstOrDefaultAsync(p => p.PayrollHeaderId == id);

        if (payroll == null)
        {
            return NotFound(new { message = $"Payroll record with ID {id} not found" });
        }

        var response = new PayrollResponse
        {
            PayrollHeaderId = payroll.PayrollHeaderId,
            EmployeeId = payroll.EmployeeId,
            EmployeeCode = payroll.Employee.EmployeeCode,
            EmployeeName = $"{payroll.Employee.FirstName} {payroll.Employee.LastName}",
            PayrollPeriodId = payroll.PayrollPeriodId,
            PeriodName = payroll.PayrollPeriod.PeriodName,
            BasicSalary = payroll.Earnings.FirstOrDefault(e => e.EarningType == "Basic Salary")?.Amount ?? 0,
            Allowances = payroll.Earnings.FirstOrDefault(e => e.EarningType == "Allowances")?.Amount ?? 0,
            OvertimePay = payroll.Earnings.FirstOrDefault(e => e.EarningType == "Overtime Pay")?.Amount ?? 0,
            HolidayPay = payroll.Earnings.FirstOrDefault(e => e.EarningType == "Holiday Pay")?.Amount ?? 0,
            OtherEarnings = payroll.Earnings.Where(e => !new[] { "Basic Salary", "Allowances", "Overtime Pay", "Holiday Pay" }.Contains(e.EarningType)).Sum(e => e.Amount),
            GrossPay = payroll.GrossPay,
            SssContribution = payroll.Deductions.FirstOrDefault(d => d.DeductionType == "SSS Contribution")?.Amount ?? 0,
            PhilhealthContribution = payroll.Deductions.FirstOrDefault(d => d.DeductionType == "PhilHealth Contribution")?.Amount ?? 0,
            PagibigContribution = payroll.Deductions.FirstOrDefault(d => d.DeductionType == "Pag-IBIG Contribution")?.Amount ?? 0,
            WithholdingTax = payroll.Deductions.FirstOrDefault(d => d.DeductionType == "Withholding Tax")?.Amount ?? 0,
            LoanDeductions = payroll.Deductions.Where(d => d.DeductionType.Contains("Loan")).Sum(d => d.Amount),
            OtherDeductions = payroll.Deductions.Where(d => !new[] { "SSS Contribution", "PhilHealth Contribution", "Pag-IBIG Contribution", "Withholding Tax" }.Contains(d.DeductionType) && !d.DeductionType.Contains("Loan")).Sum(d => d.Amount),
            TotalDeductions = payroll.TotalDeductions,
            NetPay = payroll.NetPay,
            Status = payroll.Status,
            CreatedAt = payroll.CreatedAt
        };

        return Ok(response);
    }

    /// <summary>
    /// Get payroll summary for a period
    /// </summary>
    [HttpGet("period/{periodId}/summary")]
    public async Task<ActionResult<PayrollSummaryResponse>> GetPayrollSummary(int periodId)
    {
        var period = await _context.Set<PayrollPeriod>()
            .FirstOrDefaultAsync(p => p.PayrollPeriodId == periodId);

        if (period == null)
        {
            return NotFound(new { message = $"Payroll period with ID {periodId} not found" });
        }

        var payrolls = await _context.Set<PayrollHeader>()
            .Include(p => p.Deductions)
            .Where(p => p.PayrollPeriodId == periodId)
            .ToListAsync();

        var summary = new PayrollSummaryResponse
        {
            PayrollPeriodId = periodId,
            PeriodName = period.PeriodName,
            StartDate = period.StartDate,
            EndDate = period.EndDate,
            PayDate = period.PayDate,
            TotalEmployees = payrolls.Count,
            TotalGrossPay = payrolls.Sum(p => p.GrossPay),
            TotalDeductions = payrolls.Sum(p => p.TotalDeductions),
            TotalNetPay = payrolls.Sum(p => p.NetPay),
            TotalSssEmployee = payrolls.SelectMany(p => p.Deductions).Where(d => d.DeductionType == "SSS Contribution").Sum(d => d.Amount),
            TotalPhilhealthEmployee = payrolls.SelectMany(p => p.Deductions).Where(d => d.DeductionType == "PhilHealth Contribution").Sum(d => d.Amount),
            TotalPagibigEmployee = payrolls.SelectMany(p => p.Deductions).Where(d => d.DeductionType == "Pag-IBIG Contribution").Sum(d => d.Amount),
            TotalWithholdingTax = payrolls.SelectMany(p => p.Deductions).Where(d => d.DeductionType == "Withholding Tax").Sum(d => d.Amount)
        };

        // Note: Employer contributions need to be computed separately or stored
        // For now, using approximate calculations (SSS employer = employee contribution, etc.)
        summary.TotalSssEmployer = summary.TotalSssEmployee; // Simplified - actual calculation would be more complex
        summary.TotalPhilhealthEmployer = summary.TotalPhilhealthEmployee; // 50/50 split
        summary.TotalPagibigEmployer = summary.TotalPagibigEmployee; // Usually equal

        return Ok(summary);
    }
}
