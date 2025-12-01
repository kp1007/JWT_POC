# Enterprise JWT Authentication & Authorization POC

A comprehensive .NET 8 proof-of-concept application demonstrating enterprise-grade JWT token validation with role-based access control (RBAC), two-factor authentication, and advanced security features.

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Quick Start](#quick-start)
- [Configuration](#configuration)
- [Test Users](#test-users)
- [API Documentation](#api-documentation)
- [Security Features](#security-features)
- [Project Structure](#project-structure)
- [Documentation](#documentation)
- [Contributing](#contributing)
- [License](#license)

## 🎯 Overview

This POC demonstrates best practices for implementing JWT-based authentication and authorization in a .NET 8 Web API application. It serves as both a working reference implementation and an educational resource for development teams.

**Key Objectives:**
- Demonstrate industry-standard JWT implementation
- Show RBAC and permission-based authorization patterns
- Implement advanced security features (2FA, token blacklisting, refresh rotation)
- Provide comprehensive documentation and examples
- Serve as a learning resource for enterprise authentication

## ✨ Features

### Authentication & Authorization
- ✅ **JWT Token Generation & Validation** with HS256 signing
- ✅ **Refresh Token Rotation** with family validation
- ✅ **Role-Based Access Control (RBAC)** with hierarchical roles
- ✅ **Permission-Based Authorization** for fine-grained access control
- ✅ **Two-Factor Authentication (2FA)** with TOTP support
- ✅ **Token Blacklisting** for immediate invalidation
- ✅ **Account Lockout** with brute force protection

### Security Features
- ✅ **BCrypt Password Hashing** (work factor 12)
- ✅ **Secure Token Storage** with cryptographic random generation
- ✅ **Security Headers** (X-Frame-Options, CSP, etc.)
- ✅ **CORS Configuration** with whitelist support
- ✅ **Audit Logging** for all security events
- ✅ **Rate Limiting** support
- ✅ **HTTPS Enforcement** in production

### Additional Features
- ✅ **Clean Architecture** with proper separation of concerns
- ✅ **Entity Framework Core** with Code First approach
- ✅ **Database Seeding** with test users and roles
- ✅ **Swagger/OpenAPI** documentation
- ✅ **Serilog** structured logging
- ✅ **Docker Support** with multi-stage builds
- ✅ **Comprehensive Documentation**

## 🏗️ Architecture

This application follows **Clean Architecture** principles:

```
┌─────────────────────────────────────────┐
│           Presentation Layer             │
│         (JwtPoc.Api)                    │
│  Controllers, Middleware, Filters       │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│          Application Layer               │
│     (Service Implementations)            │
│  Auth, User, Role, Token Services       │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│            Domain Layer                  │
│         (JwtPoc.Core)                   │
│  Entities, Interfaces, DTOs, Enums      │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│       Infrastructure Layer               │
│     (JwtPoc.Infrastructure)             │
│  DbContext, Repositories, Services      │
└─────────────────────────────────────────┘
```

For detailed architecture documentation, see [ARCHITECTURE.md](ARCHITECTURE.md).

## 📦 Prerequisites

- .NET 8 SDK ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- SQL Server 2019+ or LocalDB
- (Optional) Docker Desktop for containerized deployment
- (Optional) Visual Studio 2022 or VS Code

## 🚀 Quick Start

### Option 1: Local Development

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd JWT_POC
   ```

2. **Update connection string**

   Edit `src/JwtPoc.Api/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=JwtPocDb;Trusted_Connection=true;"
   }
   ```

3. **Restore dependencies**
   ```bash
   dotnet restore
   ```

4. **Run database migrations**
   ```bash
   cd src/JwtPoc.Api
   dotnet ef database update --project ../JwtPoc.Infrastructure
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Access Swagger UI**

   Navigate to: `https://localhost:5001` or `http://localhost:5000`

### Option 2: Docker Deployment

1. **Build and run with Docker Compose**
   ```bash
   docker-compose up -d
   ```

2. **Access the API**

   Navigate to: `http://localhost:5000/swagger`

3. **View logs**
   ```bash
   docker-compose logs -f api
   ```

## ⚙️ Configuration

### JWT Settings

Configure in `appsettings.json`:

```json
"Jwt": {
  "SecretKey": "YourSecretKeyMustBeAtLeast32CharactersLong!",
  "Issuer": "JwtPocApi",
  "Audience": "JwtPocClient",
  "AccessTokenExpirationMinutes": 15,
  "RefreshTokenExpirationDays": 7
}
```

**⚠️ Production Warning:** Always use environment variables for secrets in production!

```bash
export Jwt__SecretKey="your-production-secret-from-key-vault"
```

### Security Settings

```json
"Security": {
  "MaxLoginAttempts": 5,
  "LockoutDurationMinutes": 30,
  "RequireEmailConfirmation": false
}
```

### CORS Configuration

```json
"Cors": {
  "AllowedOrigins": [
    "http://localhost:3000",
    "https://your-frontend-domain.com"
  ]
}
```

## 👥 Test Users

The application seeds the following test users on first run:

| Username    | Password         | Role        | Permissions                                |
|-------------|------------------|-------------|--------------------------------------------|
| superadmin  | SuperAdmin@123   | SuperAdmin  | All permissions                            |
| admin       | Admin@123        | Admin       | User, Role, Permission management          |
| manager     | Manager@123      | Manager     | User viewing, Report management            |
| user        | User@123         | User        | Profile management, View reports           |
| guest       | Guest@123        | Guest       | View own profile only                      |

### Role Hierarchy

```
SuperAdmin (Level 1000) - Full system access
    ↓
Admin (Level 100) - Administrative functions
    ↓
Manager (Level 50) - Team and report management
    ↓
User (Level 1) - Basic user functions
    ↓
Guest (Level 0) - Read-only access
```

## 📚 API Documentation

### Authentication Endpoints

| Endpoint                    | Method | Auth Required | Description                      |
|-----------------------------|--------|---------------|----------------------------------|
| `/api/auth/register`        | POST   | No            | Register new user                |
| `/api/auth/login`           | POST   | No            | Login and get tokens             |
| `/api/auth/refresh`         | POST   | No            | Refresh access token             |
| `/api/auth/logout`          | POST   | Yes           | Logout and revoke tokens         |
| `/api/auth/change-password` | POST   | Yes           | Change user password             |
| `/api/auth/me`              | GET    | Yes           | Get current user info            |

### Two-Factor Authentication

| Endpoint               | Method | Auth Required | Description                      |
|------------------------|--------|---------------|----------------------------------|
| `/api/auth/2fa/setup`  | POST   | Yes           | Setup 2FA with QR code           |
| `/api/auth/2fa/verify` | POST   | Yes           | Verify and enable 2FA            |
| `/api/auth/2fa/disable`| POST   | Yes           | Disable 2FA                      |

### Demo Endpoints (Authorization Examples)

| Endpoint                  | Method | Required Role/Permission          |
|---------------------------|--------|-----------------------------------|
| `/api/demo/public`        | GET    | Authenticated                     |
| `/api/demo/user-only`     | GET    | User, Manager, Admin, SuperAdmin  |
| `/api/demo/manager-only`  | GET    | Manager, Admin, SuperAdmin        |
| `/api/demo/admin-only`    | GET    | Admin, SuperAdmin                 |
| `/api/demo/reports`       | GET    | Permission: users.read            |

For complete API documentation with request/response examples, see [API.md](API.md).

## 🔒 Security Features

### 1. JWT Token Security
- **HS256 Signing Algorithm**: Industry-standard HMAC-SHA256
- **Short-lived Access Tokens**: 15-minute expiration
- **Secure Refresh Tokens**: 7-day expiration with rotation
- **Token Blacklisting**: Immediate invalidation on logout/security events
- **Security Stamp Validation**: Automatic invalidation on password change

### 2. Password Security
- **BCrypt Hashing**: Work factor 12 (adjustable)
- **Password Complexity Requirements**:
  - Minimum 8 characters
  - Uppercase and lowercase letters
  - Numbers and special characters
- **Rehashing Support**: Automatic upgrade when work factor changes

### 3. Two-Factor Authentication
- **TOTP Implementation**: Time-based One-Time Passwords
- **QR Code Generation**: Compatible with Google Authenticator, Authy, etc.
- **Backup Codes**: 10 single-use recovery codes
- **Multiple Methods**: Authenticator app, SMS, Email support

### 4. Brute Force Protection
- **Account Lockout**: After 5 failed attempts
- **Lockout Duration**: 30 minutes (configurable)
- **IP-based Tracking**: Monitor failed attempts by IP
- **Audit Logging**: All authentication attempts logged

### 5. Audit Trail
- **Comprehensive Logging**: All security events tracked
- **Event Types**: Login, Logout, Password Change, Role Change, etc.
- **Metadata Storage**: IP address, user agent, request details
- **Severity Levels**: Information, Warning, Error, Critical

For detailed security documentation, see [SECURITY.md](SECURITY.md).

## 📁 Project Structure

```
JWT_POC/
├── src/
│   ├── JwtPoc.Api/              # Web API project
│   │   ├── Controllers/         # API controllers
│   │   ├── Middleware/          # Custom middleware
│   │   ├── Program.cs           # Application entry point
│   │   └── appsettings.json     # Configuration
│   │
│   ├── JwtPoc.Core/             # Domain layer
│   │   ├── Entities/            # Domain entities
│   │   ├── Interfaces/          # Service interfaces
│   │   ├── DTOs/                # Data transfer objects
│   │   ├── Enums/               # Enumerations
│   │   └── Exceptions/          # Custom exceptions
│   │
│   └── JwtPoc.Infrastructure/   # Infrastructure layer
│       ├── Data/                # DbContext and migrations
│       ├── Repositories/        # Repository implementations
│       ├── Services/            # Service implementations
│       └── Configurations/      # EF Core configurations
│
├── tests/
│   ├── JwtPoc.Tests.Unit/       # Unit tests
│   └── JwtPoc.Tests.Integration/# Integration tests
│
├── docs/                         # Additional documentation
├── Dockerfile                    # Docker configuration
├── docker-compose.yml           # Docker Compose configuration
└── README.md                    # This file
```

## 📖 Documentation

- **[ARCHITECTURE.md](ARCHITECTURE.md)** - Detailed architecture and design decisions
- **[SECURITY.md](SECURITY.md)** - Security features and best practices
- **[API.md](API.md)** - Complete API endpoint documentation
- **[DEPLOYMENT.md](DEPLOYMENT.md)** - Production deployment guide

## 🧪 Testing

### Run Unit Tests
```bash
dotnet test tests/JwtPoc.Tests.Unit
```

### Run Integration Tests
```bash
dotnet test tests/JwtPoc.Tests.Integration
```

### Run All Tests
```bash
dotnet test
```

## 🔧 Development

### Database Migrations

**Create a new migration:**
```bash
dotnet ef migrations add MigrationName --project src/JwtPoc.Infrastructure --startup-project src/JwtPoc.Api
```

**Update database:**
```bash
dotnet ef database update --project src/JwtPoc.Infrastructure --startup-project src/JwtPoc.Api
```

### Code Style

This project follows:
- C# coding conventions
- Clean Code principles
- SOLID principles
- Repository pattern
- Unit of Work pattern

## 🐛 Troubleshooting

### Database Connection Issues
1. Ensure SQL Server is running
2. Verify connection string in appsettings.json
3. Check firewall settings

### JWT Token Issues
1. Verify secret key is at least 32 characters
2. Check token expiration settings
3. Ensure system clock is synchronized

### Docker Issues
1. Ensure Docker Desktop is running
2. Check port availability (5000, 1433)
3. Review docker-compose logs

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🤝 Contributing

Contributions are welcome! Please read CONTRIBUTING.md for details on our code of conduct and the process for submitting pull requests.

## 📧 Support

For questions or issues:
- Create an issue on GitHub
- Contact: support@example.com

## 🙏 Acknowledgments

- ASP.NET Core Team for the excellent framework
- Community contributors for security best practices
- Open source libraries used in this project

---

**Note:** This is a proof-of-concept application for educational purposes. Always perform security audits and penetration testing before deploying to production.
