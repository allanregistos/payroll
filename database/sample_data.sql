-- Sample data insertion script for testing
-- Philippine Payroll System

SET search_path TO payroll, public;

-- Enable UUID extension if not already enabled
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- ============================================
-- SAMPLE DEPARTMENTS
-- ============================================
INSERT INTO departments (department_code, department_name, description) VALUES
('IT', 'Information Technology', 'IT and Development Team'),
('HR', 'Human Resources', 'Human Resources Department'),
('FIN', 'Finance', 'Finance and Accounting'),
('OPS', 'Operations', 'Operations Department'),
('SALES', 'Sales and Marketing', 'Sales and Marketing Team');

-- ============================================
-- SAMPLE POSITIONS
-- ============================================
INSERT INTO positions (position_code, position_title, description) VALUES
('DEV01', 'Software Developer', 'Application Developer'),
('DEV02', 'Senior Developer', 'Senior Software Developer'),
('MGR01', 'Department Manager', 'Department Head'),
('HR01', 'HR Specialist', 'Human Resources Specialist'),
('ACC01', 'Accountant', 'Staff Accountant'),
('SALES01', 'Sales Representative', 'Sales Agent');

-- ============================================
-- SAMPLE SALARY GRADES
-- ============================================
INSERT INTO salary_grades (grade_code, grade_name, min_salary, max_salary) VALUES
('SG1', 'Salary Grade 1', 15000.00, 20000.00),
('SG2', 'Salary Grade 2', 20000.00, 30000.00),
('SG3', 'Salary Grade 3', 30000.00, 45000.00),
('SG4', 'Salary Grade 4', 45000.00, 60000.00),
('SG5', 'Salary Grade 5', 60000.00, 80000.00);

-- ============================================
-- SAMPLE SSS CONTRIBUTION TABLE (2024 rates - simplified)
-- ============================================
INSERT INTO sss_contribution_table (min_salary, max_salary, employee_contribution, employer_contribution, total_contribution, ec_contribution, effective_date, is_active) VALUES
(0.00, 4249.99, 180.00, 380.00, 560.00, 10.00, '2024-01-01', TRUE),
(4250.00, 4749.99, 191.25, 403.75, 595.00, 10.00, '2024-01-01', TRUE),
(4750.00, 5249.99, 202.50, 427.50, 630.00, 10.00, '2024-01-01', TRUE),
(5250.00, 5749.99, 213.75, 451.25, 665.00, 10.00, '2024-01-01', TRUE),
(5750.00, 6249.99, 225.00, 475.00, 700.00, 10.00, '2024-01-01', TRUE),
(15000.00, 15749.99, 607.50, 1282.50, 1890.00, 10.00, '2024-01-01', TRUE),
(20000.00, 20749.99, 810.00, 1710.00, 2520.00, 10.00, '2024-01-01', TRUE),
(25000.00, 25749.99, 1012.50, 2137.50, 3150.00, 10.00, '2024-01-01', TRUE),
(30000.00, 999999.99, 1125.00, 2375.00, 3500.00, 10.00, '2024-01-01', TRUE);

-- ============================================
-- SAMPLE PHILHEALTH CONTRIBUTION TABLE (2024)
-- ============================================
INSERT INTO philhealth_contribution_table (min_salary, max_salary, premium_rate, employee_share, employer_share, effective_date, is_active) VALUES
(10000.00, 99999.99, 0.05, 0.50, 0.50, '2024-01-01', TRUE),
(100000.00, NULL, 0.05, 0.50, 0.50, '2024-01-01', TRUE);

-- Note: 5% premium, max ₱5,000 (based on ₱100,000 ceiling)

-- ============================================
-- SAMPLE PAG-IBIG CONTRIBUTION TABLE (2024)
-- ============================================
INSERT INTO pagibig_contribution_table (min_salary, max_salary, employee_rate, employer_rate, max_employee_contribution, max_employer_contribution, effective_date, is_active) VALUES
(1000.00, 1500.00, 0.01, 0.02, NULL, NULL, '2024-01-01', TRUE),
(1500.01, 999999.99, 0.02, 0.02, 100.00, 100.00, '2024-01-01', TRUE);

-- ============================================
-- SAMPLE TAX TABLE (TRAIN Law - Monthly)
-- ============================================
INSERT INTO tax_table (min_compensation, max_compensation, base_tax, tax_rate, excess_over, period_type, effective_date, is_active) VALUES
(0.00, 20833.00, 0.00, 0.00, 0.00, 'Monthly', '2018-01-01', TRUE),
(20833.01, 33332.00, 0.00, 0.15, 20833.00, 'Monthly', '2018-01-01', TRUE),
(33332.01, 66666.00, 1875.00, 0.20, 33332.00, 'Monthly', '2018-01-01', TRUE),
(66666.01, 166666.00, 8541.80, 0.25, 66666.00, 'Monthly', '2018-01-01', TRUE),
(166666.01, 666666.00, 33541.80, 0.30, 166666.00, 'Monthly', '2018-01-01', TRUE),
(666666.01, NULL, 183541.80, 0.35, 666666.00, 'Monthly', '2018-01-01', TRUE);

