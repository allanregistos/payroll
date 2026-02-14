-- Add missing columns to existing users table
ALTER TABLE payroll.users 
ADD COLUMN IF NOT EXISTS first_name VARCHAR(100),
ADD COLUMN IF NOT EXISTS last_name VARCHAR(100),
ADD COLUMN IF NOT EXISTS role VARCHAR(20) CHECK (role IN ('Admin', 'HR', 'Employee'));

-- Set default role for existing users (if any)
UPDATE payroll.users 
SET role = 'Admin' 
WHERE role IS NULL;

-- Make role NOT NULL after setting defaults
ALTER TABLE payroll.users 
ALTER COLUMN role SET NOT NULL;

-- Insert default admin user if not exists (password: Admin123)
INSERT INTO payroll.users (user_id, username, email, password_hash, first_name, last_name, role, is_active)
VALUES (
    gen_random_uuid(),
    'admin',
    'admin@payroll.com',
    'O2Esdae1BIpDX7bsgeUv+S1teVqLWpwXBw9qY8l6U7I=',
    'System',
    'Administrator',
    'Admin',
    TRUE
) ON CONFLICT (username) DO NOTHING;

COMMENT ON COLUMN payroll.users.role IS 'User role: Admin (full access), HR (payroll management), Employee (self-service)';
