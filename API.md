# API Documentation

Complete API endpoint documentation for JWT POC application.

## Base URL

- **Development**: `http://localhost:5000/api`
- **Production**: `https://your-domain.com/api`

## Authentication

All authenticated endpoints require a Bearer token in the Authorization header:

```http
Authorization: Bearer <your_jwt_token>
```

## Response Format

All endpoints return responses in this format:

```json
{
  "success": true|false,
  "message": "Operation description",
  "data": { ... },
  "errors": [],
  "timestamp": "2024-01-01T12:00:00Z"
}
```

## Authentication Endpoints

### Register User

Create a new user account.

**Endpoint**: `POST /auth/register`

**Auth Required**: No

**Request Body**:
```json
{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+1234567890"
}
```

**Success Response** (201 Created):
```json
{
  "success": true,
  "message": "Registration successful",
  "data": {
    "id": "user-guid",
    "username": "johndoe",
    "email": "john@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "emailConfirmed": false,
    "isActive": true,
    "roles": [{ "name": "User" }]
  }
}
```

### Login

Authenticate and receive tokens.

**Endpoint**: `POST /auth/login`

**Auth Required**: No

**Request Body**:
```json
{
  "usernameOrEmail": "johndoe",
  "password": "SecurePass123!",
  "twoFactorCode": "123456",
  "rememberMe": false
}
```

**Success Response** (200 OK):
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "refresh_token_string",
    "expiresAt": "2024-01-01T12:15:00Z",
    "requiresTwoFactor": false,
    "user": {
      "id": "user-guid",
      "username": "johndoe",
      "email": "john@example.com",
      "roles": [{ "name": "User" }],
      "permissions": ["profile.read", "profile.update"]
    }
  }
}
```

**Error Responses**:
- **401 Unauthorized**: Invalid credentials
- **423 Locked**: Account is locked
- **400 Bad Request**: Validation errors

### Refresh Token

Obtain new access token using refresh token.

**Endpoint**: `POST /auth/refresh`

**Auth Required**: No

**Request Body**:
```json
{
  "accessToken": "current_access_token",
  "refreshToken": "current_refresh_token"
}
```

**Success Response** (200 OK):
```json
{
  "success": true,
  "message": "Token refreshed successfully",
  "data": {
    "accessToken": "new_access_token",
    "refreshToken": "new_refresh_token",
    "expiresAt": "2024-01-01T12:30:00Z"
  }
}
```

### Logout

Revoke user tokens.

**Endpoint**: `POST /auth/logout`

**Auth Required**: Yes

**Request Body**:
```json
{
  "logoutFromAllDevices": false
}
```

**Success Response** (200 OK):
```json
{
  "success": true,
  "message": "Logged out successfully"
}
```

### Change Password

Update user password.

**Endpoint**: `POST /auth/change-password`

**Auth Required**: Yes

**Request Body**:
```json
{
  "currentPassword": "OldPass123!",
  "newPassword": "NewPass123!",
  "confirmPassword": "NewPass123!"
}
```

**Success Response** (200 OK):
```json
{
  "success": true,
  "message": "Password changed successfully. Please login again."
}
```

### Get Current User

Retrieve authenticated user information.

**Endpoint**: `GET /auth/me`

**Auth Required**: Yes

**Success Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "id": "user-guid",
    "username": "johndoe",
    "email": "john@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "roles": [
      { "name": "User", "description": "Standard user" }
    ],
    "permissions": ["profile.read", "profile.update", "reports.read"]
  }
}
```

## Two-Factor Authentication

### Setup 2FA

Initialize two-factor authentication.

**Endpoint**: `POST /auth/2fa/setup`

**Auth Required**: Yes

**Request Body**:
```json
{
  "method": "Authenticator"
}
```

**Success Response** (200 OK):
```json
{
  "success": true,
  "message": "2FA setup initiated",
  "data": {
    "qrCodeUrl": "otpauth://totp/JwtPoc:user@example.com?secret=SECRET&issuer=JwtPoc",
    "manualEntryKey": "JBSWY3DPEHPK3PXP",
    "backupCodes": [
      "1A2B-3C4D",
      "5E6F-7G8H",
      "9I0J-1K2L"
    ]
  }
}
```

### Verify and Enable 2FA

Confirm 2FA setup with verification code.

**Endpoint**: `POST /auth/2fa/verify`

**Auth Required**: Yes

**Request Body**:
```json
{
  "code": "123456"
}
```

