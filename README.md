# Philippine Payroll System

A comprehensive payroll management system for Philippine companies built with .NET Core and PostgreSQL.

## Tech Stack

- **Backend**: ASP.NET Core 8.0 Web API
- **Database**: PostgreSQL 15+
- **ORM**: Entity Framework Core with Npgsql
- **Authentication**: ASP.NET Core Identity + JWT
- **Frontend**: React (or Blazor Server)
- **OS**: Linux (Ubuntu recommended)

## Core Features

### 1. Employee Management
- Employee profiles with complete personal and employment information
- Department and position assignments
- Government ID tracking (SSS, PhilHealth, Pag-IBIG, TIN)

### 2. Compensation & Benefits
- Salary grades and compensation management
- Multiple allowances support (transportation, meal, housing, etc.)
- Flexible pay frequencies (monthly, semi-monthly, weekly, daily)

### 3. Time & Attendance
- Daily time in/out tracking
- Overtime, late, and undertime calculations
- Holiday and rest day processing
- Leave management (VL, SL, ML, PL, etc.)

### 4. Philippine Government Contributions
- **SSS (Social Security System)** - Automated contribution computation
- **PhilHealth** - Premium calculation based on monthly salary
- **Pag-IBIG** - HDMF contribution with rates
- **BIR Withholding Tax** - TRAIN law compliant tax computation

### 5. Payroll Processing
- Automated payroll computation per period
- Gross pay, deductions, and net pay calculation
- Detailed earnings and deductions breakdown
- Support for multiple pay periods

### 6. Loans & Deductions
- SSS, Pag-IBIG, and company loans
- Automated amortization deduction
- Other deductions (tardiness, absence, cash advances)

### 7. Compliance & Reporting
- Holiday calendar with premium rates
- Audit trail for all transactions
- Role-based access control

## Database Structure

The database schema includes the following main areas:

### Core Tables
- `employees` - Employee master data
- `departments` - Organization structure
- `positions` - Job positions
- `employee_assignments` - Employee-department-position mapping

### Compensation
- `employee_compensation` - Salary and pay rates
- `allowances` - Allowance types
- `employee_allowances` - Employee allowance assignments

### Time Management
- `attendance` - Daily time records
- `leave_types` - Leave categories
- `employee_leaves` - Leave applications

### Government Contributions
- `sss_contribution_table` - SSS contribution matrix
- `philhealth_contribution_table` - PhilHealth rates
- `pagibig_contribution_table` - Pag-IBIG rates
- `tax_table` - BIR withholding tax brackets

### Payroll
- `payroll_periods` - Payroll processing periods
- `payroll_headers` - Main payroll computation
- `payroll_earnings` - Detailed earnings
- `payroll_deductions` - Detailed deductions

### Loans & Deductions
- `employee_loans` - Loan records
- `loan_payments` - Payment history
- `employee_other_deductions` - Other deduction types

## Database Setup

### Prerequisites
```bash
# Install PostgreSQL
sudo apt update
sudo apt install postgresql postgresql-contrib

# Start PostgreSQL service
sudo systemctl start postgresql
sudo systemctl enable postgresql
```

### Create Database
```bash
# Switch to postgres user
sudo -u postgres psql

# In PostgreSQL prompt:
CREATE DATABASE payroll_db;
CREATE USER payroll_user WITH PASSWORD 'your_secure_password';
GRANT ALL PRIVILEGES ON DATABASE payroll_db TO payroll_user;
\q
```

### Run Schema
```bash
psql -U payroll_user -d payroll_db -f database/schema.sql
```

## Project Structure (Recommended)

```
payroll/
├── database/
│   └── schema.sql
├── src/
│   ├── Payroll.API/              # Web API Project
│   ├── Payroll.Core/             # Domain models & interfaces
│   ├── Payroll.Infrastructure/   # Data access & EF Core
│   ├── Payroll.Application/      # Business logic & services
│   └── Payroll.Shared/           # Shared utilities & DTOs
├── tests/
│   ├── Payroll.UnitTests/
│   └── Payroll.IntegrationTests/
└── README.md
```

## Getting Started

### 1. Create .NET Solution
```bash
dotnet new sln -n Payroll

# Create projects
dotnet new webapi -n Payroll.API
dotnet new classlib -n Payroll.Core
dotnet new classlib -n Payroll.Infrastructure
dotnet new classlib -n Payroll.Application
dotnet new classlib -n Payroll.Shared

# Add projects to solution
dotnet sln add src/**/*.csproj
```

### 2. Install Required Packages
```bash
cd src/Payroll.Infrastructure
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design

cd ../Payroll.API
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Swashbuckle.AspNetCore
```

### 3. Configure Connection String
In `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=payroll_db;Username=payroll_user;Password=your_secure_password"
  }
}
```

## Key Philippine Payroll Computations

### SSS Contribution
Based on monthly salary bracket with fixed employee/employer shares.

### PhilHealth Premium
- 5% of monthly basic salary (as of 2024)
- Split 50/50 between employee and employer
- Maximum ceiling applies

### Pag-IBIG Contribution
- 2% employee share (max ₱100)
- 2% employer share (max ₱100)

### Withholding Tax (TRAIN Law)
Progressive tax rates:
- Up to ₱250,000: 0%
- ₱250,001 - ₱400,000: 15%
- ₱400,001 - ₱800,000: 20%
- ₱800,001 - ₱2,000,000: 25%
- ₱2,000,001 - ₱8,000,000: 30%
- Above ₱8,000,000: 35%

### Overtime Pay
- Regular OT: Basic rate × 1.25
- Rest day OT: Basic rate × 1.30
- Holiday OT: Basic rate × 2.0 (or 2.6 on rest day)

## Development Workflow

1. **Setup Database**: Run schema.sql
2. **Generate EF Models**: Use database-first or code-first approach
3. **Implement Repositories**: Data access layer
4. **Build Services**: Business logic for payroll computation
5. **Create API Endpoints**: RESTful API controllers
6. **Add Authentication**: JWT-based auth
7. **Testing**: Unit and integration tests

## Security Considerations

- Use parameterized queries (EF Core handles this)
- Implement role-based authorization
- Encrypt sensitive data (passwords, TIN, etc.)
- Enable audit logging for all changes
- Use HTTPS in production
- Secure API with JWT tokens

## License

Proprietary - Internal Use Only

## Support

For questions or issues, contact the development team.
