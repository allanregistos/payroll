-- Grant permissions to phpayroll user on payroll schema
-- Run this as postgres superuser in pgAdmin

-- Grant usage on schema
GRANT USAGE ON SCHEMA payroll TO phpayroll;

-- Grant SELECT, INSERT, UPDATE, DELETE on all existing tables
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA payroll TO phpayroll;

-- Grant permissions on sequences
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA payroll TO phpayroll;

-- Grant permissions on future tables (important!)
ALTER DEFAULT PRIVILEGES IN SCHEMA payroll 
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO phpayroll;

ALTER DEFAULT PRIVILEGES IN SCHEMA payroll 
GRANT USAGE, SELECT ON SEQUENCES TO phpayroll;

-- Verify permissions
SELECT 
    grantee, 
    privilege_type 
FROM information_schema.role_table_grants 
WHERE grantee = 'phpayroll' 
  AND table_schema = 'payroll'
ORDER BY table_name, privilege_type;
