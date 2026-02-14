-- Insert Test Data for Philippine Payroll System
-- Run this in pgAdmin Query Tool

-- Insert Departments
INSERT INTO payroll.departments (department_code, department_name, description, is_active) 
SELECT 'IT', 'Information Technology', 'IT Department', true
WHERE NOT EXISTS (SELECT 1 FROM payroll.departments WHERE department_code = 'IT');

INSERT INTO payroll.departments (department_code, department_name, description, is_active) 
SELECT 'HR', 'Human Resources', 'HR Department', true
WHERE NOT EXISTS (SELECT 1 FROM payroll.departments WHERE department_code = 'HR');

INSERT INTO payroll.departments (department_code, department_name, description, is_active) 
SELECT 'ACC', 'Accounting', 'Accounting Department', true
WHERE NOT EXISTS (SELECT 1 FROM payroll.departments WHERE department_code = 'ACC');

INSERT INTO payroll.departments (department_code, department_name, description, is_active) 
SELECT 'OPS', 'Operations', 'Operations Department', true
WHERE NOT EXISTS (SELECT 1 FROM payroll.departments WHERE department_code = 'OPS');

-- Insert Positions
INSERT INTO payroll.positions (position_code, position_title, description, is_active) 
SELECT 'DEV', 'Software Developer', 'Software Development Position', true
WHERE NOT EXISTS (SELECT 1 FROM payroll.positions WHERE position_code = 'DEV');

INSERT INTO payroll.positions (position_code, position_title, description, is_active) 
SELECT 'MGR', 'Manager', 'Management Position', true
WHERE NOT EXISTS (SELECT 1 FROM payroll.positions WHERE position_code = 'MGR');

INSERT INTO payroll.positions (position_code, position_title, description, is_active) 
SELECT 'ACC', 'Accountant', 'Accounting Position', true
WHERE NOT EXISTS (SELECT 1 FROM payroll.positions WHERE position_code = 'ACC');

INSERT INTO payroll.positions (position_code, position_title, description, is_active) 
SELECT 'HR-SPEC', 'HR Specialist', 'HR Specialist Position', true
WHERE NOT EXISTS (SELECT 1 FROM payroll.positions WHERE position_code = 'HR-SPEC');

-- Insert Salary Grades
INSERT INTO payroll.salary_grades (grade_code, grade_name, min_salary, max_salary, is_active) 
SELECT 'SG1', 'Entry Level', 15000.00, 20000.00, true
WHERE NOT EXISTS (SELECT 1 FROM payroll.salary_grades WHERE grade_code = 'SG1');

INSERT INTO payroll.salary_grades (grade_code, grade_name, min_salary, max_salary, is_active) 
SELECT 'SG2', 'Junior Level', 20000.00, 30000.00, true
WHERE NOT EXISTS (SELECT 1 FROM payroll.salary_grades WHERE grade_code = 'SG2');

INSERT INTO payroll.salary_grades (grade_code, grade_name, min_salary, max_salary, is_active) 
SELECT 'SG3', 'Mid Level', 30000.00, 50000.00, true
WHERE NOT EXISTS (SELECT 1 FROM payroll.salary_grades WHERE grade_code = 'SG3');

INSERT INTO payroll.salary_grades (grade_code, grade_name, min_salary, max_salary, is_active) 
SELECT 'SG4', 'Senior Level', 50000.00, 80000.00, true
WHERE NOT EXISTS (SELECT 1 FROM payroll.salary_grades WHERE grade_code = 'SG4');

-- Insert Employees
INSERT INTO payroll.employees (employee_number, first_name, middle_name, last_name, date_of_birth, gender, civil_status, 
    email, mobile_number, address, city, province, postal_code, date_hired, employment_status, employee_type,
    sss_number, philhealth_number, pagibig_number, tin_number, is_active)
SELECT 'EMP001', 'Juan', 'Santos', 'Dela Cruz', '1990-05-15', 'M', 'Single', 
    'juan.delacruz@company.com', '09171234567', '123 Main St', 'Manila', 'Metro Manila', '1000', 
    '2020-01-15', 'Active', 'Regular', '1234567890', '123456789012', '123456789012', '123-456-789-000', true
