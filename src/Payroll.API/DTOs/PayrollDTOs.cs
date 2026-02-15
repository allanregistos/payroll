namespace Payroll.API.DTOs;

/// <summary>
/// Data Transfer Objects for Payroll API
/// </summary>
/// 
public class CreatePayrollPeriodRequest
{
    public string PeriodName { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateOnly PayDate { get; set; }
    public string PeriodType { get; set; } = "Monthly"; // Monthly, Semi-Monthly, Weekly
}

public class PayrollPeriodResponse
{
    public int PayrollPeriodId { get; set; }
    public string PeriodName { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateOnly PayDate { get; set; }
    public string PeriodType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class ProcessPayrollRequest
{
    public int PayrollPeriodId { get; set; }
    public DateTime? EffectiveDate { get; set; } // For government contributions, defaults to current date
}

public class UpdatePeriodStatusRequest
{
    public string Status { get; set; } = string.Empty;
}

public class ComputePayrollRequest
{
    public Guid EmployeeId { get; set; }
    public DateOnly PayPeriodStart { get; set; }
    public DateOnly PayPeriodEnd { get; set; }
}

public class ComputePayrollResponse
{
    public Guid PayrollId { get; set; }
    public Guid EmployeeId { get; set; }
    public DateOnly PayPeriodStart { get; set; }
    public DateOnly PayPeriodEnd { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal SssContribution { get; set; }
    public decimal PhilhealthContribution { get; set; }
    public decimal PagibigContribution { get; set; }
    public decimal WithholdingTax { get; set; }
    public decimal OvertimePay { get; set; }
    public decimal GrossPay { get; set; }
    public decimal NetPay { get; set; }
    public string PayrollStatus { get; set; } = string.Empty;
    public string? EmployeeName { get; set; }
    public string? EmployeeNumber { get; set; }
}

public class PayrollResponse
{
    public Guid PayrollHeaderId { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public int PayrollPeriodId { get; set; }
    public string PeriodName { get; set; } = string.Empty;
    
    // Earnings
    public decimal BasicSalary { get; set; }
    public decimal Allowances { get; set; }
    public decimal OvertimePay { get; set; }
    public decimal HolidayPay { get; set; }
    public decimal OtherEarnings { get; set; }
    public decimal GrossPay { get; set; }
    
    // Deductions
    public decimal SssContribution { get; set; }
    public decimal PhilhealthContribution { get; set; }
    public decimal PagibigContribution { get; set; }
    public decimal WithholdingTax { get; set; }
    public decimal LoanDeductions { get; set; }
    public decimal OtherDeductions { get; set; }
    public decimal TotalDeductions { get; set; }
    
    // Net Pay
    public decimal NetPay { get; set; }
    
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class PayrollSummaryResponse
{
    public int PayrollPeriodId { get; set; }
    public string PeriodName { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateOnly PayDate { get; set; }
    public int TotalEmployees { get; set; }
    public decimal TotalGrossPay { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TotalNetPay { get; set; }
    public decimal TotalSssEmployee { get; set; }
    public decimal TotalSssEmployer { get; set; }
    public decimal TotalPhilhealthEmployee { get; set; }
    public decimal TotalPhilhealthEmployer { get; set; }
    public decimal TotalPagibigEmployee { get; set; }
    public decimal TotalPagibigEmployer { get; set; }
    public decimal TotalWithholdingTax { get; set; }
}
