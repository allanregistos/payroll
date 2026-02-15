namespace Payroll.Core.Entities;

public class LoanType
{
    public int LoanTypeId { get; set; }
    public string LoanCode { get; set; } = string.Empty;
    public string LoanName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<EmployeeLoan> EmployeeLoans { get; set; } = new List<EmployeeLoan>();
}

public class EmployeeLoan
{
    public int LoanId { get; set; }
    public Guid? EmployeeId { get; set; }
    public int? LoanTypeId { get; set; }
    public decimal LoanAmount { get; set; }
    public decimal InterestRate { get; set; }
    public int NumberOfInstallments { get; set; }
    public decimal MonthlyAmortization { get; set; }
    public DateOnly LoanDate { get; set; }
    public DateOnly FirstDeductionDate { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal? Balance { get; set; }
    public string Status { get; set; } = "Active";
    public string? Remarks { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }

    public Employee? Employee { get; set; }
    public LoanType? LoanType { get; set; }
    public ICollection<LoanPayment> Payments { get; set; } = new List<LoanPayment>();
}

public class LoanPayment
{
    public int PaymentId { get; set; }
    public int? LoanId { get; set; }
    public Guid? PayrollHeaderId { get; set; }
    public DateOnly PaymentDate { get; set; }
    public decimal PrincipalAmount { get; set; }
    public decimal InterestAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? BalanceAfterPayment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public EmployeeLoan? Loan { get; set; }
}
