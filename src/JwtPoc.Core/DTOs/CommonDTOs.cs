namespace JwtPoc.Core.DTOs;

/// <summary>
/// Generic API response wrapper
/// Provides consistent response structure across all endpoints
/// </summary>
/// <typeparam name="T">Type of data being returned</typeparam>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> SuccessResponse(T data, string message = "Operation successful")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}

/// <summary>
/// Generic paginated response
/// </summary>
/// <typeparam name="T">Type of items in the list</typeparam>
public class PaginatedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;
}

/// <summary>
/// Base pagination query parameters
/// </summary>
public class PaginationQuery
{
    private int _pageSize = 10;
    private const int MaxPageSize = 100;

    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}

/// <summary>
/// DTO for audit log entries
/// </summary>
public class AuditLogDto
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string Severity { get; set; } = string.Empty;
    public bool IsSuccessful { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Query parameters for audit log filtering
/// </summary>
public class AuditLogQuery : PaginationQuery
{
    public Guid? UserId { get; set; }
    public string? EventType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Severity { get; set; }
    public bool? IsSuccessful { get; set; }
}
