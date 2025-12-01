using JwtPoc.Core.DTOs;

namespace JwtPoc.Core.Interfaces;

/// <summary>
/// Service interface for authentication operations
/// Handles user login, registration, token management, and 2FA
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Authenticates a user and returns JWT tokens
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <param name="ipAddress">Client IP address for security logging</param>
    /// <param name="userAgent">Client user agent for security logging</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Login response with tokens and user information</returns>
    Task<LoginResponse> LoginAsync(LoginRequest request, string ipAddress, string? userAgent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a new user account
    /// </summary>
    /// <param name="request">Registration information</param>
    /// <param name="ipAddress">Client IP address for security logging</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created user information</returns>
    Task<UserDto> RegisterAsync(RegisterRequest request, string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes an access token using a refresh token
    /// Implements token rotation for enhanced security
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <param name="ipAddress">Client IP address for security logging</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New access and refresh tokens</returns>
    Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs out a user by revoking their tokens
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="request">Logout options</param>
    /// <param name="ipAddress">Client IP address for security logging</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task LogoutAsync(Guid userId, LogoutRequest request, string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes a user's password
    /// Invalidates all existing tokens for security
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="request">Password change request</param>
    /// <param name="ipAddress">Client IP address for security logging</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates password reset process
    /// Sends reset token to user's email
    /// </summary>
    /// <param name="request">Password reset request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Completes password reset using reset token
    /// </summary>
    /// <param name="request">Reset password request</param>
    /// <param name="ipAddress">Client IP address for security logging</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ResetPasswordAsync(ResetPasswordRequest request, string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets up two-factor authentication for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="request">2FA setup request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Setup information including QR code and backup codes</returns>
    Task<SetupTwoFactorResponse> SetupTwoFactorAsync(Guid userId, SetupTwoFactorRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies and enables two-factor authentication
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="request">Verification code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task VerifyAndEnableTwoFactorAsync(Guid userId, VerifyTwoFactorRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disables two-factor authentication
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="request">Disable 2FA request with password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DisableTwoFactorAsync(Guid userId, DisableTwoFactorRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies a two-factor code during login
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="code">Verification code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if code is valid, false otherwise</returns>
    Task<bool> VerifyTwoFactorCodeAsync(Guid userId, string code, CancellationToken cancellationToken = default);
}