WHERE NOT EXISTS (SELECT 1 FROM payroll.employees WHERE employee_number = 'EMP001');

INSERT INTO payroll.employees (employee_number, first_name, middle_name, last_name, date_of_birth, gender, civil_status, 
    email, mobile_number, address, city, province, postal_code, date_hired, employment_status, employee_type,
    sss_number, philhealth_number, pagibig_number, tin_number, is_active)
SELECT 'EMP002', 'Maria', 'Garcia', 'Santos', '1992-08-20', 'F', 'Married', 
    'maria.santos@company.com', '09181234567', '456 Side St', 'Quezon City', 'Metro Manila', '1100', 
    '2019-03-10', 'Active', 'Regular', '2345678901', '234567890123', '234567890123', '234-567-890-111', true
WHERE NOT EXISTS (SELECT 1 FROM payroll.employees WHERE employee_number = 'EMP002');

INSERT INTO payroll.employees (employee_number, first_name, middle_name, last_name, date_of_birth, gender, civil_status, 
    email, mobile_number, address, city, province, postal_code, date_hired, employment_status, employee_type,
    sss_number, philhealth_number, pagibig_number, tin_number, is_active)
SELECT 'EMP003', 'Pedro', 'Reyes', 'Aquino', '1988-12-03', 'M', 'Single', 
    'pedro.aquino@company.com', '09191234567', '789 Back St', 'Makati', 'Metro Manila', '1200', 
    '2018-06-01', 'Active', 'Regular', '3456789012', '345678901234', '345678901234', '345-678-901-222', true
WHERE NOT EXISTS (SELECT 1 FROM payroll.employees WHERE employee_number = 'EMP003');

-- Insert Employee Assignments
INSERT INTO payroll.employee_assignments (employee_id, department_id, position_id, is_primary, effective_date)
SELECT 
    e.employee_id,
    d.department_id,
    p.position_id,
    true,
    '2020-01-15'::date
FROM payroll.employees e
CROSS JOIN payroll.departments d
CROSS JOIN payroll.positions p
WHERE e.employee_number = 'EMP001'
  AND d.department_code = 'IT'
  AND p.position_code = 'DEV'
  AND NOT EXISTS (
    SELECT 1 FROM payroll.employee_assignments ea 
    WHERE ea.employee_id = e.employee_id AND ea.is_primary = true
  );

INSERT INTO payroll.employee_assignments (employee_id, department_id, position_id, is_primary, effective_date)
SELECT 
    e.employee_id,
    d.department_id,
    p.position_id,
    true,
    '2019-03-10'::date
FROM payroll.employees e
CROSS JOIN payroll.departments d
CROSS JOIN payroll.positions p
WHERE e.employee_number = 'EMP002'
  AND d.department_code = 'HR'
  AND p.position_code = 'MGR'
  AND NOT EXISTS (
    SELECT 1 FROM payroll.employee_assignments ea 
    WHERE ea.employee_id = e.employee_id AND ea.is_primary = true
  );

INSERT INTO payroll.employee_assignments (employee_id, department_id, position_id, is_primary, effective_date)
SELECT 
    e.employee_id,
    d.department_id,
    p.position_id,
    true,
    '2018-06-01'::date
FROM payroll.employees e
CROSS JOIN payroll.departments d
CROSS JOIN payroll.positions p
WHERE e.employee_number = 'EMP003'
  AND d.department_code = 'ACC'
  AND p.position_code = 'ACC'
  AND NOT EXISTS (
    SELECT 1 FROM payroll.employee_assignments ea 
    WHERE ea.employee_id = e.employee_id AND ea.is_primary = true
  );

-- Insert Employee Compensation
INSERT INTO payroll.employee_compensation (employee_id, salary_grade_id, basic_salary, daily_rate, hourly_rate, 
    pay_frequency, cola, effective_date, is_active)
SELECT 
    e.employee_id,
    sg.salary_grade_id,
    25000.00,
    1136.36,
    142.05,
    'Monthly',
    1000.00,
    '2020-01-15'::date,
    true
