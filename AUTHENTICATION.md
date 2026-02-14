# Authentication & Authorization Setup

## Overview

The Philippine Payroll System now includes JWT-based authentication and role-based access control.

## Roles

- **Admin**: Full system access (can create users, manage all data)
- **HR**: Payroll and employee management (cannot create users)
- **Employee**: Self-service access (view own records, submit leave requests)

## API Endpoints

### Authentication

#### Login
```
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin123"
}

Response:
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "admin",
  "email": "admin@payroll.com",
  "role": "Admin",
  "fullName": "System Administrator",
  "employeeId": null
}
```

#### Register New User (Admin only)
```
POST /api/auth/register
Authorization: Bearer {token}
Content-Type: application/json

{
  "username": "hruser",
  "email": "hr@company.com",
  "password": "Password123",
  "firstName": "Jane",
  "lastName": "Doe",
  "role": "HR",
  "employeeId": null
}
```

#### Get Profile
```
GET /api/auth/profile
Authorization: Bearer {token}
```

#### Change Password
```
POST /api/auth/change-password
Authorization: Bearer {token}
Content-Type: application/json

{
  "currentPassword": "OldPassword123",
  "newPassword": "NewPassword123"
}
```

## Protected Endpoints

### Admin & HR Access
- `GET /api/employees` - List all employees
- `POST /api/employees` - Create employee
- `PUT /api/employees/{id}` - Update employee
- `DELETE /api/employees/{id}` - Delete employee
- `GET /api/attendance` - View attendance records
- `POST /api/attendance` - Record attendance
- `GET /api/payroll/periods` - List payroll periods
- `POST /api/payroll/compute` - Process payroll

### All Authenticated Users
- `GET /api/leaves` - View leave requests
- `POST /api/leaves` - Submit leave request
- `GET /api/auth/profile` - Get own profile
- `POST /api/auth/change-password` - Change own password

## Using JWT Tokens

### In Swagger UI
1. Click the "Authorize" button at the top
2. Enter: `Bearer {your-token-here}`
3. Click "Authorize"

### In HTTP Requests
Add the Authorization header to all protected endpoints:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Database Setup

Run the users table creation script:
```bash
psql -d phpayroll -U postgres -f database/create_users_table.sql
```

This creates:
- `payroll.users` table
- Default admin user (username: `admin`, password: `Admin123`)

## Default Credentials

**Username**: `admin`  
**Password**: `Admin123`

⚠️ **Important**: Change the default admin password immediately after first login!

## JWT Configuration

Edit `appsettings.json`:
```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "PayrollAPI",
    "Audience": "PayrollAPIUsers",
    "ExpiryMinutes": "480"
  }
}
```

⚠️ **Security**: In production, store the `SecretKey` in environment variables or Azure Key Vault, NOT in appsettings.json!

## Password Security

Currently using SHA256 hashing. For production, consider upgrading to:
- BCrypt
- Argon2
- PBKDF2

## Testing Authentication

1. Start the API:
```bash
cd src/Payroll.API
dotnet run
```

2. Navigate to: http://localhost:5189/swagger

3. Test login:
   - Call `POST /api/auth/login`
   - Use credentials: admin / Admin123
   - Copy the returned token

4. Authorize:
   - Click "Authorize" button
   - Paste token with "Bearer " prefix
   - Test protected endpoints

## Role-Based Authorization Examples

### Controller Level
```csharp
[Authorize(Roles = "Admin,HR")]
public class EmployeesController : ControllerBase
{
    // All actions require Admin or HR role
}
```

### Action Level
```csharp
[HttpGet]
[Authorize] // Any authenticated user
public async Task<IActionResult> GetLeaves()

[HttpPost]
[Authorize(Roles = "Admin")] // Only admin
public async Task<IActionResult> CreateUser()
```

## Next Steps

1. ✅ JWT authentication implemented
2. ✅ Role-based authorization added
3. ✅ Protected endpoints secured
4. ⏳ Create users table in database (manual step due to permissions)
5. ⏳ Test with Swagger UI
6. 📋 Future: Add refresh tokens
7. 📋 Future: Implement password reset via email
8. 📋 Future: Add audit logging for authentication events

## Troubleshooting

### "Unauthorized" on protected endpoints
- Ensure you've included the JWT token in the Authorization header
- Verify token hasn't expired (480 minutes = 8 hours by default)
- Check if your role has access to the endpoint

### "Invalid username or password"
- Verify credentials are correct
- Check if user exists in database: `SELECT * FROM payroll.users;`
- Ensure user.is_active = true

### Database permission denied
- Run the create_users_table.sql script as postgres superuser
- Grant necessary permissions to phpayroll user
