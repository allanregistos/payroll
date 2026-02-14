namespace Payroll.Core.Entities;

public class PayrollPeriod
{
    public int PayrollPeriodId { get; set; }
    public string PeriodName { get; set; } = string.Empty;
    public string PeriodType { get; set; } = "Monthly";
    
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateOnly PayDate { get; set; }
    
    public DateTime? CutoffDate { get; set; }
    
    public string Status { get; set; } = "Draft";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public Guid? ProcessedBy { get; set; }
    
    public ICollection<PayrollHeader> PayrollHeaders { get; set; } = new List<PayrollHeader>();
}

public class PayrollHeader
{
    public Guid PayrollHeaderId { get; set; }
    public int PayrollPeriodId { get; set; }
    public Guid EmployeeId { get; set; }
    
    // Basic Information
    public decimal BasicSalary { get; set; }
    public decimal? DailyRate { get; set; }
    public decimal? HourlyRate { get; set; }
    
    // Hours Worked
    public decimal RegularHours { get; set; }
    public decimal OvertimeHours { get; set; }
    public decimal HolidayHours { get; set; }
    public decimal RestDayHours { get; set; }
    
    // Days
    public decimal DaysWorked { get; set; }
    public decimal DaysAbsent { get; set; }
    public decimal DaysLeave { get; set; }
    
    // Gross Pay Components
    public decimal BasicPay { get; set; }
    public decimal OvertimePay { get; set; }
    public decimal HolidayPay { get; set; }
    public decimal NightDifferential { get; set; }
    
    // Other Income
    public decimal TotalAllowances { get; set; }
    public decimal TotalBonuses { get; set; }
    public decimal TotalAdjustments { get; set; }
    
    // Gross Calculation
    public decimal GrossPay { get; set; }
    
    // Deductions
    public decimal SssContribution { get; set; }
    public decimal PhilhealthContribution { get; set; }
    public decimal PagibigContribution { get; set; }
    public decimal WithholdingTax { get; set; }
    
    public decimal TotalLoans { get; set; }
    public decimal TotalOtherDeductions { get; set; }
    
    // Net Calculation
    public decimal TotalDeductions { get; set; }
    public decimal NetPay { get; set; }
    
    // Status
    public string Status { get; set; } = "Draft";
    
    public string? Remarks { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public Guid? ApprovedBy { get; set; }
    
    public PayrollPeriod PayrollPeriod { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
    public ICollection<PayrollEarning> Earnings { get; set; } = new List<PayrollEarning>();
    public ICollection<PayrollDeduction> Deductions { get; set; } = new List<PayrollDeduction>();
}

public class PayrollEarning
{
    public int PayrollEarningId { get; set; }
    public Guid PayrollHeaderId { get; set; }
    
    public string EarningType { get; set; } = string.Empty;
    public string? EarningCode { get; set; }
    public string? Description { get; set; }
    
    public decimal? Quantity { get; set; }
    public decimal? Rate { get; set; }
    public decimal Amount { get; set; }
    
    public bool IsTaxable { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public PayrollHeader PayrollHeader { get; set; } = null!;
}

public class PayrollDeduction
{
    public int PayrollDeductionId { get; set; }
    public Guid PayrollHeaderId { get; set; }
    
    public string DeductionType { get; set; } = string.Empty;
    public string? DeductionCode { get; set; }
    public string? Description { get; set; }
    
    public decimal Amount { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public PayrollHeader PayrollHeader { get; set; } = null!;
}
