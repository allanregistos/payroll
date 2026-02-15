using Microsoft.EntityFrameworkCore;
using Payroll.Core.Entities;

namespace Payroll.Infrastructure.Data;

public class PayrollDbContext : DbContext
{
    public PayrollDbContext(DbContextOptions<PayrollDbContext> options) : base(options)
    {
    }

    // Employee Management
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<EmployeeAssignment> EmployeeAssignments => Set<EmployeeAssignment>();

    // Compensation & Benefits
    public DbSet<SalaryGrade> SalaryGrades => Set<SalaryGrade>();
    public DbSet<EmployeeCompensation> EmployeeCompensations => Set<EmployeeCompensation>();
    public DbSet<Allowance> Allowances => Set<Allowance>();
    public DbSet<EmployeeAllowance> EmployeeAllowances => Set<EmployeeAllowance>();

    // Time & Attendance
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
    public DbSet<EmployeeLeave> EmployeeLeaves => Set<EmployeeLeave>();

    // Government Contribution Tables
    public DbSet<SssContributionTable> SssContributionTables => Set<SssContributionTable>();
    public DbSet<PhilhealthContributionTable> PhilhealthContributionTables => Set<PhilhealthContributionTable>();
    public DbSet<PagibigContributionTable> PagibigContributionTables => Set<PagibigContributionTable>();
    public DbSet<TaxTable> TaxTables => Set<TaxTable>();
    public DbSet<Holiday> Holidays => Set<Holiday>();

    // Payroll Processing
    public DbSet<PayrollPeriod> PayrollPeriods => Set<PayrollPeriod>();
    public DbSet<PayrollHeader> PayrollHeaders => Set<PayrollHeader>();
    public DbSet<PayrollEarning> PayrollEarnings => Set<PayrollEarning>();
    public DbSet<PayrollDeduction> PayrollDeductions => Set<PayrollDeduction>();

    // Authentication
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Set schema
        modelBuilder.HasDefaultSchema("payroll");

