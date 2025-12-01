using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JwtPoc.Core.DTOs;

namespace JwtPoc.Api.Controllers;

/// <summary>
/// Demonstration controller showing various authorization patterns
/// Demonstrates role-based and permission-based access control
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Requires authentication for all endpoints
public class DemoController : ControllerBase
{
    /// <summary>
    /// Public endpoint accessible to all authenticated users
    /// </summary>
    [HttpGet("public")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public IActionResult GetPublicData()
    {
        var username = User.Identity?.Name ?? User.FindFirst("username")?.Value;

        return Ok(ApiResponse<object>.SuccessResponse(
            new
            {
                message = $"Hello {username}! This is accessible to all authenticated users.",
                timestamp = DateTime.UtcNow
            },
            "Public data retrieved"
        ));
    }

    /// <summary>
    /// Endpoint accessible only to users with User role or higher
    /// </summary>
    [HttpGet("user-only")]
    [Authorize(Roles = "User,Manager,Admin,SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetUserData()
    {
        var roles = User.FindAll("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
            .Select(c => c.Value).ToList();

        return Ok(ApiResponse<object>.SuccessResponse(
            new
            {
                message = "This data is for User role and above",
                your_roles = roles,
                data = "User-level sensitive information"
            },
            "User data retrieved"
        ));
    }

    /// <summary>
    /// Endpoint accessible only to Managers and above
    /// Demonstrates hierarchical role access
    /// </summary>
    [HttpGet("manager-only")]
    [Authorize(Policy = "RequireManagerRole")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetManagerData()
    {
        return Ok(ApiResponse<object>.SuccessResponse(
            new
            {
                message = "Manager-level access granted",
                data = new
                {
                    team_reports = new[] { "Report 1", "Report 2", "Report 3" },
                    statistics = new { total_users = 150, active_projects = 12 }
                }
            },
            "Manager data retrieved"
        ));
    }

    /// <summary>
    /// Endpoint accessible only to Administrators
    /// </summary>
    [HttpGet("admin-only")]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetAdminData()
    {
        return Ok(ApiResponse<object>.SuccessResponse(
            new
            {
                message = "Administrator access granted",
                data = new
                {
                    system_config = new { database_status = "Healthy", cache_status = "Healthy" },
                    pending_approvals = 5,
                    system_alerts = new[] { "None" }
                }
            },
            "Admin data retrieved"
        ));
    }

    /// <summary>
    /// Endpoint using permission-based authorization
    /// Accessible to users with specific permission
    /// </summary>
    [HttpGet("reports")]
    [Authorize(Policy = "CanViewUsers")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetReports()
    {
        var permissions = User.FindAll("permission").Select(c => c.Value).ToList();

        return Ok(ApiResponse<object>.SuccessResponse(
            new
            {
                message = "Reports accessible based on permissions",
                your_permissions = permissions,
                reports = new[]
                {
                    new { id = 1, name = "Monthly Sales", status = "Ready" },
                    new { id = 2, name = "User Analytics", status = "Processing" }
                }
            },
            "Reports retrieved"
        ));
    }

    /// <summary>
    /// Protected POST endpoint demonstrating data modification with authorization
    /// </summary>
    [HttpPost("create-report")]
    [Authorize(Roles = "Manager,Admin,SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult CreateReport([FromBody] object reportData)
    {
        var userId = User.FindFirst("sub")?.Value;
        var username = User.FindFirst("username")?.Value;

        return Created("/api/demo/create-report/1", ApiResponse<object>.SuccessResponse(
            new
            {
                id = Guid.NewGuid(),
                created_by = username,
                created_at = DateTime.UtcNow,
                status = "Created"
            },
            "Report created successfully"
        ));
    }

    /// <summary>
    /// Endpoint demonstrating multiple authorization requirements
    /// Requires both role AND specific permission
    /// </summary>
    [HttpDelete("sensitive-operation/{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [Authorize(Policy = "CanManageUsers")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult SensitiveOperation(Guid id)
    {
        return Ok(ApiResponse<object>.SuccessResponse(
            new { id, deleted = true, deleted_at = DateTime.UtcNow },
            "Sensitive operation completed successfully"
        ));
    }

    /// <summary>
    /// Shows user's current authorization context
    /// Useful for debugging and understanding JWT claims
    /// </summary>
    [HttpGet("my-access")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public IActionResult GetMyAccess()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        var roles = User.FindAll("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
            .Select(c => c.Value).ToList();
        var permissions = User.FindAll("permission").Select(c => c.Value).ToList();

        return Ok(ApiResponse<object>.SuccessResponse(
            new
            {
                user_id = User.FindFirst("sub")?.Value,
                username = User.FindFirst("username")?.Value,
                email = User.FindFirst("email")?.Value,
                roles,
                permissions,
                all_claims = claims
            },
            "Access information retrieved"
        ));
    }
}
