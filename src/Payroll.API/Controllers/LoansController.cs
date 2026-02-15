using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payroll.Core.Entities;
using Payroll.Infrastructure.Data;

namespace Payroll.API.Controllers;

[ApiController]
[Route("api/loans")]
[Authorize(Roles = "Admin,HR")]
public class LoansController : ControllerBase
{
    private readonly PayrollDbContext _context;
    private readonly ILogger<LoansController> _logger;

    public LoansController(PayrollDbContext context, ILogger<LoansController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // ───────────────────── Loan Types ─────────────────────

    [HttpGet("types")]
    public async Task<ActionResult<List<LoanType>>> GetLoanTypes()
    {
        var types = await _context.Set<LoanType>()
            .OrderBy(t => t.LoanName)
            .ToListAsync();
        return Ok(types);
    }

    [HttpPost("types")]
    public async Task<ActionResult<LoanType>> CreateLoanType(LoanTypeRequest request)
    {
        var loanType = new LoanType
        {
            LoanCode = request.LoanCode,
            LoanName = request.LoanName,
            Description = request.Description,
            IsActive = true
        };

        _context.Set<LoanType>().Add(loanType);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created loan type {LoanName}", request.LoanName);
        return CreatedAtAction(nameof(GetLoanTypes), loanType);
    }

    [HttpPut("types/{id}")]
    public async Task<ActionResult> UpdateLoanType(int id, LoanTypeRequest request)
    {
        var existing = await _context.Set<LoanType>().FindAsync(id);
        if (existing == null) return NotFound(new { message = "Loan type not found" });

        existing.LoanCode = request.LoanCode;
        existing.LoanName = request.LoanName;
        existing.Description = request.Description;
        existing.IsActive = request.IsActive;

        await _context.SaveChangesAsync();
        return Ok(existing);
    }

    // ───────────────────── Employee Loans ─────────────────────

    [HttpGet]
    public async Task<ActionResult<List<EmployeeLoanResponse>>> GetLoans([FromQuery] Guid? employeeId)
    {
        var query = _context.Set<EmployeeLoan>()
            .Include(l => l.Employee)
            .Include(l => l.LoanType)
            .AsQueryable();

        if (employeeId.HasValue)
            query = query.Where(l => l.EmployeeId == employeeId.Value);

        var loans = await query
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new EmployeeLoanResponse
            {
                LoanId = l.LoanId,
                EmployeeId = l.EmployeeId ?? Guid.Empty,
                EmployeeName = l.Employee != null ? l.Employee.FirstName + " " + l.Employee.LastName : "",
                LoanTypeId = l.LoanTypeId ?? 0,
                LoanTypeName = l.LoanType != null ? l.LoanType.LoanName : "",
                LoanAmount = l.LoanAmount,
                InterestRate = l.InterestRate,
                NumberOfInstallments = l.NumberOfInstallments,
                MonthlyAmortization = l.MonthlyAmortization,
                LoanDate = l.LoanDate,
                FirstDeductionDate = l.FirstDeductionDate,
                TotalPaid = l.TotalPaid,
                Balance = l.Balance ?? l.LoanAmount,
                Status = l.Status,
                Remarks = l.Remarks
            })
            .ToListAsync();

        return Ok(loans);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeLoanResponse>> CreateLoan(CreateLoanRequest request)
    {
        // Validate employee exists
        var employee = await _context.Set<Employee>().FindAsync(request.EmployeeId);
        if (employee == null) return NotFound(new { message = "Employee not found" });

        // Calculate loan details
        var totalWithInterest = request.LoanAmount * (1 + request.InterestRate);
        var monthlyAmortization = Math.Round(totalWithInterest / request.NumberOfInstallments, 2);

        var loan = new EmployeeLoan
        {
            EmployeeId = request.EmployeeId,
            LoanTypeId = request.LoanTypeId,
            LoanAmount = request.LoanAmount,
            InterestRate = request.InterestRate,
            NumberOfInstallments = request.NumberOfInstallments,
            MonthlyAmortization = monthlyAmortization,
            LoanDate = request.LoanDate,
            FirstDeductionDate = request.FirstDeductionDate,
            TotalPaid = 0,
            Balance = totalWithInterest,
            Status = "Active",
            Remarks = request.Remarks,
            CreatedAt = DateTime.UtcNow
        };

        _context.Set<EmployeeLoan>().Add(loan);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created loan {LoanId} for employee {EmployeeId}, amount {Amount}",
            loan.LoanId, request.EmployeeId, request.LoanAmount);

        return CreatedAtAction(nameof(GetLoans), new EmployeeLoanResponse
        {
            LoanId = loan.LoanId,
            EmployeeId = request.EmployeeId,
            EmployeeName = $"{employee.FirstName} {employee.LastName}",
            LoanTypeId = request.LoanTypeId,
            LoanAmount = loan.LoanAmount,
            InterestRate = loan.InterestRate,
            NumberOfInstallments = loan.NumberOfInstallments,
            MonthlyAmortization = loan.MonthlyAmortization,
            LoanDate = loan.LoanDate,
            FirstDeductionDate = loan.FirstDeductionDate,
            TotalPaid = loan.TotalPaid,
            Balance = loan.Balance ?? loan.LoanAmount,
            Status = loan.Status,
            Remarks = loan.Remarks
        });
    }

    [HttpPut("{id}/cancel")]
    public async Task<ActionResult> CancelLoan(int id)
    {
        var loan = await _context.Set<EmployeeLoan>().FindAsync(id);
        if (loan == null) return NotFound(new { message = "Loan not found" });
        if (loan.Status != "Active") return BadRequest(new { message = "Only active loans can be cancelled" });

        loan.Status = "Cancelled";
        await _context.SaveChangesAsync();
        return Ok(new { message = "Loan cancelled" });
    }

    // ───────────────────── Loan Payments ─────────────────────

    [HttpGet("{id}/payments")]
    public async Task<ActionResult<List<LoanPayment>>> GetLoanPayments(int id)
    {
        var payments = await _context.Set<LoanPayment>()
            .Where(p => p.LoanId == id)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
        return Ok(payments);
    }
}

// ───────────────────── Request/Response DTOs ─────────────────────

public class LoanTypeRequest
{
    public string LoanCode { get; set; } = string.Empty;
    public string LoanName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}

public class CreateLoanRequest
{
    public Guid EmployeeId { get; set; }
    public int LoanTypeId { get; set; }
    public decimal LoanAmount { get; set; }
    public decimal InterestRate { get; set; }
    public int NumberOfInstallments { get; set; }
    public DateOnly LoanDate { get; set; }
    public DateOnly FirstDeductionDate { get; set; }
    public string? Remarks { get; set; }
}

public class EmployeeLoanResponse
{
    public int LoanId { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int LoanTypeId { get; set; }
    public string LoanTypeName { get; set; } = string.Empty;
    public decimal LoanAmount { get; set; }
    public decimal InterestRate { get; set; }
    public int NumberOfInstallments { get; set; }
    public decimal MonthlyAmortization { get; set; }
    public DateOnly LoanDate { get; set; }
    public DateOnly FirstDeductionDate { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Balance { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Remarks { get; set; }
}