FROM payroll.employees e
CROSS JOIN payroll.salary_grades sg
WHERE e.employee_number = 'EMP001'
  AND sg.grade_code = 'SG2'
  AND NOT EXISTS (
    SELECT 1 FROM payroll.employee_compensation ec 
    WHERE ec.employee_id = e.employee_id
  );

INSERT INTO payroll.employee_compensation (employee_id, salary_grade_id, basic_salary, daily_rate, hourly_rate, 
    pay_frequency, cola, effective_date, is_active)
SELECT 
    e.employee_id,
    sg.salary_grade_id,
    35000.00,
    1590.91,
    198.86,
    'Monthly',
    1500.00,
    '2019-03-10'::date,
    true
FROM payroll.employees e
CROSS JOIN payroll.salary_grades sg
WHERE e.employee_number = 'EMP002'
  AND sg.grade_code = 'SG3'
  AND NOT EXISTS (
    SELECT 1 FROM payroll.employee_compensation ec 
    WHERE ec.employee_id = e.employee_id
  );

INSERT INTO payroll.employee_compensation (employee_id, salary_grade_id, basic_salary, daily_rate, hourly_rate, 
    pay_frequency, cola, effective_date, is_active)
SELECT 
    e.employee_id,
    sg.salary_grade_id,
    55000.00,
    2500.00,
    312.50,
    'Monthly',
    2000.00,
    '2018-06-01'::date,
    true
FROM payroll.employees e
CROSS JOIN payroll.salary_grades sg
WHERE e.employee_number = 'EMP003'
  AND sg.grade_code = 'SG4'
  AND NOT EXISTS (
    SELECT 1 FROM payroll.employee_compensation ec 
    WHERE ec.employee_id = e.employee_id
  );

-- Insert Leave Types
INSERT INTO payroll.leave_types (leave_code, leave_name, description, is_paid, is_active) 
SELECT 'SL', 'Sick Leave', 'Sick Leave', true, true
WHERE NOT EXISTS (SELECT 1 FROM payroll.leave_types WHERE leave_code = 'SL');

INSERT INTO payroll.leave_types (leave_code, leave_name, description, is_paid, is_active) 
SELECT 'VL', 'Vacation Leave', 'Vacation Leave', true, true
WHERE NOT EXISTS (SELECT 1 FROM payroll.leave_types WHERE leave_code = 'VL');

INSERT INTO payroll.leave_types (leave_code, leave_name, description, is_paid, is_active) 
SELECT 'EL', 'Emergency Leave', 'Emergency Leave', true, true
WHERE NOT EXISTS (SELECT 1 FROM payroll.leave_types WHERE leave_code = 'EL');

INSERT INTO payroll.leave_types (leave_code, leave_name, description, is_paid, is_active) 
SELECT 'ML', 'Maternity Leave', 'Maternity Leave', true, true
WHERE NOT EXISTS (SELECT 1 FROM payroll.leave_types WHERE leave_code = 'ML');

INSERT INTO payroll.leave_types (leave_code, leave_name, description, is_paid, is_active) 
SELECT 'PL', 'Paternity Leave', 'Paternity Leave', true, true
WHERE NOT EXISTS (SELECT 1 FROM payroll.leave_types WHERE leave_code = 'PL');

-- Insert Allowance Types
INSERT INTO payroll.allowances (allowance_code, allowance_name, description, is_taxable, is_active) 
SELECT 'TA', 'Transportation Allowance', 'Monthly transportation allowance', true, true
WHERE NOT EXISTS (SELECT 1 FROM payroll.allowances WHERE allowance_code = 'TA');

INSERT INTO payroll.allowances (allowance_code, allowance_name, description, is_taxable, is_active) 
SELECT 'MA', 'Meal Allowance', 'Daily meal allowance', true, true
WHERE NOT EXISTS (SELECT 1 FROM payroll.allowances WHERE allowance_code = 'MA');

INSERT INTO payroll.allowances (allowance_code, allowance_name, description, is_taxable, is_active) 
SELECT 'HA', 'Housing Allowance', 'Monthly housing allowance', true, true
WHERE NOT EXISTS (SELECT 1 FROM payroll.allowances WHERE allowance_code = 'HA');

-- Show results
SELECT 'Test data inserted successfully!' AS message;
