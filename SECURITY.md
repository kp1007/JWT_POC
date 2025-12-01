# Security Documentation

## Table of Contents

- [Security Overview](#security-overview)
- [Authentication Security](#authentication-security)
- [Authorization Security](#authorization-security)
- [Token Security](#token-security)
- [Password Security](#password-security)
- [Two-Factor Authentication](#two-factor-authentication)
- [Brute Force Protection](#brute-force-protection)
- [Security Headers](#security-headers)
- [Audit Logging](#audit-logging)
- [Security Best Practices](#security-best-practices)
- [Threat Mitigation](#threat-mitigation)
- [Security Checklist](#security-checklist)

## Security Overview

This application implements multiple layers of security following industry best practices and OWASP guidelines. The security approach follows the **Defense in Depth** principle with multiple overlapping security controls.

### Security Layers

```
┌─────────────────────────────────────────────────┐
│ Layer 7: Audit & Monitoring                     │
├─────────────────────────────────────────────────┤
│ Layer 6: Application Security (Rate Limiting)   │
├─────────────────────────────────────────────────┤
│ Layer 5: Authorization (RBAC/Permissions)       │
├─────────────────────────────────────────────────┤
│ Layer 4: Authentication (JWT/2FA)               │
├─────────────────────────────────────────────────┤
│ Layer 3: Input Validation                       │
├─────────────────────────────────────────────────┤
│ Layer 2: Transport Security (HTTPS/TLS)         │
├─────────────────────────────────────────────────┤
│ Layer 1: Network Security (Firewall/CORS)       │
└─────────────────────────────────────────────────┘
```

## Authentication Security

### JWT Implementation

**Algorithm**: HS256 (HMAC-SHA256)
- Symmetric signing algorithm
- Requires shared secret key
- Industry-standard for server-to-server auth

**Token Structure**:
```json
{
  "header": {
    "alg": "HS256",
    "typ": "JWT"
  },
  "payload": {
    "sub": "user-id-guid",
    "email": "user@example.com",
    "username": "username",
    "roles": ["User", "Manager"],
    "permissions": ["users.read", "reports.create"],
    "security_stamp": "stamp-value",
    "jti": "unique-token-id",
    "exp": 1234567890,
    "iss": "JwtPocApi",
    "aud": "JwtPocClient"
  },
  "signature": "signed-with-secret-key"
}
```

### Security Features

#### 1. **Short-Lived Access Tokens**
- **Default Expiration**: 15 minutes
- **Rationale**: Limits damage if token is compromised
- **Configuration**: Adjustable via `Jwt:AccessTokenExpirationMinutes`

```csharp
"Jwt": {
  "AccessTokenExpirationMinutes": 15  // Recommended: 15-60 minutes
}
```

#### 2. **Token Blacklisting**

**Purpose**: Immediate token invalidation

**Implementation**:
```csharp
// Blacklist token on logout
await _blacklistService.BlacklistTokenAsync(
    jti: jwtId,
    userId: userId,
    expiresAt: tokenExpiration,
    reason: "User logout",
    ipAddress: clientIp
);

// Check if token is blacklisted (middleware)
if (await _blacklistService.IsTokenBlacklistedAsync(jti))
{
    context.Fail("Token has been revoked");
}
```

**Use Cases**:
- User logout
- Password change
- Account compromise
- Administrative revocation

#### 3. **Security Stamp Validation**

**Purpose**: Automatically invalidate tokens when credentials change

**How it works**:
1. Each user has a unique `SecurityStamp` (GUID)
2. SecurityStamp is embedded in JWT
3. SecurityStamp changes when password changes
4. Middleware validates SecurityStamp matches current value

```csharp
// In JWT
"security_stamp": "current-user-security-stamp"

// Validation
var tokenStamp = GetSecurityStamp(token);
var currentStamp = user.SecurityStamp;

if (tokenStamp != currentStamp)
{
    // Token is invalid - credentials changed
    return Unauthorized();
}
```

### Refresh Token Security

**Purpose**: Long-lived tokens for obtaining new access tokens

#### Token Rotation

**Flow**:
```
Client uses refresh token
    ↓
Server validates token
    ↓
Server generates new access + refresh tokens
    ↓
Server REVOKES old refresh token
    ↓
Client receives new tokens
```

**Benefits**:
- Prevents refresh token reuse
- Limits exposure window
- Detects token theft

#### Token Family Validation

**Purpose**: Detect token replay attacks

**Implementation**:
```
Token Family: root-token-id
    ├─ Token 1 (original)
    ├─ Token 2 (rotated from Token 1)
    ├─ Token 3 (rotated from Token 2)
    └─ Token 4 (rotated from Token 3)
```

**Security Check**:
```csharp
// If a revoked token in the family is reused:
if (token.IsRevoked && token.TokenFamily != null)
{
    // SECURITY BREACH DETECTED
    // Revoke entire token family
    await RevokeTokenFamilyAsync(token.TokenFamily);

    // Log security event
    await LogSecurityEventAsync("Token replay detected", userId);

    // Optionally: Lock account
    await LockAccountAsync(userId);
}
```

## Authorization Security

### Role-Based Access Control (RBAC)

**Hierarchical Roles**:
```
SuperAdmin (1000) ──┐
                    ├─ Full access
Admin (100) ────────┤
                    ├─ Management access
Manager (50) ───────┤
                    ├─ Team access
User (1) ───────────┤
                    ├─ Standard access
Guest (0) ──────────┘
                    └─ Read-only access
```

**Role Comparison**:
```csharp
// Higher role levels inherit lower role permissions
public bool CanAccessResource(int requiredLevel, int userLevel)
{
    return userLevel >= requiredLevel;
}
```

### Permission-Based Authorization

**Fine-Grained Control**:
```csharp
// Check specific permission
[Authorize(Policy = "CanManageUsers")]
public async Task<IActionResult> DeleteUser(Guid id)
{
    // Only users with "users.delete" permission can access
}
```

**Permission Naming Convention**:
```
resource.action

Examples:
- users.read
- users.create
- users.update
- users.delete
- reports.read
- reports.create
```

## Password Security

### BCrypt Hashing

**Algorithm**: BCrypt
**Work Factor**: 12 (adjustable)
**Salt**: Automatically generated and embedded in hash

**Why BCrypt?**
- ✅ Designed for password hashing
- ✅ Adaptive - work factor can increase over time
- ✅ Includes salt automatically
- ✅ Resistant to rainbow table attacks
- ✅ Slow by design (prevents brute force)

**Work Factor Comparison**:
```
Work Factor | Hash Time | Security Level
------------|-----------|---------------
10          | ~100ms    | Minimum
12          | ~400ms    | Recommended (Default)
14          | ~1.6s     | High security
16          | ~6.4s     | Maximum
```

**Implementation**:
```csharp
// Hashing (registration/password change)
var hash = BCrypt.HashPassword(password, workFactor: 12);

// Verification (login)
var isValid = BCrypt.Verify(password, hash);

// Rehashing when work factor increases
if (BCrypt.PasswordNeedsRehash(hash, newWorkFactor))
{
    var newHash = BCrypt.HashPassword(password, newWorkFactor);
    await UpdatePasswordHashAsync(userId, newHash);
}
```

### Password Complexity Requirements

**Default Requirements**:
- ✅ Minimum 8 characters
- ✅ At least one uppercase letter (A-Z)
- ✅ At least one lowercase letter (a-z)
- ✅ At least one digit (0-9)
- ✅ At least one special character (!@#$%^&*)

**Validation**:
```csharp
[RegularExpression(
    @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]",
    ErrorMessage = "Password must contain uppercase, lowercase, number, and special character"
)]
```

### Password Storage Security

**Never stored in plain text**:
```
❌ Plain text: MyPassword123
❌ Simple hash: 5f4dcc3b5aa765d61d8327deb882cf99
✅ BCrypt hash: $2a$12$eImiTXuWVxfM37uY4JANjQ.a9MZvJ6eY3.6TpQlPc8s/4BvGqB3va
```

## Two-Factor Authentication

### TOTP (Time-based One-Time Password)

**Algorithm**: RFC 6238 TOTP
**Time Step**: 30 seconds
**Code Length**: 6 digits
**Compatible Apps**: Google Authenticator, Authy, Microsoft Authenticator

**Setup Flow**:
```
1. User enables 2FA
    ↓
2. Server generates secret key
    ↓
3. Server generates QR code URL
    ↓
4. User scans QR code with authenticator app
    ↓
5. User enters verification code
    ↓
6. Server validates code
    ↓
7. 2FA is enabled
    ↓
8. User receives backup codes
```

**Security Features**:

#### 1. **Secret Key Security**
- 160-bit random secret (Base32 encoded)
- Stored encrypted in database
- Never transmitted except during setup

#### 2. **Time Window Tolerance**
- Accepts codes from: previous, current, and next time steps
- Total window: 90 seconds (±30 seconds)
- Accounts for clock drift between server/client

```csharp
// Verification with window
var totp = new Totp(secretBytes, step: 30, totpSize: 6);
var isValid = totp.VerifyTotp(code, out _, new VerificationWindow(1, 1));
```

#### 3. **Backup Codes**

**Purpose**: Account recovery when 2FA device is unavailable

**Generation**:
```
Format: XXXX-XXXX (8 characters, hyphenated)
Count: 10 codes
Storage: BCrypt hashed (like passwords)
Usage: Single-use only
```

**Example**:
```
1A2B-3C4D
5E6F-7G8H
9I0J-1K2L
```

**Security**:
- Each code can only be used once
- Hashed before storage
- Removed from list after use
- New set generated when all used

## Brute Force Protection

### Account Lockout

**Parameters**:
```csharp
"Security": {
  "MaxLoginAttempts": 5,
  "LockoutDurationMinutes": 30
}
```

**Lockout Flow**:
```
Failed Login Attempt
    ↓
Increment AccessFailedCount
    ↓
AccessFailedCount >= MaxLoginAttempts?
    ↓ Yes
Set LockoutEnd = Now + LockoutDuration
    ↓
Log Security Event
    ↓
Return "Account Locked" Error
```

**Automatic Unlock**:
- Time-based: After lockout duration expires
- Manual: Administrator can unlock
- Successful login: Reset failed count to 0

### Rate Limiting

**IP-Based Limiting**:
```csharp
"RateLimiting": {
  "PermitLimit": 100,      // Requests allowed
  "WindowSeconds": 60,     // Per time window
  "QueueLimit": 0          // No queueing
}
```

**Implementation Options**:
1. Memory-based (single server)
2. Redis-based (distributed)
3. API Gateway (AWS/Azure)

### Failed Attempt Tracking

**Audit Log Entry**:
```json
{
  "event_type": "LoginFailed",
  "username": "attempted_user",
  "ip_address": "192.168.1.100",
  "reason": "Invalid password",
  "severity": "Warning",
  "timestamp": "2024-01-01T12:00:00Z"
}
```

**Monitoring Alerts**:
- Multiple failures from same IP
- Multiple failures for same user
- Distributed attack patterns

## Security Headers

### Implemented Headers

```csharp
app.Use(async (context, next) =>
{
    // Prevent MIME type sniffing
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

    // Prevent clickjacking
    context.Response.Headers.Add("X-Frame-Options", "DENY");

    // Enable XSS protection
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");

    // Control referrer information
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

    // Disable dangerous features
    context.Response.Headers.Add("Permissions-Policy", "geolocation=(), microphone=(), camera=()");

    // Remove server header (information disclosure)
    context.Response.Headers.Remove("Server");

    await next();
});
```

### Content Security Policy (CSP)

**Recommended Policy**:
```
Content-Security-Policy:
  default-src 'self';
  script-src 'self' 'unsafe-inline';
  style-src 'self' 'unsafe-inline';
  img-src 'self' data: https:;
  font-src 'self';
  connect-src 'self';
  frame-ancestors 'none';
```

## Audit Logging

### What We Log

**Authentication Events**:
- ✅ Login attempts (success/failure)
- ✅ Logout events
- ✅ Token refresh
- ✅ Password changes
- ✅ 2FA setup/disable

**Authorization Events**:
- ✅ Role assignments
- ✅ Permission grants/revocations
- ✅ Access denials

**Security Events**:
- ✅ Account lockouts
- ✅ Suspicious activities
- ✅ Token revocations
- ✅ Security stamp changes

### Audit Log Structure

```json
{
  "id": "log-id",
  "user_id": "user-guid",
  "username": "john.doe",
  "event_type": "Login",
  "description": "Successful login",
  "ip_address": "192.168.1.100",
  "user_agent": "Mozilla/5.0...",
  "http_method": "POST",
  "endpoint": "/api/auth/login",
  "status_code": 200,
  "severity": "Information",
  "is_successful": true,
  "metadata": "{\"login_method\":\"password\"}",
  "timestamp": "2024-01-01T12:00:00Z"
}
```

### Retention Policy

**Recommended**:
- **Active Logs**: Last 90 days in hot storage
- **Archive**: 1-7 years in cold storage
- **Compliance**: Retain per regulatory requirements (GDPR, HIPAA, etc.)

## Security Best Practices

### 1. Secret Management

**DO**:
- ✅ Store secrets in environment variables
- ✅ Use Azure Key Vault / AWS Secrets Manager
- ✅ Rotate secrets regularly
- ✅ Use different secrets per environment

**DON'T**:
- ❌ Hard-code secrets in code
- ❌ Commit secrets to version control
- ❌ Share secrets via email/chat
- ❌ Use same secret in dev/prod

### 2. HTTPS/TLS

**Requirements**:
- ✅ TLS 1.2+ only (disable TLS 1.0, 1.1)
- ✅ Strong cipher suites
- ✅ Valid SSL certificate
- ✅ HSTS header in production

**Configuration**:
```csharp
// Production
app.UseHsts();
app.UseHttpsRedirection();
```

### 3. CORS Configuration

**Principle of Least Privilege**:
```csharp
// ❌ Bad - Allow all origins
policy.AllowAnyOrigin()

// ✅ Good - Whitelist specific origins
policy.WithOrigins(
    "https://trusted-frontend.com",
    "https://app.example.com"
)
```

### 4. SQL Injection Prevention

**Use Parameterized Queries**:
```csharp
// ✅ Safe - EF Core uses parameters
var user = await context.Users
    .Where(u => u.Username == username)
    .FirstOrDefaultAsync();

// ❌ Dangerous - String concatenation
var query = $"SELECT * FROM Users WHERE Username = '{username}'";
```

### 5. Sensitive Data Exposure

**Never Log**:
- ❌ Passwords (plain or hashed)
- ❌ Credit card numbers
- ❌ Social security numbers
- ❌ Full access tokens (log only last 4 chars)

**Mask in Logs**:
```csharp
// ❌ Bad
_logger.LogInformation($"Login attempt: {password}");

// ✅ Good
_logger.LogInformation($"Login attempt for user: {username}");
```

## Threat Mitigation

### OWASP Top 10

| Threat | Mitigation |
|--------|------------|
| **A01: Broken Access Control** | RBAC, Permission checks, Proper authorization |
| **A02: Cryptographic Failures** | BCrypt for passwords, TLS for transport, Secure token storage |
| **A03: Injection** | Parameterized queries (EF Core), Input validation |
| **A04: Insecure Design** | Security by design, Threat modeling, Defense in depth |
| **A05: Security Misconfiguration** | Secure defaults, Remove unnecessary features, Security headers |
| **A06: Vulnerable Components** | Regular updates, Dependency scanning, Known good versions |
| **A07: Authentication Failures** | Strong auth, MFA, Session management, Brute force protection |
| **A08: Software/Data Integrity** | Code signing, Secure CI/CD, Validate dependencies |
| **A09: Logging Failures** | Comprehensive audit logging, Log monitoring, Alerting |
| **A10: SSRF** | Validate and sanitize URLs, Whitelist allowed hosts |

### Common Attack Scenarios

#### 1. **Credential Stuffing**
**Attack**: Using leaked credentials from other breaches
**Mitigation**:
- Rate limiting
- Account lockout
- MFA requirement
- Breach detection integration

#### 2. **Token Theft**
**Attack**: Stealing JWT from client storage
**Mitigation**:
- Short-lived tokens
- Secure storage (httpOnly cookies)
- Token blacklisting
- Security stamp validation

#### 3. **Session Fixation**
**Attack**: Forcing user to use attacker's session
**Mitigation**:
- Generate new tokens on login
- Rotate refresh tokens
- Bind tokens to client

#### 4. **XSS (Cross-Site Scripting)**
**Attack**: Inject malicious scripts
**Mitigation**:
- CSP headers
- Input validation
- Output encoding
- X-XSS-Protection header

## Security Checklist

### Pre-Production

- [ ] Change all default secrets and passwords
- [ ] Use environment variables for secrets
- [ ] Enable HTTPS with valid certificate
- [ ] Configure proper CORS policy
- [ ] Enable security headers
- [ ] Set up audit logging
- [ ] Configure rate limiting
- [ ] Enable account lockout
- [ ] Test with OWASP ZAP or similar
- [ ] Perform code security review
- [ ] Set up monitoring and alerting
- [ ] Document incident response plan

### Regular Maintenance

- [ ] Review audit logs weekly
- [ ] Update dependencies monthly
- [ ] Rotate secrets quarterly
- [ ] Review user permissions quarterly
- [ ] Conduct security assessments annually
- [ ] Update threat model as needed
- [ ] Review and update security policies

### Incident Response

If security breach is detected:
1. **Contain**: Revoke compromised tokens
2. **Assess**: Review audit logs
3. **Remediate**: Fix vulnerability
4. **Notify**: Inform affected users
5. **Document**: Create incident report
6. **Improve**: Update security controls

---

**Remember**: Security is not a feature, it's a process. Continuous vigilance and improvement are essential.