-- ============================================
-- SAMPLE EMPLOYEES
-- ============================================
INSERT INTO employees (
    employee_id, employee_number, first_name, middle_name, last_name, 
    date_of_birth, gender, civil_status,
    email, mobile_number, address, city, province,
    date_hired, employment_status, employee_type,
    sss_number, philhealth_number, pagibig_number, tin_number
) VALUES
(
    'e8c8f6a3-1234-4567-89ab-111111111111'::uuid, 'EMP001', 'Juan', 'Santos', 'Dela Cruz',
    '1990-05-15', 'M', 'Married',
    'juan.delacruz@company.com', '+63917-123-4567', 
    '123 Main St., Brgy. San Antonio', 'Quezon City', 'Metro Manila',
    '2020-01-15', 'Active', 'Regular',
    '34-1234567-8', '12-345678901-2', '121234567890', '123-456-789-000'
),
(
    'e8c8f6a3-1234-4567-89ab-222222222222'::uuid, 'EMP002', 'Maria', 'Garcia', 'Santos',
    '1992-08-20', 'F', 'Single',
    'maria.santos@company.com', '+63917-234-5678',
    '456 Rizal Ave., Brgy. Poblacion', 'Makati City', 'Metro Manila',
    '2021-03-01', 'Active', 'Regular',
    '34-2345678-9', '12-456789012-3', '121234567891', '234-567-890-000'
),
(
    'e8c8f6a3-1234-4567-89ab-333333333333'::uuid, 'EMP003', 'Pedro', 'Reyes', 'Ramos',
    '1988-12-10', 'M', 'Married',
    'pedro.ramos@company.com', '+63917-345-6789',
    '789 Bonifacio St., Brgy. San Isidro', 'Pasig City', 'Metro Manila',
    '2019-06-15', 'Active', 'Regular',
    '34-3456789-0', '12-567890123-4', '121234567892', '345-678-901-000'
);

-- ============================================
-- EMPLOYEE ASSIGNMENTS
-- ============================================
INSERT INTO employee_assignments (employee_id, department_id, position_id, is_primary, effective_date) 
VALUES
('e8c8f6a3-1234-4567-89ab-111111111111'::uuid, 1, 2, TRUE, '2020-01-15'), -- Juan - IT - Senior Developer
('e8c8f6a3-1234-4567-89ab-222222222222'::uuid, 2, 4, TRUE, '2021-03-01'), -- Maria - HR - HR Specialist
('e8c8f6a3-1234-4567-89ab-333333333333'::uuid, 3, 5, TRUE, '2019-06-15'); -- Pedro - Finance - Accountant

-- ============================================
-- EMPLOYEE COMPENSATION
-- ============================================
INSERT INTO employee_compensation (
    employee_id, salary_grade_id, basic_salary, daily_rate, hourly_rate,
    pay_frequency, cola, effective_date, is_active
) VALUES
('e8c8f6a3-1234-4567-89ab-111111111111'::uuid, 4, 45000.00, 2073.39, 259.17, 'Monthly', 0.00, '2020-01-15', TRUE),
('e8c8f6a3-1234-4567-89ab-222222222222'::uuid, 3, 30000.00, 1382.26, 172.78, 'Monthly', 0.00, '2021-03-01', TRUE),
('e8c8f6a3-1234-4567-89ab-333333333333'::uuid, 3, 35000.00, 1612.64, 201.58, 'Monthly', 0.00, '2019-06-15', TRUE);

-- ============================================
-- EMPLOYEE ALLOWANCES
-- ============================================
INSERT INTO employee_allowances (employee_id, allowance_id, amount, frequency, effective_date, is_active)
VALUES
('e8c8f6a3-1234-4567-89ab-111111111111'::uuid, 1, 2000.00, 'Monthly', '2020-01-15', TRUE), -- Transportation
('e8c8f6a3-1234-4567-89ab-111111111111'::uuid, 2, 1500.00, 'Monthly', '2020-01-15', TRUE), -- Meal
('e8c8f6a3-1234-4567-89ab-222222222222'::uuid, 1, 1500.00, 'Monthly', '2021-03-01', TRUE), -- Transportation
('e8c8f6a3-1234-4567-89ab-333333333333'::uuid, 1, 1500.00, 'Monthly', '2019-06-15', TRUE); -- Transportation

-- ============================================
-- SAMPLE PAYROLL PERIOD
-- ============================================
INSERT INTO payroll_periods (
    period_name, period_type, period_start_date, period_end_date, 
    payment_date, cutoff_date, status
) VALUES
(
    'January 2026 - 1st Half', 'Semi-Monthly', 
    '2026-01-01', '2026-01-15', '2026-01-17', 
    '2026-01-10', 'Draft'
);

