namespace JwtPoc.Core.Exceptions;

/// <summary>
/// Base exception for all application-specific exceptions
/// </summary>
public class AppException : Exception
{
    public AppException(string message) : base(message) { }

    public AppException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when a requested entity is not found
/// Maps to HTTP 404
/// </summary>
public class NotFoundException : AppException
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' was not found") { }

    public NotFoundException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when a validation error occurs
/// Maps to HTTP 400
/// </summary>
public class ValidationException : AppException
{
    public List<string> Errors { get; }

    public ValidationException(string message) : base(message)
    {
        Errors = new List<string> { message };
    }

    public ValidationException(List<string> errors)
        : base("One or more validation errors occurred")
    {
        Errors = errors;
    }
}

/// <summary>
/// Exception thrown when user credentials are invalid
/// Maps to HTTP 401
/// </summary>
public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "Invalid credentials")
        : base(message) { }
}

/// <summary>
/// Exception thrown when user lacks required permissions
/// Maps to HTTP 403
/// </summary>
public class ForbiddenException : AppException
{
    public ForbiddenException(string message = "You do not have permission to perform this action")
        : base(message) { }
}

/// <summary>
/// Exception thrown when a conflict occurs (e.g., duplicate username)
/// Maps to HTTP 409
/// </summary>
public class ConflictException : AppException
{
    public ConflictException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when an account is locked
/// Maps to HTTP 423
/// </summary>
public class AccountLockedException : AppException
{
    public DateTime? LockoutEnd { get; }

    public AccountLockedException(DateTime? lockoutEnd)
        : base($"Account is locked{(lockoutEnd.HasValue ? $" until {lockoutEnd.Value:yyyy-MM-dd HH:mm:ss} UTC" : "")}")
    {
        LockoutEnd = lockoutEnd;
    }
}

/// <summary>
/// Exception thrown when a token is invalid or expired
/// Maps to HTTP 401
/// </summary>
public class InvalidTokenException : AppException
{
    public InvalidTokenException(string message = "Invalid or expired token")
        : base(message) { }
}

/// <summary>
/// Exception thrown when two-factor authentication fails
/// Maps to HTTP 401
/// </summary>
public class TwoFactorRequiredException : AppException
{
    public TwoFactorRequiredException(string message = "Two-factor authentication is required")
        : base(message) { }
}

/// <summary>
/// Exception thrown when a business rule is violated
/// Maps to HTTP 422
/// </summary>
public class BusinessRuleException : AppException
{
    public BusinessRuleException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when rate limiting is exceeded
/// Maps to HTTP 429
/// </summary>
public class RateLimitExceededException : AppException
{
    public RateLimitExceededException(string message = "Rate limit exceeded. Please try again later.")
        : base(message) { }
}
