using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JwtPoc.Core.DTOs;

namespace JwtPoc.Api.Controllers;

/// <summary>
/// Authentication controller demonstrating JWT auth patterns
/// This is a DEMONSTRATION controller showing the structure
/// Full implementation would use injected IAuthenticationService
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthController(
        ILogger<AuthController> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// User login endpoint
    /// Returns JWT access token and refresh token
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Login response with tokens</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Production implementation would:
        // 1. Validate credentials using IAuthenticationService
        // 2. Check for account lockout
        // 3. Verify 2FA if enabled
        // 4. Generate and return JWT tokens
        // 5. Log audit event

        _logger.LogInformation("Login attempt for user: {Username}", request.UsernameOrEmail);

        // Demonstration response structure
        var response = ApiResponse<LoginResponse>.SuccessResponse(
            new LoginResponse
            {
                AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
                RefreshToken = "refresh_token_here",
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                RequiresTwoFactor = false,
                User = new UserDto
                {
                    Id = Guid.NewGuid(),
                    Username = "demo_user",
                    Email = "demo@example.com",
                    FirstName = "Demo",
                    LastName = "User",
                    Roles = new List<RoleDto>
                    {
                        new RoleDto { Name = "User", Description = "Standard User" }
                    }
                }
            },
            "Login successful"
        );

        return Ok(response);
    }

    /// <summary>
    /// User registration endpoint
    /// Creates a new user account
    /// </summary>
    /// <param name="request">Registration information</param>
    /// <returns>Created user information</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        _logger.LogInformation("Registration attempt for username: {Username}", request.Username);

        // Production implementation would:
        // 1. Validate username/email uniqueness
        // 2. Hash password securely
        // 3. Create user with default role
        // 4. Send email verification if configured
        // 5. Log audit event

        var response = ApiResponse<UserDto>.SuccessResponse(
            new UserDto
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            },
            "Registration successful"
        );

        return CreatedAtAction(nameof(Register), response);
    }

    /// <summary>
    /// Token refresh endpoint
    /// Exchanges refresh token for new access token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <returns>New tokens</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        // Production implementation would:
        // 1. Validate refresh token
        // 2. Check if token is revoked or expired
        // 3. Validate token family (prevent replay attacks)
        // 4. Generate new access and refresh tokens
        // 5. Revoke old refresh token
        // 6. Log audit event

        _logger.LogInformation("Token refresh attempt");

        var response = ApiResponse<LoginResponse>.SuccessResponse(
            new LoginResponse
            {
                AccessToken = "new_access_token",
                RefreshToken = "new_refresh_token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(15)
            },
            "Token refreshed successfully"
        );

        return Ok(response);
    }

    /// <summary>
    /// Logout endpoint
    /// Revokes user tokens
    /// </summary>
    /// <param name="request">Logout options</param>
    /// <returns>Success message</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        var userId = User.FindFirst("sub")?.Value;

        _logger.LogInformation("Logout for user: {UserId}", userId);

        // Production implementation would:
        // 1. Extract JWT ID from current token
        // 2. Blacklist current access token
        // 3. Revoke refresh tokens (all or current device)
        // 4. Log audit event

        var response = ApiResponse<object>.SuccessResponse(
            null,
            request.LogoutFromAllDevices
                ? "Logged out from all devices successfully"
                : "Logged out successfully"
        );

        return Ok(response);
    }

    /// <summary>
    /// Change password endpoint
    /// Updates user password and invalidates all tokens
    /// </summary>
    /// <param name="request">Password change request</param>
    /// <returns>Success message</returns>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = User.FindFirst("sub")?.Value;

        _logger.LogInformation("Password change for user: {UserId}", userId);

        // Production implementation would:
        // 1. Verify current password
        // 2. Validate new password strength
        // 3. Hash and update password
        // 4. Update security stamp
        // 5. Revoke all existing tokens
        // 6. Log audit event

        var response = ApiResponse<object>.SuccessResponse(
            null,
            "Password changed successfully. Please login again."
        );

        return Ok(response);
    }

    /// <summary>
    /// Setup two-factor authentication
    /// Returns QR code and backup codes
    /// </summary>
    /// <param name="request">2FA setup request</param>
    /// <returns>QR code and backup codes</returns>
    [HttpPost("2fa/setup")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<SetupTwoFactorResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SetupTwoFactor([FromBody] SetupTwoFactorRequest request)
    {
        var userId = User.FindFirst("sub")?.Value;

        _logger.LogInformation("2FA setup for user: {UserId}", userId);

        // Production implementation would:
        // 1. Generate TOTP secret
        // 2. Create QR code URL
        // 3. Generate backup codes
        // 4. Store encrypted secret (not yet enabled)
        // 5. Return setup information

        var response = ApiResponse<SetupTwoFactorResponse>.SuccessResponse(
            new SetupTwoFactorResponse
            {
                QrCodeUrl = "otpauth://totp/JwtPoc:user@example.com?secret=DEMO&issuer=JwtPoc",
                ManualEntryKey = "DEMO SECRET KEY",
                BackupCodes = new List<string>
                {
                    "1234-5678", "2345-6789", "3456-7890"
                }
            },
            "2FA setup initiated. Scan QR code with authenticator app."
        );

        return Ok(response);
    }

    /// <summary>
    /// Verify and enable two-factor authentication
    /// </summary>
    /// <param name="request">Verification code</param>
    /// <returns>Success message</returns>
    [HttpPost("2fa/verify")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyTwoFactor([FromBody] VerifyTwoFactorRequest request)
    {
        var userId = User.FindFirst("sub")?.Value;

        _logger.LogInformation("2FA verification for user: {UserId}", userId);

        // Production implementation would:
        // 1. Verify TOTP code against stored secret
        // 2. Enable 2FA for user
        // 3. Hash and store backup codes
        // 4. Update security stamp
        // 5. Log audit event

        var response = ApiResponse<object>.SuccessResponse(
            null,
            "Two-factor authentication enabled successfully"
        );

        return Ok(response);
    }

    /// <summary>
    /// Disable two-factor authentication
    /// </summary>
    /// <param name="request">Password confirmation</param>
    /// <returns>Success message</returns>
    [HttpPost("2fa/disable")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DisableTwoFactor([FromBody] DisableTwoFactorRequest request)
    {
        var userId = User.FindFirst("sub")?.Value;

        _logger.LogInformation("2FA disable for user: {UserId}", userId);

        // Production implementation would:
        // 1. Verify password
        // 2. Disable 2FA
        // 3. Clear TOTP secret and backup codes
        // 4. Update security stamp
        // 5. Log audit event

        var response = ApiResponse<object>.SuccessResponse(
            null,
            "Two-factor authentication disabled"
        );

        return Ok(response);
    }

    /// <summary>
    /// Get current user information
    /// Demonstrates authenticated endpoint
    /// </summary>
    /// <returns>Current user details</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    public IActionResult GetCurrentUser()
    {
        // Extract claims from JWT
        var userId = User.FindFirst("sub")?.Value;
        var username = User.FindFirst("username")?.Value;
        var email = User.FindFirst("email")?.Value;
        var roles = User.FindAll("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
            .Select(c => c.Value).ToList();
        var permissions = User.FindAll("permission")
            .Select(c => c.Value).ToList();

        _logger.LogInformation("Get current user info: {UserId}", userId);

        var response = ApiResponse<UserDto>.SuccessResponse(
            new UserDto
            {
                Id = Guid.Parse(userId ?? Guid.Empty.ToString()),
                Username = username ?? "unknown",
                Email = email ?? "unknown",
                Roles = roles.Select(r => new RoleDto { Name = r }).ToList(),
                Permissions = permissions
            },
            "User information retrieved successfully"
        );

        return Ok(response);
    }
}
