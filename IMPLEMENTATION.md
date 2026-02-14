# Philippine Payroll System - Implementation Guide

## Project Structure

```
payroll/
├── src/
│   ├── Payroll.API/              # Web API endpoints
│   ├── Payroll.Core/             # Domain entities & interfaces
│   ├── Payroll.Infrastructure/   # DbContext, repositories, data access
│   └── Payroll.Application/      # Business logic & services
├── database/
│   ├── schema.sql                # PostgreSQL schema
│   └── sample_data.sql           # Sample data for testing
└── Payroll.sln
```

## Implementation Progress

### ✅ Phase 1: Foundation (COMPLETED)

1. **Project Structure**
   - Created solution with 4 projects: API, Core, Infrastructure, Application
   - Set up proper project references

2. **Domain Models** (Payroll.Core/Entities/)
   - `Employee.cs` - Employee master data
   - `Organization.cs` - Department, Position, EmployeeAssignment
   - `Compensation.cs` - SalaryGrade, EmployeeCompensation, Allowance, EmployeeAllowance
   - `Attendance.cs` - Attendance, LeaveType, EmployeeLeave
   - `GovernmentTables.cs` - SSS, PhilHealth, Pag-IBIG, TaxTable, Holiday
   - `Payroll.cs` - PayrollPeriod, PayrollHeader, PayrollEarning, PayrollDeduction

3. **Database Context** (Payroll.Infrastructure/Data/)
   - `PayrollDbContext.cs` - Full EF Core configuration
   - All entities mapped to PostgreSQL tables with proper column names
   - Relationships configured

4. **NuGet Packages Installed**
   - Npgsql.EntityFrameworkCore.PostgreSQL (v9.0.2)
   - Microsoft.EntityFrameworkCore.Design (v9.0.0)

### ✅ Phase 2: Philippine Government Calculators (COMPLETED)

5. **SSS Contribution Calculator** (`SssContributionCalculator.cs`)
   - Database-driven bracket lookup
   - Employee + Employer + EC contributions
   
6. **PhilHealth Calculator** (`PhilhealthContributionCalculator.cs`)
   - 5% premium rate with salary ceiling
   - 50/50 employee-employer split
   
7. **Pag-IBIG Calculator** (`PagibigContributionCalculator.cs`)
   - 2% contribution rate
   - ₱100 maximum cap per share
   
8. **Withholding Tax Calculator** (`WithholdingTaxCalculator.cs`)
   - TRAIN Law progressive tax brackets
   - Annual to monthly conversion

### ✅ Phase 3: Payroll Computation Engine (COMPLETED)

9. **Payroll Computation Service** (`PayrollComputationService.cs`)
   - Orchestrates all 4 government calculators
   - Computes gross pay (basic + allowances + OT + holiday)
   - Calculates all deductions
   - Returns complete net pay breakdown
   - Batch processing for multiple employees

### ✅ Phase 4: API Endpoints (COMPLETED)

10. **Employee Management APIs** (`EmployeesController.cs`)
    - GET /api/employees - List all employees
    - GET /api/employees/{id} - Get employee details
    - POST /api/employees - Create new employee
    - PUT /api/employees/{id} - Update employee
    - DELETE /api/employees/{id} - Deactivate employee

11. **Attendance & Leave APIs** (`AttendanceController.cs`, `LeavesController.cs`)
    - GET /api/attendance - Query attendance records
    - POST /api/attendance - Record attendance
    - PUT /api/attendance/{id} - Update attendance
    - GET /api/leaves - Query leave requests
    - POST /api/leaves - Submit leave request
    - PUT /api/leaves/{id}/approve - Approve/reject leave

12. **Payroll Processing APIs** (`PayrollController.cs`)
    - GET /api/payroll/periods - List payroll periods
    - POST /api/payroll/periods - Create payroll period
    - POST /api/payroll/process - Process payroll for period
    - GET /api/payroll/period/{id} - Get payroll records
    - GET /api/payroll/period/{id}/summary - Get payroll summary

## Key Features

### Core Philippine Payroll Computations

#### SSS (Social Security System)
- Based on salary brackets
- Employee + Employer contributions
- EC (Employees' Compensation) contribution

#### PhilHealth
- 5% premium rate (as of 2024)
- 50/50 employee-employer split
- Maximum salary ceiling: ₱100,000

#### Pag-IBIG (HDMF)
- 2% employee contribution (max ₱100)
- 2% employer contribution (max ₱100)

#### Withholding Tax (TRAIN Law)
Progressive tax rates:
- ₱0 - ₱250,000: 0%
- ₱250,001 - ₱400,000: 15%
- ₱400,001 - ₱800,000: 20%
- ₱800,001 - ₱2,000,000: 25%
- ₱2,000,001 - ₱8,000,000: 30%
- Above ₱8,000,000: 35%

#### Overtime Rates
- Regular OT: 1.25x hourly rate
- Rest day OT: 1.30x hourly rate
- Regular holiday: 2.0x
- Regular holiday on rest day: 2.6x

## Database Configuration

### Connection String
Update `appsettings.json` in Payroll.API:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=payroll_db;Username=payroll_user;Password=your_password"
  }
}
```

### Running Migrations

```bash
# From src/Payroll.API folder
dotnet ef migrations add InitialCreate --project ../Payroll.Infrastructure

