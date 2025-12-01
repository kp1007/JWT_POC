namespace JwtPoc.Core.Interfaces;

/// <summary>
/// Service interface for password hashing operations
/// Uses BCrypt for secure password hashing
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a plain text password using BCrypt
    /// Uses a work factor for computational cost
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <returns>Hashed password</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a plain text password against a hash
    /// </summary>
    /// <param name="password">Plain text password to verify</param>
    /// <param name="hash">Hashed password to compare against</param>
    /// <returns>True if password matches, false otherwise</returns>
    bool VerifyPassword(string password, string hash);

    /// <summary>
    /// Checks if a password hash needs rehashing
    /// Used when security requirements change (e.g., increased work factor)
    /// </summary>
    /// <param name="hash">Password hash to check</param>
    /// <returns>True if rehashing is needed, false otherwise</returns>
    bool NeedsRehash(string hash);
}