        // Configure Employee
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("employees");
            entity.HasKey(e => e.EmployeeId);
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.EmployeeCode).HasColumnName("employee_number").HasMaxLength(20).IsRequired();
            entity.Property(e => e.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.MiddleName).HasColumnName("middle_name").HasMaxLength(100);
            entity.Property(e => e.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Suffix).HasColumnName("suffix").HasMaxLength(10);
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.Gender).HasColumnName("gender").HasMaxLength(1);
            entity.Property(e => e.CivilStatus).HasColumnName("civil_status").HasMaxLength(20);
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasColumnName("mobile_number").HasMaxLength(20);
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.City).HasColumnName("city").HasMaxLength(100);
            entity.Property(e => e.Province).HasColumnName("province").HasMaxLength(100);
            entity.Property(e => e.PostalCode).HasColumnName("postal_code").HasMaxLength(10);
            entity.Property(e => e.HireDate).HasColumnName("date_hired");
            entity.Property(e => e.DateRegularized).HasColumnName("date_regularized");
            entity.Property(e => e.DateSeparated).HasColumnName("date_separated");
            entity.Property(e => e.EmploymentStatus).HasColumnName("employment_status").HasMaxLength(20);
            entity.Property(e => e.EmployeeType).HasColumnName("employee_type").HasMaxLength(20);
            entity.Property(e => e.SssNumber).HasColumnName("sss_number").HasMaxLength(20);
            entity.Property(e => e.PhilhealthNumber).HasColumnName("philhealth_number").HasMaxLength(20);
            entity.Property(e => e.PagibigNumber).HasColumnName("pagibig_number").HasMaxLength(20);
            entity.Property(e => e.TinNumber).HasColumnName("tin_number").HasMaxLength(20);
            entity.Property(e => e.BankName).HasColumnName("bank_name").HasMaxLength(100);
            entity.Property(e => e.BankAccountNumber).HasColumnName("bank_account_number").HasMaxLength(30);
            entity.Property(e => e.BankAccountName).HasColumnName("bank_account_name").HasMaxLength(150);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasIndex(e => e.EmployeeCode).IsUnique();
            entity.HasIndex(e => e.TinNumber);
            
            // Ignore the CurrentCompensation navigation property to avoid conflict
            entity.Ignore(e => e.CurrentCompensation);
            
            // Map IsActive property to is_active column
            entity.Property(e => e.IsActive).HasColumnName("is_active");
        });

        // Configure Department
        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("departments");
            entity.HasKey(e => e.DepartmentId);
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.DepartmentCode).HasColumnName("department_code").HasMaxLength(20).IsRequired();
            entity.Property(e => e.DepartmentName).HasColumnName("department_name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.HasIndex(e => e.DepartmentCode).IsUnique();
        });

        // Configure Position
        modelBuilder.Entity<Position>(entity =>
        {
            entity.ToTable("positions");
            entity.HasKey(e => e.PositionId);
            entity.Property(e => e.PositionId).HasColumnName("position_id");
            entity.Property(e => e.PositionCode).HasColumnName("position_code").HasMaxLength(20).IsRequired();
            entity.Property(e => e.PositionTitle).HasColumnName("position_title").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.HasIndex(e => e.PositionCode).IsUnique();
        });

        // Configure EmployeeAssignment
        modelBuilder.Entity<EmployeeAssignment>(entity =>
        {
            entity.ToTable("employee_assignments");
            entity.HasKey(e => e.AssignmentId);
            entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.PositionId).HasColumnName("position_id");
            entity.Property(e => e.IsPrimary).HasColumnName("is_primary");
            entity.Property(e => e.EffectiveDate).HasColumnName("effective_date");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.HasOne(e => e.Employee)
                .WithMany(e => e.Assignments)
                .HasForeignKey(e => e.EmployeeId);
            entity.HasOne(e => e.Department)
                .WithMany(d => d.EmployeeAssignments)
                .HasForeignKey(e => e.DepartmentId);
            entity.HasOne(e => e.Position)
                .WithMany(p => p.EmployeeAssignments)
                .HasForeignKey(e => e.PositionId);
        });

        // Configure SalaryGrade
        modelBuilder.Entity<SalaryGrade>(entity =>
        {
            entity.ToTable("salary_grades");
            entity.HasKey(e => e.SalaryGradeId);
            entity.Property(e => e.SalaryGradeId).HasColumnName("salary_grade_id");
            entity.Property(e => e.GradeCode).HasColumnName("grade_code").HasMaxLength(20).IsRequired();
            entity.Property(e => e.GradeName).HasColumnName("grade_name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.MinSalary).HasColumnName("min_salary").HasPrecision(15, 2);
            entity.Property(e => e.MaxSalary).HasColumnName("max_salary").HasPrecision(15, 2);
            entity.Property(e => e.IsActive).HasColumnName("is_active");

            entity.HasIndex(e => e.GradeCode).IsUnique();
        });

        // Configure EmployeeCompensation
        modelBuilder.Entity<EmployeeCompensation>(entity =>
        {
            entity.ToTable("employee_compensation");
            entity.HasKey(e => e.CompensationId);
            entity.Property(e => e.CompensationId).HasColumnName("compensation_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.SalaryGradeId).HasColumnName("salary_grade_id");
            entity.Property(e => e.BasicSalary).HasColumnName("basic_salary").HasPrecision(15, 2);
            entity.Property(e => e.DailyRate).HasColumnName("daily_rate").HasPrecision(15, 2);
            entity.Property(e => e.HourlyRate).HasColumnName("hourly_rate").HasPrecision(15, 2);
            entity.Property(e => e.PayFrequency).HasColumnName("pay_frequency").HasMaxLength(20);
            entity.Property(e => e.Cola).HasColumnName("cola").HasPrecision(15, 2);
            entity.Property(e => e.EffectiveDate).HasColumnName("effective_date");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");

            entity.HasOne(e => e.Employee)
                .WithMany(e => e.Compensations)
                .HasForeignKey(e => e.EmployeeId);
            entity.HasOne(e => e.SalaryGrade)
                .WithMany(s => s.EmployeeCompensations)
                .HasForeignKey(e => e.SalaryGradeId);
        });

        // Configure Allowance
        modelBuilder.Entity<Allowance>(entity =>
        {
            entity.ToTable("allowances");
            entity.HasKey(e => e.AllowanceId);
            entity.Property(e => e.AllowanceId).HasColumnName("allowance_id");
            entity.Property(e => e.AllowanceCode).HasColumnName("allowance_code").HasMaxLength(20).IsRequired();
            entity.Property(e => e.AllowanceName).HasColumnName("allowance_name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsTaxable).HasColumnName("is_taxable");
            entity.Property(e => e.IsActive).HasColumnName("is_active");

            entity.HasIndex(e => e.AllowanceCode).IsUnique();
        });

        // Configure EmployeeAllowance
        modelBuilder.Entity<EmployeeAllowance>(entity =>
        {
            entity.ToTable("employee_allowances");
            entity.HasKey(e => e.EmployeeAllowanceId);
            entity.Property(e => e.EmployeeAllowanceId).HasColumnName("employee_allowance_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.AllowanceId).HasColumnName("allowance_id");
            entity.Property(e => e.Amount).HasColumnName("amount").HasPrecision(15, 2);
            entity.Property(e => e.Frequency).HasColumnName("frequency").HasMaxLength(20);
            entity.Property(e => e.EffectiveDate).HasColumnName("effective_date");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.IsActive).HasColumnName("is_active");

            entity.HasOne(e => e.Employee)
                .WithMany(e => e.Allowances)
                .HasForeignKey(e => e.EmployeeId);
            entity.HasOne(e => e.Allowance)
                .WithMany(a => a.EmployeeAllowances)
                .HasForeignKey(e => e.AllowanceId);
        });

        // Configure Attendance
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.ToTable("attendance");
            entity.HasKey(e => e.AttendanceId);
            entity.Property(e => e.AttendanceId).HasColumnName("attendance_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.AttendanceDate).HasColumnName("attendance_date");
            entity.Property(e => e.TimeIn).HasColumnName("time_in");
            entity.Property(e => e.TimeOut).HasColumnName("time_out");
            entity.Property(e => e.BreakOut).HasColumnName("break_out");
            entity.Property(e => e.BreakIn).HasColumnName("break_in");
            entity.Property(e => e.RegularHours).HasColumnName("regular_hours").HasPrecision(5, 2);
            entity.Property(e => e.OvertimeHours).HasColumnName("overtime_hours").HasPrecision(5, 2);
            entity.Property(e => e.LateMinutes).HasColumnName("late_minutes");
            entity.Property(e => e.UndertimeMinutes).HasColumnName("undertime_minutes");
            entity.Property(e => e.IsHoliday).HasColumnName("is_holiday");
            entity.Property(e => e.IsRestDay).HasColumnName("is_rest_day");
            entity.Property(e => e.IsAbsent).HasColumnName("is_absent");
            entity.Property(e => e.Remarks).HasColumnName("remarks");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(e => e.Employee)
                .WithMany(e => e.Attendances)
                .HasForeignKey(e => e.EmployeeId);

            entity.HasIndex(e => new { e.EmployeeId, e.AttendanceDate }).IsUnique();
        });

        // Configure LeaveType
        modelBuilder.Entity<LeaveType>(entity =>
        {
            entity.ToTable("leave_types");
            entity.HasKey(e => e.LeaveTypeId);
            entity.Property(e => e.LeaveTypeId).HasColumnName("leave_type_id");
            entity.Property(e => e.LeaveCode).HasColumnName("leave_code").HasMaxLength(20).IsRequired();
            entity.Property(e => e.LeaveTypeName).HasColumnName("leave_name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsPaid).HasColumnName("is_paid");
            entity.Property(e => e.IsActive).HasColumnName("is_active");

            entity.HasIndex(e => e.LeaveCode).IsUnique();
        });

        // Configure EmployeeLeave
        modelBuilder.Entity<EmployeeLeave>(entity =>
        {
            entity.ToTable("employee_leaves");
            entity.HasKey(e => e.LeaveId);
            entity.Property(e => e.LeaveId).HasColumnName("leave_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.LeaveTypeId).HasColumnName("leave_type_id");
            entity.Property(e => e.StartDate).HasColumnName("leave_date_from");
            entity.Property(e => e.EndDate).HasColumnName("leave_date_to");
            entity.Property(e => e.NumberOfDays).HasColumnName("number_of_days").HasPrecision(5, 2);
            entity.Property(e => e.Reason).HasColumnName("reason");
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20);
            entity.Property(e => e.ApprovalRemarks).HasColumnName("approval_remarks");
            entity.Property(e => e.ApprovedBy).HasColumnName("approved_by");
            entity.Property(e => e.ApprovedDate).HasColumnName("approved_date");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(e => e.Employee)
                .WithMany(e => e.Leaves)
                .HasForeignKey(e => e.EmployeeId);
            entity.HasOne(e => e.LeaveType)
                .WithMany(l => l.EmployeeLeaves)
                .HasForeignKey(e => e.LeaveTypeId);
        });

        // Configure Government Tables
        ConfigureGovernmentTables(modelBuilder);

        // Configure Payroll
        ConfigurePayroll(modelBuilder);
    }

    private void ConfigureGovernmentTables(ModelBuilder modelBuilder)
    {
        // SSS Contribution Table
        modelBuilder.Entity<SssContributionTable>(entity =>
        {
            entity.ToTable("sss_contribution_table");
            entity.HasKey(e => e.SssTableId);
            entity.Property(e => e.SssTableId).HasColumnName("sss_table_id");
            entity.Property(e => e.MinSalary).HasColumnName("min_salary").HasPrecision(15, 2);
            entity.Property(e => e.MaxSalary).HasColumnName("max_salary").HasPrecision(15, 2);
            entity.Property(e => e.EmployeeContribution).HasColumnName("employee_contribution").HasPrecision(15, 2);
            entity.Property(e => e.EmployerContribution).HasColumnName("employer_contribution").HasPrecision(15, 2);
            entity.Property(e => e.TotalContribution).HasColumnName("total_contribution").HasPrecision(15, 2);
            entity.Property(e => e.EcContribution).HasColumnName("ec_contribution").HasPrecision(15, 2);
            entity.Property(e => e.EffectiveDate).HasColumnName("effective_date");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
        });

        // PhilHealth Contribution Table
        modelBuilder.Entity<PhilhealthContributionTable>(entity =>
        {
            entity.ToTable("philhealth_contribution_table");
            entity.HasKey(e => e.PhilhealthTableId);
            entity.Property(e => e.PhilhealthTableId).HasColumnName("philhealth_table_id");
            entity.Property(e => e.MinSalary).HasColumnName("min_salary").HasPrecision(15, 2);
            entity.Property(e => e.MaxSalary).HasColumnName("max_salary").HasPrecision(15, 2);
            entity.Property(e => e.PremiumRate).HasColumnName("premium_rate").HasPrecision(5, 4);
            entity.Property(e => e.EmployeeShare).HasColumnName("employee_share").HasPrecision(5, 4);
            entity.Property(e => e.EmployerShare).HasColumnName("employer_share").HasPrecision(5, 4);
            entity.Property(e => e.EffectiveDate).HasColumnName("effective_date");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
        });

        // Pag-IBIG Contribution Table
        modelBuilder.Entity<PagibigContributionTable>(entity =>
        {
            entity.ToTable("pagibig_contribution_table");
            entity.HasKey(e => e.PagibigTableId);
            entity.Property(e => e.PagibigTableId).HasColumnName("pagibig_table_id");
            entity.Property(e => e.MinSalary).HasColumnName("min_salary").HasPrecision(15, 2);
            entity.Property(e => e.MaxSalary).HasColumnName("max_salary").HasPrecision(15, 2);
            entity.Property(e => e.EmployeeRate).HasColumnName("employee_rate").HasPrecision(5, 4);
            entity.Property(e => e.EmployerRate).HasColumnName("employer_rate").HasPrecision(5, 4);
            entity.Property(e => e.MaxEmployeeContribution).HasColumnName("max_employee_contribution").HasPrecision(15, 2);
            entity.Property(e => e.MaxEmployerContribution).HasColumnName("max_employer_contribution").HasPrecision(15, 2);
            entity.Property(e => e.EffectiveDate).HasColumnName("effective_date");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
        });

        // Tax Table
        modelBuilder.Entity<TaxTable>(entity =>
        {
            entity.ToTable("tax_table");
            entity.HasKey(e => e.TaxTableId);
            entity.Property(e => e.TaxTableId).HasColumnName("tax_table_id");
            entity.Property(e => e.MinCompensation).HasColumnName("min_compensation").HasPrecision(15, 2);
            entity.Property(e => e.MaxCompensation).HasColumnName("max_compensation").HasPrecision(15, 2);
            entity.Property(e => e.BaseTax).HasColumnName("base_tax").HasPrecision(15, 2);
            entity.Property(e => e.TaxRate).HasColumnName("tax_rate").HasPrecision(5, 4);
            entity.Property(e => e.ExcessOver).HasColumnName("excess_over").HasPrecision(15, 2);
            entity.Property(e => e.PeriodType).HasColumnName("period_type").HasMaxLength(20);
            entity.Property(e => e.EffectiveDate).HasColumnName("effective_date");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
        });

        // Holiday
        modelBuilder.Entity<Holiday>(entity =>
        {
            entity.ToTable("holidays");
            entity.HasKey(e => e.HolidayId);
            entity.Property(e => e.HolidayId).HasColumnName("holiday_id");
            entity.Property(e => e.HolidayName).HasColumnName("holiday_name").HasMaxLength(100);
            entity.Property(e => e.HolidayDate).HasColumnName("holiday_date");
            entity.Property(e => e.HolidayType).HasColumnName("holiday_type").HasMaxLength(20);
            entity.Property(e => e.RegularDayPremium).HasColumnName("regular_day_premium").HasPrecision(5, 4);
            entity.Property(e => e.RestDayPremium).HasColumnName("rest_day_premium").HasPrecision(5, 4);
            entity.Property(e => e.Year).HasColumnName("year");
            entity.Property(e => e.IsRecurring).HasColumnName("is_recurring");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.HasIndex(e => e.HolidayDate);
        });
    }

    private void ConfigurePayroll(ModelBuilder modelBuilder)
    {
        // PayrollPeriod
        modelBuilder.Entity<PayrollPeriod>(entity =>
        {
            entity.ToTable("payroll_periods");
            entity.HasKey(e => e.PayrollPeriodId);
            entity.Property(e => e.PayrollPeriodId).HasColumnName("payroll_period_id");
            entity.Property(e => e.PeriodName).HasColumnName("period_name").HasMaxLength(100);
            entity.Property(e => e.PeriodType).HasColumnName("period_type").HasMaxLength(20);
            entity.Property(e => e.StartDate).HasColumnName("period_start_date");
            entity.Property(e => e.EndDate).HasColumnName("period_end_date");
            entity.Property(e => e.PayDate).HasColumnName("payment_date");
            entity.Property(e => e.CutoffDate).HasColumnName("cutoff_date");
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ProcessedAt).HasColumnName("processed_at");
            entity.Property(e => e.ProcessedBy).HasColumnName("processed_by");
        });

        // PayrollHeader
        modelBuilder.Entity<PayrollHeader>(entity =>
        {
            entity.ToTable("payroll_headers");
            entity.HasKey(e => e.PayrollHeaderId);
            entity.Property(e => e.PayrollHeaderId).HasColumnName("payroll_header_id");
            entity.Property(e => e.PayrollPeriodId).HasColumnName("payroll_period_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.BasicSalary).HasColumnName("basic_salary").HasPrecision(15, 2);
            entity.Property(e => e.DailyRate).HasColumnName("daily_rate").HasPrecision(15, 2);
            entity.Property(e => e.HourlyRate).HasColumnName("hourly_rate").HasPrecision(15, 2);
            entity.Property(e => e.RegularHours).HasColumnName("regular_hours").HasPrecision(10, 2);
            entity.Property(e => e.OvertimeHours).HasColumnName("overtime_hours").HasPrecision(10, 2);
            entity.Property(e => e.HolidayHours).HasColumnName("holiday_hours").HasPrecision(10, 2);
            entity.Property(e => e.RestDayHours).HasColumnName("rest_day_hours").HasPrecision(10, 2);
            entity.Property(e => e.DaysWorked).HasColumnName("days_worked").HasPrecision(5, 2);
            entity.Property(e => e.DaysAbsent).HasColumnName("days_absent").HasPrecision(5, 2);
            entity.Property(e => e.DaysLeave).HasColumnName("days_leave").HasPrecision(5, 2);
            entity.Property(e => e.BasicPay).HasColumnName("basic_pay").HasPrecision(15, 2);
            entity.Property(e => e.OvertimePay).HasColumnName("overtime_pay").HasPrecision(15, 2);
            entity.Property(e => e.HolidayPay).HasColumnName("holiday_pay").HasPrecision(15, 2);
            entity.Property(e => e.NightDifferential).HasColumnName("night_differential").HasPrecision(15, 2);
            entity.Property(e => e.TotalAllowances).HasColumnName("total_allowances").HasPrecision(15, 2);
            entity.Property(e => e.TotalBonuses).HasColumnName("total_bonuses").HasPrecision(15, 2);
            entity.Property(e => e.TotalAdjustments).HasColumnName("total_adjustments").HasPrecision(15, 2);
            entity.Property(e => e.GrossPay).HasColumnName("gross_pay").HasPrecision(15, 2);
            entity.Property(e => e.SssContribution).HasColumnName("sss_contribution").HasPrecision(15, 2);
            entity.Property(e => e.PhilhealthContribution).HasColumnName("philhealth_contribution").HasPrecision(15, 2);
            entity.Property(e => e.PagibigContribution).HasColumnName("pagibig_contribution").HasPrecision(15, 2);
            entity.Property(e => e.WithholdingTax).HasColumnName("withholding_tax").HasPrecision(15, 2);
            entity.Property(e => e.TotalLoans).HasColumnName("total_loans").HasPrecision(15, 2);
            entity.Property(e => e.TotalOtherDeductions).HasColumnName("total_other_deductions").HasPrecision(15, 2);
            entity.Property(e => e.TotalDeductions).HasColumnName("total_deductions").HasPrecision(15, 2);
            entity.Property(e => e.NetPay).HasColumnName("net_pay").HasPrecision(15, 2);
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20);
            entity.Property(e => e.Remarks).HasColumnName("remarks");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ApprovedAt).HasColumnName("approved_at");
            entity.Property(e => e.ApprovedBy).HasColumnName("approved_by");

            entity.HasOne(e => e.PayrollPeriod)
                .WithMany(p => p.PayrollHeaders)
                .HasForeignKey(e => e.PayrollPeriodId);
            entity.HasOne(e => e.Employee)
                .WithMany(e => e.PayrollHeaders)
                .HasForeignKey(e => e.EmployeeId);

            entity.HasIndex(e => e.PayrollPeriodId);
            entity.HasIndex(e => e.EmployeeId);
        });

        // PayrollEarning
        modelBuilder.Entity<PayrollEarning>(entity =>
        {
            entity.ToTable("payroll_earnings");
            entity.HasKey(e => e.PayrollEarningId);
            entity.Property(e => e.PayrollEarningId).HasColumnName("payroll_earning_id");
            entity.Property(e => e.PayrollHeaderId).HasColumnName("payroll_header_id");
            entity.Property(e => e.EarningType).HasColumnName("earning_type").HasMaxLength(50);
            entity.Property(e => e.EarningCode).HasColumnName("earning_code").HasMaxLength(20);
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(200);
            entity.Property(e => e.Quantity).HasColumnName("quantity").HasPrecision(10, 2);
            entity.Property(e => e.Rate).HasColumnName("rate").HasPrecision(15, 2);
            entity.Property(e => e.Amount).HasColumnName("amount").HasPrecision(15, 2);
            entity.Property(e => e.IsTaxable).HasColumnName("is_taxable");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.HasOne(e => e.PayrollHeader)
                .WithMany(p => p.Earnings)
                .HasForeignKey(e => e.PayrollHeaderId);
        });

        // PayrollDeduction
        modelBuilder.Entity<PayrollDeduction>(entity =>
        {
            entity.ToTable("payroll_deductions");
            entity.HasKey(e => e.PayrollDeductionId);
            entity.Property(e => e.PayrollDeductionId).HasColumnName("payroll_deduction_id");
            entity.Property(e => e.PayrollHeaderId).HasColumnName("payroll_header_id");
            entity.Property(e => e.DeductionType).HasColumnName("deduction_type").HasMaxLength(50);
            entity.Property(e => e.DeductionCode).HasColumnName("deduction_code").HasMaxLength(20);
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(200);
            entity.Property(e => e.Amount).HasColumnName("amount").HasPrecision(15, 2);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.HasOne(e => e.PayrollHeader)
                .WithMany(p => p.Deductions)
                .HasForeignKey(e => e.PayrollHeaderId);
        });

        // Configure User
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.UserId);

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Username).HasColumnName("username").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(100).IsRequired();
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            entity.Property(e => e.FirstName).HasColumnName("first_name").HasMaxLength(100);
            entity.Property(e => e.LastName).HasColumnName("last_name").HasMaxLength(100);
            entity.Property(e => e.Role).HasColumnName("role").HasMaxLength(20).IsRequired();
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.LastLogin).HasColumnName("last_login");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();

            entity.HasOne(e => e.Employee)
                .WithMany()
                .HasForeignKey(e => e.EmployeeId);
        });

        // LoanType
        modelBuilder.Entity<LoanType>(entity =>
        {
            entity.ToTable("loan_types");
            entity.HasKey(e => e.LoanTypeId);
            entity.Property(e => e.LoanTypeId).HasColumnName("loan_type_id");
            entity.Property(e => e.LoanCode).HasColumnName("loan_code").HasMaxLength(20);
            entity.Property(e => e.LoanName).HasColumnName("loan_name").HasMaxLength(100);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsActive).HasColumnName("is_active");

            entity.HasIndex(e => e.LoanCode).IsUnique();
        });

        // EmployeeLoan
        modelBuilder.Entity<EmployeeLoan>(entity =>
        {
            entity.ToTable("employee_loans");
            entity.HasKey(e => e.LoanId);
            entity.Property(e => e.LoanId).HasColumnName("loan_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.LoanTypeId).HasColumnName("loan_type_id");
            entity.Property(e => e.LoanAmount).HasColumnName("loan_amount").HasPrecision(15, 2);
            entity.Property(e => e.InterestRate).HasColumnName("interest_rate").HasPrecision(5, 4);
            entity.Property(e => e.NumberOfInstallments).HasColumnName("number_of_installments");
            entity.Property(e => e.MonthlyAmortization).HasColumnName("monthly_amortization").HasPrecision(15, 2);
            entity.Property(e => e.LoanDate).HasColumnName("loan_date");
            entity.Property(e => e.FirstDeductionDate).HasColumnName("first_deduction_date");
            entity.Property(e => e.TotalPaid).HasColumnName("total_paid").HasPrecision(15, 2);
            entity.Property(e => e.Balance).HasColumnName("balance").HasPrecision(15, 2);
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20);
            entity.Property(e => e.Remarks).HasColumnName("remarks");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");

            entity.HasOne(e => e.Employee)
                .WithMany()
                .HasForeignKey(e => e.EmployeeId);

            entity.HasOne(e => e.LoanType)
                .WithMany(lt => lt.EmployeeLoans)
                .HasForeignKey(e => e.LoanTypeId);
        });

        // LoanPayment
        modelBuilder.Entity<LoanPayment>(entity =>
        {
            entity.ToTable("loan_payments");
            entity.HasKey(e => e.PaymentId);
            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.LoanId).HasColumnName("loan_id");
            entity.Property(e => e.PayrollHeaderId).HasColumnName("payroll_header_id");
            entity.Property(e => e.PaymentDate).HasColumnName("payment_date");
            entity.Property(e => e.PrincipalAmount).HasColumnName("principal_amount").HasPrecision(15, 2);
            entity.Property(e => e.InterestAmount).HasColumnName("interest_amount").HasPrecision(15, 2);
            entity.Property(e => e.TotalAmount).HasColumnName("total_amount").HasPrecision(15, 2);
            entity.Property(e => e.BalanceAfterPayment).HasColumnName("balance_after_payment").HasPrecision(15, 2);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.HasOne(e => e.Loan)
                .WithMany(l => l.Payments)
                .HasForeignKey(e => e.LoanId);
        });
    }
}
