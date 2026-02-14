-- Add is_active column to employees table
ALTER TABLE payroll.employees 
ADD COLUMN IF NOT EXISTS is_active BOOLEAN DEFAULT TRUE;

-- Update existing records to be active
UPDATE payroll.employees 
SET is_active = TRUE 
WHERE is_active IS NULL;

-- Create index for better query performance
CREATE INDEX IF NOT EXISTS idx_employees_is_active ON payroll.employees(is_active);