-- ============================================
-- SAMPLE ATTENDANCE RECORDS
-- ============================================
INSERT INTO attendance (
    employee_id, attendance_date, time_in, time_out,
    break_out, break_in, regular_hours, overtime_hours,
    late_minutes, undertime_minutes, is_holiday, is_rest_day
) VALUES
-- Juan's attendance (first 10 days of January 2026)
('e8c8f6a3-1234-4567-89ab-111111111111'::uuid, '2026-01-02', '2026-01-02 08:00:00', '2026-01-02 17:00:00', '2026-01-02 12:00:00', '2026-01-02 13:00:00', 8.0, 0.0, 0, 0, FALSE, FALSE),
('e8c8f6a3-1234-4567-89ab-111111111111'::uuid, '2026-01-03', '2026-01-03 08:00:00', '2026-01-03 17:00:00', '2026-01-03 12:00:00', '2026-01-03 13:00:00', 8.0, 0.0, 0, 0, FALSE, FALSE),
('e8c8f6a3-1234-4567-89ab-111111111111'::uuid, '2026-01-05', '2026-01-05 08:00:00', '2026-01-05 17:00:00', '2026-01-05 12:00:00', '2026-01-05 13:00:00', 8.0, 0.0, 0, 0, FALSE, FALSE),
('e8c8f6a3-1234-4567-89ab-111111111111'::uuid, '2026-01-06', '2026-01-06 08:00:00', '2026-01-06 17:00:00', '2026-01-06 12:00:00', '2026-01-06 13:00:00', 8.0, 0.0, 0, 0, FALSE, FALSE),
('e8c8f6a3-1234-4567-89ab-111111111111'::uuid, '2026-01-07', '2026-01-07 08:00:00', '2026-01-07 17:00:00', '2026-01-07 12:00:00', '2026-01-07 13:00:00', 8.0, 0.0, 0, 0, FALSE, FALSE),
('e8c8f6a3-1234-4567-89ab-111111111111'::uuid, '2026-01-08', '2026-01-08 08:00:00', '2026-01-08 17:00:00', '2026-01-08 12:00:00', '2026-01-08 13:00:00', 8.0, 0.0, 0, 0, FALSE, FALSE),
('e8c8f6a3-1234-4567-89ab-111111111111'::uuid, '2026-01-09', '2026-01-09 08:00:00', '2026-01-09 17:00:00', '2026-01-09 12:00:00', '2026-01-09 13:00:00', 8.0, 0.0, 0, 0, FALSE, FALSE),

-- Maria's attendance
('e8c8f6a3-1234-4567-89ab-222222222222'::uuid, '2026-01-02', '2026-01-02 08:00:00', '2026-01-02 17:00:00', '2026-01-02 12:00:00', '2026-01-02 13:00:00', 8.0, 0.0, 0, 0, FALSE, FALSE),
('e8c8f6a3-1234-4567-89ab-222222222222'::uuid, '2026-01-03', '2026-01-03 08:00:00', '2026-01-03 17:00:00', '2026-01-03 12:00:00', '2026-01-03 13:00:00', 8.0, 0.0, 0, 0, FALSE, FALSE),
('e8c8f6a3-1234-4567-89ab-222222222222'::uuid, '2026-01-05', '2026-01-05 08:00:00', '2026-01-05 17:00:00', '2026-01-05 12:00:00', '2026-01-05 13:00:00', 8.0, 0.0, 0, 0, FALSE, FALSE);

-- ============================================
-- SAMPLE USER ACCOUNTS
-- ============================================
-- Note: In production, use proper password hashing (bcrypt, etc.)
INSERT INTO users (user_id, username, password_hash, email, employee_id, is_active) VALUES
('f9d9f7b4-5678-4567-89ab-111111111111'::uuid, 'admin', '$2a$11$hashed_password_here', 'admin@company.com', NULL, TRUE),
('f9d9f7b4-5678-4567-89ab-222222222222'::uuid, 'jdelacruz', '$2a$11$hashed_password_here', 'juan.delacruz@company.com', 'e8c8f6a3-1234-4567-89ab-111111111111'::uuid, TRUE),
('f9d9f7b4-5678-4567-89ab-333333333333'::uuid, 'msantos', '$2a$11$hashed_password_here', 'maria.santos@company.com', 'e8c8f6a3-1234-4567-89ab-222222222222'::uuid, TRUE);

-- Assign roles
INSERT INTO user_roles (user_id, role_id) VALUES
('f9d9f7b4-5678-4567-89ab-111111111111'::uuid, 1), -- admin - Administrator
('f9d9f7b4-5678-4567-89ab-222222222222'::uuid, 5), -- jdelacruz - Employee
('f9d9f7b4-5678-4567-89ab-333333333333'::uuid, 5); -- msantos - Employee

COMMENT ON TABLE employees IS 'Contains sample employee records for testing';
