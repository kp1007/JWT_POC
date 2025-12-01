using System.Security.Claims;
using JwtPoc.Core.Entities;

namespace JwtPoc.Core.Interfaces;

/// <summary>
/// Service interface for JWT token operations
/// Handles token generation, validation, and claims extraction
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a JWT access token for a user
    /// </summary>
    /// <param name="user">User entity</param>
    /// <param name="roles">User's roles</param>
    /// <param name="permissions">User's permissions</param>
    /// <returns>JWT token string and expiration time</returns>
    (string Token, DateTime ExpiresAt) GenerateAccessToken(User user, IEnumerable<string> roles, IEnumerable<string> permissions);

    /// <summary>
    /// Generates a refresh token
    /// Returns a cryptographically secure random token
    /// </summary>
    /// <returns>Refresh token string</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Validates a JWT token and extracts claims
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>ClaimsPrincipal if valid, null if invalid</returns>
    ClaimsPrincipal? ValidateToken(string token);

    /// <summary>
    /// Extracts the JTI (JWT ID) claim from a token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>JTI value or null if not found</returns>
    string? GetJwtId(string token);

    /// <summary>
    /// Extracts the user ID from a token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>User ID or null if not found</returns>
    Guid? GetUserIdFromToken(string token);

    /// <summary>
    /// Extracts the security stamp from a token
    /// Used for token invalidation when credentials change
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Security stamp or null if not found</returns>
    string? GetSecurityStamp(string token);

    /// <summary>
    /// Gets the expiration time from a token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Expiration DateTime or null if not found</returns>
    DateTime? GetTokenExpiration(string token);
}
