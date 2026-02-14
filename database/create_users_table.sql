-- Create users table
CREATE TABLE IF NOT EXISTS payroll.users (
    user_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    username VARCHAR(50) NOT NULL UNIQUE,
    email VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    first_name VARCHAR(100),
    last_name VARCHAR(100),
    role VARCHAR(20) NOT NULL CHECK (role IN ('Admin', 'HR', 'Employee')),
    is_active BOOLEAN DEFAULT TRUE,
    employee_id UUID,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE,
    CONSTRAINT fk_user_employee FOREIGN KEY (employee_id) 
        REFERENCES payroll.employees(employee_id)
);

CREATE INDEX idx_users_username ON payroll.users(username);
CREATE INDEX idx_users_email ON payroll.users(email);
CREATE INDEX idx_users_employee_id ON payroll.users(employee_id);

-- Insert default admin user (password: Admin123)
-- Password hash is SHA256 of 'Admin123'
INSERT INTO payroll.users (user_id, username, email, password_hash, first_name, last_name, role, is_active)
VALUES (
    uuid_generate_v4(),
    'admin',
    'admin@payroll.com',
    'JWxe8B04Wqg/jR+jGZJYlj1SQGqp6SuXWkRZvcN4F8A=',
    'System',
    'Administrator',
    'Admin',
    TRUE
) ON CONFLICT (username) DO NOTHING;

COMMENT ON TABLE payroll.users IS 'System users for authentication and authorization';
COMMENT ON COLUMN payroll.users.role IS 'User role: Admin (full access), HR (payroll management), Employee (self-service)';