**Success Response** (200 OK):
```json
{
  "success": true,
  "message": "Two-factor authentication enabled successfully"
}
```

### Disable 2FA

Turn off two-factor authentication.

**Endpoint**: `POST /auth/2fa/disable`

**Auth Required**: Yes

**Request Body**:
```json
{
  "password": "UserPass123!"
}
```

**Success Response** (200 OK):
```json
{
  "success": true,
  "message": "Two-factor authentication disabled"
}
```

## Demo Endpoints (Authorization Examples)

### Public Data

Accessible to all authenticated users.

**Endpoint**: `GET /demo/public`

**Auth Required**: Yes (any role)

**Success Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "message": "Hello johndoe! This is accessible to all authenticated users.",
    "timestamp": "2024-01-01T12:00:00Z"
  }
}
```

### User-Only Data

Accessible to User role and above.

**Endpoint**: `GET /demo/user-only`

**Auth Required**: Yes (Roles: User, Manager, Admin, SuperAdmin)

**Success Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "message": "This data is for User role and above",
    "your_roles": ["User"],
    "data": "User-level sensitive information"
  }
}
```

### Manager-Only Data

Accessible to Manager role and above.

**Endpoint**: `GET /demo/manager-only`

**Auth Required**: Yes (Roles: Manager, Admin, SuperAdmin)

**Success Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "message": "Manager-level access granted",
    "data": {
      "team_reports": ["Report 1", "Report 2"],
      "statistics": { "total_users": 150, "active_projects": 12 }
    }
  }
}
```

### Admin-Only Data

Accessible to Admin role and above.

**Endpoint**: `GET /demo/admin-only`

**Auth Required**: Yes (Roles: Admin, SuperAdmin)

**Success Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "message": "Administrator access granted",
    "data": {
      "system_config": { "database_status": "Healthy" },
      "pending_approvals": 5
    }
  }
}
```

### Reports (Permission-Based)

Accessible based on permissions.

**Endpoint**: `GET /demo/reports`

**Auth Required**: Yes (Permission: users.read)

**Success Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "message": "Reports accessible based on permissions",
    "your_permissions": ["users.read", "reports.read"],
    "reports": [
      { "id": 1, "name": "Monthly Sales", "status": "Ready" },
      { "id": 2, "name": "User Analytics", "status": "Processing" }
    ]
  }
}
```

### My Access Information

View current user's authorization context.

**Endpoint**: `GET /demo/my-access`

**Auth Required**: Yes

**Success Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "user_id": "user-guid",
    "username": "johndoe",
    "email": "john@example.com",
    "roles": ["User", "Manager"],
    "permissions": ["users.read", "reports.read", "reports.create"],
    "all_claims": [
      { "type": "sub", "value": "user-guid" },
      { "type": "email", "value": "john@example.com" },
      { "type": "role", "value": "User" },
      { "type": "permission", "value": "users.read" }
    ]
  }
}
```

## Error Responses

### Standard Error Format

```json
{
  "success": false,
  "message": "Error description",
  "errors": [
    "Specific error 1",
    "Specific error 2"
  ],
  "timestamp": "2024-01-01T12:00:00Z"
}
```

### HTTP Status Codes

| Code | Meaning | When Used |
|------|---------|-----------|
| 200 | OK | Successful GET/POST/PUT |
| 201 | Created | Successful resource creation |
| 400 | Bad Request | Validation errors |
| 401 | Unauthorized | Invalid/missing authentication |
| 403 | Forbidden | Insufficient permissions |
| 404 | Not Found | Resource doesn't exist |
| 409 | Conflict | Duplicate resource (e.g., username exists) |
| 422 | Unprocessable Entity | Business logic error |
| 423 | Locked | Account is locked |
| 429 | Too Many Requests | Rate limit exceeded |
| 500 | Internal Server Error | Unexpected server error |

## Swagger/OpenAPI

Interactive API documentation is available at:

**Development**: `http://localhost:5000/swagger`

Use Swagger UI to:
- View all endpoints
- Test endpoints directly
- View request/response schemas
- Authenticate with JWT tokens

## Testing with cURL

### Login Example

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "user",
    "password": "User@123"
  }'
```

### Authenticated Request Example

```bash
curl -X GET http://localhost:5000/api/auth/me \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"
```

## Rate Limiting

**Default Limits**:
- 100 requests per minute per IP
- 429 status code when exceeded
- Retry-After header included

**Headers**:
```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1640000000
```

---

For more details, see the [Swagger UI documentation](http://localhost:5000/swagger) or contact support@example.com.
