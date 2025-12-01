# Architecture Documentation

## Table of Contents

- [Overview](#overview)
- [Architectural Principles](#architectural-principles)
- [System Architecture](#system-architecture)
- [Layer Responsibilities](#layer-responsibilities)
- [Design Patterns](#design-patterns)
- [Data Flow](#data-flow)
- [Security Architecture](#security-architecture)
- [Scalability Considerations](#scalability-considerations)
- [Technology Stack](#technology-stack)

## Overview

This application implements **Clean Architecture** (also known as Onion Architecture or Hexagonal Architecture) to achieve:
- High maintainability
- Testability
- Independence from frameworks and UI
- Business logic isolation
- Dependency inversion

## Architectural Principles

### 1. Separation of Concerns
Each layer has a specific responsibility and doesn't overlap with others:
- **Presentation**: HTTP requests/responses, validation, authentication
- **Application**: Business logic orchestration, use cases
- **Domain**: Core business entities and rules
- **Infrastructure**: External concerns (database, external APIs, file system)

### 2. Dependency Inversion
- **Dependencies flow inward**: Outer layers depend on inner layers, never the reverse
- **Abstractions over implementations**: Use interfaces to decouple layers
- **IoC Container**: Dependency injection resolves concrete implementations

```
┌─────────────────────────────────────┐
│     Presentation Layer (API)         │
│      Depends on ↓                   │
├─────────────────────────────────────┤
│   Application Layer (Services)       │
│      Depends on ↓                   │
├─────────────────────────────────────┤
│    Domain Layer (Core)               │  ← No Dependencies
│    Entities, Interfaces, DTOs        │
├─────────────────────────────────────┤
│  Infrastructure Layer                │
│  Implements → Core Interfaces        │
└─────────────────────────────────────┘
```

### 3. Single Responsibility Principle (SOLID)
Each class has one reason to change:
- `User` entity: User data structure
- `IUserService`: User business logic interface
- `UserController`: HTTP request handling
- `UserRepository`: Data persistence

### 4. Open/Closed Principle
- Open for extension, closed for modification
- Use inheritance and interfaces to add features without changing existing code

### 5. Liskov Substitution Principle
- Derived classes can substitute base classes
- All repository implementations can be swapped without breaking code

## System Architecture

### High-Level Component Diagram

```
┌──────────────────────────────────────────────────────────────┐
│                         Clients                               │
│   (Web App, Mobile App, Postman, Third-party Services)       │
└────────────────────┬─────────────────────────────────────────┘
                     │ HTTPS
                     ▼
┌──────────────────────────────────────────────────────────────┐
│                   API Gateway / Load Balancer                 │
│              (Optional - for production scale)                │
└────────────────────┬─────────────────────────────────────────┘
                     │
                     ▼
┌──────────────────────────────────────────────────────────────┐
│                    JwtPoc.Api (Web API)                       │
│  ┌────────────────────────────────────────────────────────┐  │
│  │  Middleware Pipeline                                    │  │
│  │  • Security Headers                                     │  │
│  │  • CORS                                                 │  │
│  │  • Authentication (JWT Bearer)                          │  │
│  │  • Authorization                                        │  │
│  │  • Rate Limiting                                        │  │
│  │  • Exception Handling                                   │  │
│  │  • Request Logging                                      │  │
│  └────────────────────────────────────────────────────────┘  │
│  ┌────────────────────────────────────────────────────────┐  │
│  │  Controllers                                            │  │
│  │  • AuthController                                       │  │
│  │  • UserController                                       │  │
│  │  • RoleController                                       │  │
│  │  • DemoController                                       │  │
│  └────────────────────────────────────────────────────────┘  │
└────────────────────┬─────────────────────────────────────────┘
                     │
                     ▼
┌──────────────────────────────────────────────────────────────┐
│              JwtPoc.Infrastructure (Services)                 │
│  ┌────────────────────────────────────────────────────────┐  │
│  │  Services                                               │  │
│  │  • AuthenticationService                                │  │
│  │  • JwtService                                           │  │
│  │  • PasswordHasher                                       │  │
│  │  • TwoFactorService                                     │  │
│  │  • AuditService                                         │  │
│  │  • TokenBlacklistService                                │  │
│  └────────────────────────────────────────────────────────┘  │
│  ┌────────────────────────────────────────────────────────┐  │
│  │  Repositories (Unit of Work Pattern)                    │  │
│  │  • UserRepository                                       │  │
│  │  • RoleRepository                                       │  │
│  │  • PermissionRepository                                 │  │
│  │  • RefreshTokenRepository                               │  │
│  │  • AuditLogRepository                                   │  │
│  └────────────────────────────────────────────────────────┘  │
└────────────────────┬─────────────────────────────────────────┘
                     │
                     ▼
┌──────────────────────────────────────────────────────────────┐
│                    JwtPoc.Core (Domain)                       │
│  • Entities (User, Role, Permission, etc.)                   │
│  • Interfaces (IRepository, IService)                        │
│  • DTOs (Request/Response models)                            │
│  • Enums (Event types, Severity levels)                     │
│  • Exceptions (Domain-specific exceptions)                   │
└──────────────────────────────────────────────────────────────┘
                     │
                     ▼
┌──────────────────────────────────────────────────────────────┐
│                    Database Layer                             │
│  • SQL Server                                                 │
│  • Entity Framework Core                                      │
│  • Migrations                                                 │
└──────────────────────────────────────────────────────────────┘
```

## Layer Responsibilities

### 1. API Layer (JwtPoc.Api)

**Responsibilities:**
- Handle HTTP requests and responses
- Route requests to appropriate handlers
- Validate input data (model validation)
- Implement middleware pipeline
- Manage authentication and authorization
- Return proper HTTP status codes
- Transform domain models to API responses

**Key Components:**
- `Controllers/`: API endpoints
- `Middleware/`: Custom middleware (JWT validation, exception handling)
- `Attributes/`: Custom authorization attributes
- `Program.cs`: Application startup and configuration

**Does NOT:**
- Contain business logic
- Directly access database
- Implement data validation rules (beyond format validation)

### 2. Core Layer (JwtPoc.Core)

**Responsibilities:**
- Define domain entities and business rules
- Define service interfaces (contracts)
- Define DTOs for data transfer
- Define custom exceptions
- Define enumerations

**Key Components:**
- `Entities/`: Domain models (User, Role, Permission, etc.)
- `Interfaces/`: Service and repository contracts
- `DTOs/`: Request and response models
- `Enums/`: Application enumerations
- `Exceptions/`: Custom exception types

**Characteristics:**
- **Zero external dependencies** (except .NET runtime)
- **Pure business logic**
- **Framework-agnostic**
- **Highly testable**

### 3. Infrastructure Layer (JwtPoc.Infrastructure)

**Responsibilities:**
- Implement Core interfaces
- Handle database access
- Manage external service integrations
- Implement caching strategies
- Provide logging infrastructure

**Key Components:**
- `Data/`: DbContext, configurations, migrations
- `Repositories/`: Repository implementations
- `Services/`: Service implementations
- `Configurations/`: Entity configurations

**Dependencies:**
- Depends on Core layer for interfaces
- Uses Entity Framework Core
- Uses third-party libraries (BCrypt, OTP.NET, etc.)

## Design Patterns

### 1. Repository Pattern

**Purpose**: Abstract data access logic

**Implementation**:
```csharp
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly ApplicationDbContext _context;
    // Implementation...
}
```

**Benefits**:
- Decouples business logic from data access
- Enables easy unit testing with mocks
- Centralized data access logic
- Easy to switch data sources

### 2. Unit of Work Pattern

**Purpose**: Coordinate multiple repository operations in a single transaction

**Implementation**:
```csharp
public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Role> Roles { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
}
```

**Benefits**:
- Ensures data consistency
- Manages transactions across multiple entities
- Single point of commit

### 3. Dependency Injection

**Purpose**: Achieve loose coupling and testability

**Implementation**:
```csharp
// Registration in Program.cs
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Usage in Controller
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;

    public AuthController(IAuthenticationService authService)
    {
        _authService = authService;
    }
}
```

### 4. Strategy Pattern

**Purpose**: Define family of algorithms (e.g., different 2FA methods)

**Application**: Two-factor authentication methods (TOTP, SMS, Email)

### 5. Factory Pattern

**Purpose**: Object creation logic

**Application**: Token generation, refresh token creation

### 6. Chain of Responsibility

**Purpose**: Process requests through a pipeline

**Application**: Middleware pipeline (authentication → authorization → execution)

## Data Flow

### Authentication Flow

```
1. Client Request
   │
   ▼
2. AuthController.Login()
   │
   ▼
3. IAuthenticationService.LoginAsync()
   │
   ├─► Verify credentials (PasswordHasher)
   ├─► Check account status (lockout, active)
   ├─► Verify 2FA if enabled
   ├─► Generate JWT (JwtService)
   ├─► Generate refresh token
   ├─► Save refresh token (UnitOfWork)
   ├─► Log audit event (AuditService)
   │
   ▼
4. Return LoginResponse with tokens
   │
   ▼
5. Client stores tokens
```

### Authorization Flow

```
1. Client Request with JWT
   │
   ▼
2. JWT Bearer Middleware
   │
   ├─► Validate token signature
   ├─► Check expiration
   ├─► Extract claims
   │
   ▼
3. Custom Authorization Middleware
   │
   ├─► Check token blacklist
   ├─► Verify security stamp
   ├─► Check user active status
   │
   ▼
4. Authorization Handler
   │
   ├─► Check role requirements
   ├─► Check permission requirements
   │
   ▼
5. Controller Action Execution
   │
   ▼
6. Response
```

### Token Refresh Flow

```
1. Client sends refresh token
   │
   ▼
2. AuthController.RefreshToken()
   │
   ▼
3. Validate refresh token
   │
   ├─► Check existence
   ├─► Check expiration
   ├─► Check revocation status
   ├─► Validate token family
   │
   ▼
4. Generate new tokens
   │
   ├─► Create new access token
   ├─► Create new refresh token
   ├─► Revoke old refresh token
   ├─► Update token family
   │
   ▼
5. Return new tokens
```

## Security Architecture

### Defense in Depth

Multiple layers of security:

1. **Network Layer**: HTTPS, CORS, rate limiting
2. **Application Layer**: Authentication, authorization
3. **Data Layer**: Encryption at rest, SQL injection prevention
4. **Audit Layer**: Comprehensive logging

### Token Security

```
┌─────────────────────────────────────┐
│   Access Token (JWT)                 │
│   • Short-lived (15 minutes)        │
│   • Contains user claims            │
│   • Signed with secret key          │
│   • Can be blacklisted              │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│   Refresh Token                      │
│   • Long-lived (7 days)             │
│   • Cryptographically random        │
│   • One-time use                    │
│   • Token family tracking           │
│   • Revocable                       │
└─────────────────────────────────────┘
```

### Password Security Pipeline

```
Plain Password
    ↓
[Validation]
    ↓
[BCrypt Hashing (Work Factor 12)]
    ↓
Hashed Password (Stored)
```

## Scalability Considerations

### Horizontal Scaling

**Stateless Design**:
- No session state stored on server
- JWT tokens are self-contained
- Can deploy multiple API instances

**Shared Resources**:
- Centralized database
- Distributed cache (Redis) for blacklist
- Shared file storage for logs

### Vertical Scaling

**Optimization Points**:
- Database indexing on frequently queried columns
- Query optimization with EF Core
- Response caching for public endpoints
- Connection pooling

### Caching Strategy

```
┌─────────────────────────────────────┐
│   Memory Cache (L1)                  │
│   • User permissions (5 min TTL)    │
│   • Role permissions (10 min TTL)   │
└─────────────────────────────────────┘
           ↓ (Cache miss)
┌─────────────────────────────────────┐
│   Distributed Cache - Redis (L2)    │
│   • Token blacklist                 │
│   • Rate limit counters             │
└─────────────────────────────────────┘
           ↓ (Cache miss)
┌─────────────────────────────────────┐
│   Database                           │
│   • Source of truth                 │
└─────────────────────────────────────┘
```

### Performance Optimizations

1. **Eager Loading**: Include related entities to avoid N+1 queries
2. **Projection**: Select only needed columns
3. **Async/Await**: Non-blocking I/O operations
4. **Connection Pooling**: Reuse database connections
5. **Compiled Queries**: Pre-compile frequent EF queries

## Technology Stack

### Backend Framework
- **.NET 8**: Latest LTS version
- **ASP.NET Core Web API**: RESTful API framework

### Data Access
- **Entity Framework Core 8**: ORM
- **SQL Server**: Relational database
- **Code First Migrations**: Database version control

### Authentication & Security
- **Microsoft.AspNetCore.Authentication.JwtBearer**: JWT middleware
- **BCrypt.Net-Next**: Password hashing
- **OtpNet**: TOTP implementation
- **System.IdentityModel.Tokens.Jwt**: JWT generation/validation

### Logging
- **Serilog**: Structured logging
- **Serilog.Sinks.Console**: Console output
- **Serilog.Sinks.File**: File output

### Documentation
- **Swashbuckle.AspNetCore**: OpenAPI/Swagger

### Testing
- **xUnit**: Unit testing framework
- **Moq**: Mocking library
- **FluentAssertions**: Assertion library
- **Microsoft.AspNetCore.Mvc.Testing**: Integration testing

### Containerization
- **Docker**: Container platform
- **Docker Compose**: Multi-container orchestration

## Deployment Architecture

### Development
```
Developer Machine
├── Visual Studio / VS Code
├── LocalDB / SQL Server
└── .NET 8 SDK
```

### Production
```
┌───────────────────────────────────┐
│   Load Balancer                    │
└───────────┬───────────────────────┘
            │
    ┌───────┴───────┐
    │               │
┌───▼───┐       ┌───▼───┐
│ API 1 │       │ API 2 │
└───┬───┘       └───┬───┘
    └───────┬───────┘
            │
    ┌───────▼───────┐
    │   Database     │
    │   (SQL Server) │
    └────────────────┘
```

## Extensibility Points

The architecture supports easy extension:

1. **New Authentication Methods**: Implement `IAuthenticationService`
2. **Additional Databases**: Implement `IRepository<T>`
3. **Custom Authorization**: Create custom `IAuthorizationHandler`
4. **External Services**: Add new service interfaces in Core
5. **Alternative Caching**: Swap IMemoryCache for IDistributedCache

## Best Practices Implemented

1. ✅ **Async/Await**: All I/O operations are asynchronous
2. ✅ **Cancellation Tokens**: Support for request cancellation
3. ✅ **Configuration**: Environment-based configuration
4. ✅ **Logging**: Structured logging throughout
5. ✅ **Error Handling**: Global exception handling
6. ✅ **Validation**: Model validation at API boundary
7. ✅ **Documentation**: XML comments and Swagger
8. ✅ **Testing**: Unit and integration tests
9. ✅ **Security**: Industry-standard practices
10. ✅ **Clean Code**: SOLID principles, meaningful names

---

This architecture provides a solid foundation for enterprise applications with excellent maintainability, testability, and scalability characteristics.
