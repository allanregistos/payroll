using System.Text.Json.Serialization;

namespace PayrollWeb.Models;

public class LoanTypeDto
{
    [JsonPropertyName("loanTypeId")]
    public int LoanTypeId { get; set; }
    [JsonPropertyName("loanCode")]
    public string LoanCode { get; set; } = string.Empty;
    [JsonPropertyName("loanName")]
    public string LoanName { get; set; } = string.Empty;
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; } = true;
}

public class CreateLoanTypeDto
{
    [JsonPropertyName("loanCode")]
    public string LoanCode { get; set; } = string.Empty;
    [JsonPropertyName("loanName")]
    public string LoanName { get; set; } = string.Empty;
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; } = true;
}

public class EmployeeLoanDto
{
    [JsonPropertyName("loanId")]
    public int LoanId { get; set; }
    [JsonPropertyName("employeeId")]
    public Guid EmployeeId { get; set; }
    [JsonPropertyName("employeeName")]
    public string EmployeeName { get; set; } = string.Empty;
    [JsonPropertyName("loanTypeId")]
    public int LoanTypeId { get; set; }
    [JsonPropertyName("loanTypeName")]
    public string LoanTypeName { get; set; } = string.Empty;
    [JsonPropertyName("loanAmount")]
    public decimal LoanAmount { get; set; }
    [JsonPropertyName("interestRate")]
    public decimal InterestRate { get; set; }
    [JsonPropertyName("numberOfInstallments")]
    public int NumberOfInstallments { get; set; }
    [JsonPropertyName("monthlyAmortization")]
    public decimal MonthlyAmortization { get; set; }
    [JsonPropertyName("loanDate")]
    public DateOnly LoanDate { get; set; }
    [JsonPropertyName("firstDeductionDate")]
    public DateOnly FirstDeductionDate { get; set; }
    [JsonPropertyName("totalPaid")]
    public decimal TotalPaid { get; set; }
    [JsonPropertyName("balance")]
    public decimal Balance { get; set; }
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    [JsonPropertyName("remarks")]
    public string? Remarks { get; set; }
}

public class CreateLoanDto
{
    [JsonPropertyName("employeeId")]
    public Guid EmployeeId { get; set; }
    [JsonPropertyName("loanTypeId")]
    public int LoanTypeId { get; set; }
    [JsonPropertyName("loanAmount")]
    public decimal LoanAmount { get; set; }
    [JsonPropertyName("interestRate")]
    public decimal InterestRate { get; set; }
    [JsonPropertyName("numberOfInstallments")]
    public int NumberOfInstallments { get; set; }
    [JsonPropertyName("loanDate")]
    public DateOnly LoanDate { get; set; }
    [JsonPropertyName("firstDeductionDate")]
    public DateOnly FirstDeductionDate { get; set; }
    [JsonPropertyName("remarks")]
    public string? Remarks { get; set; }
}

public class LoanPaymentDto
{
    [JsonPropertyName("paymentId")]
    public int PaymentId { get; set; }
    [JsonPropertyName("loanId")]
    public int LoanId { get; set; }
    [JsonPropertyName("paymentDate")]
    public DateOnly PaymentDate { get; set; }
    [JsonPropertyName("principalAmount")]
    public decimal PrincipalAmount { get; set; }
    [JsonPropertyName("interestAmount")]
    public decimal InterestAmount { get; set; }
    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; set; }
    [JsonPropertyName("balanceAfterPayment")]
    public decimal? BalanceAfterPayment { get; set; }
}