# Apply migrations
dotnet ef database update --project ../Payroll.Infrastructure
```

## Entity Key Relationships

### Employee → Compensation Flow
```
Employee (UUID)
  ├─→ EmployeeAssignment (Department, Position)
  ├─→ EmployeeCompensation (SalaryGrade, BasicSalary)
  ├─→ EmployeeAllowance (Allowance types)
  ├─→ Attendance (Daily records)
  └─→ PayrollHeader (Computed payroll)
```

### Payroll Computation Flow
```
1. Get Employee Compensation (Basic Salary)
2. Calculate work hours from Attendance
3. Compute Gross Pay (Basic + OT + Allowances)
4. Compute Deductions:
   ├─→ SSS (from contribution table)
   ├─→ PhilHealth (premium calculation)
   ├─→ Pag-IBIG (2% of salary)
   └─→ Withholding Tax (TRAIN law)
5. Net Pay = Gross Pay - Deductions
```

## Testing with Sample Data

The database includes sample data:
- 3 employees (Juan, Maria, Pedro)
- 5 departments
- 6 positions
- Government contribution tables (2024 rates)
- 2026 Philippine holidays

## API Endpoints (To be implemented)

### Employees
- GET /api/employees
- GET /api/employees/{id}
- POST /api/employees
- PUT /api/employees/{id}
- DELETE /api/employees/{id}

### Attendance
- POST /api/attendance
- GET /api/attendance/{employeeId}?from={date}&to={date}

### Payroll
- POST /api/payroll/periods (Create payroll period)
- POST /api/payroll/compute (Generate payroll for period)
- GET /api/payroll/{periodId}
- POST /api/payroll/{periodId}/finalize

### Reports
- GET /api/reports/payroll-summary/{periodId}
- GET /api/reports/government-remittance/{periodId}
- GET /api/reports/payslip/{employeeId}/{periodId}

## Development Workflow

1. **Add new entity**: Create in Payroll.Core/Entities
2. **Map to database**: Configure in PayrollDbContext.OnModelCreating
3. **Create service**: Add business logic in Payroll.Application
4. **Create API**: Add controller in Payroll.API

## Environment Setup

### Prerequisites
- .NET 9.0 SDK
- PostgreSQL 15+
- pgAdmin (optional)
- VS Code or Visual Studio

### Quick Start
```bash
# Clone/navigate to project
cd /home/allanregistos/apps/payroll

# Restore packages
dotnet restore

# Build solution
dotnet build

# Run API
cd src/Payroll.API
dotnet run
```

## Notes

- **UUID Usage**: Employees, Users, and PayrollHeaders use UUID for better security and distributed systems support
- **IDENTITY Columns**: All other tables use INTEGER IDENTITY instead of SERIAL
- **Schema**: All tables are in the `payroll` schema
- **Audit Fields**: Most tables include created_at, updated_at, created_by, updated_by
- **Soft Deletes**: Use is_active flags instead of hard deletes

## Next Steps

### 🎯 Immediate Next Steps

1. **Database Setup**
   ```bash
   # Create PostgreSQL database
   createdb payroll_db
   
   # Run schema (from database/schema.sql)
   psql -d payroll_db -f database/schema.sql
   
   # Load sample data (from database/sample_data.sql)
   psql -d payroll_db -f database/sample_data.sql
   ```

2. **Update Connection String**
   - Edit `src/Payroll.API/appsettings.json`
   - Set your PostgreSQL credentials

3. **Run and Test**
   ```bash
   cd src/Payroll.API
   dotnet run
   # API will be available at https://localhost:5001
   ```

4. **Test API Endpoints**
   - Use the OpenAPI/Swagger UI at https://localhost:5001/swagger
   - Test employee creation, attendance recording, payroll processing

### 📋 Future Enhancements

5. ✅ **Authentication & Authorization (COMPLETED)**
   - JWT authentication with Bearer tokens
   - Role-based access control (Admin, HR, Employee)
   - Secure sensitive endpoints
   - User management (register, login, change password)
   - See [AUTHENTICATION.md](AUTHENTICATION.md) for details

6. **Report Generation**
   - Payslip PDF generation
   - Government remittance reports (SSS, PhilHealth, Pag-IBIG, BIR)
   - Payroll summary Excel export

7. **Unit Tests**
   - Calculator unit tests
   - Service layer tests
   - API integration tests

8. **Additional Features**
   - 13th month pay calculation
   - Loan management
   - Performance bonus computation
   - Email notifications for payslips

## Support

For Philippine payroll compliance questions, refer to:
- SSS: https://www.sss.gov.ph
- PhilHealth: https://www.philhealth.gov.ph
- Pag-IBIG: https://www.pagibigfund.gov.ph
- BIR: https://www.bir.gov.ph
