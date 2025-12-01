using JwtPoc.Core.Interfaces;
using BCrypt.Net;

namespace JwtPoc.Infrastructure.Services;

/// <summary>
/// Password hashing service using BCrypt
/// BCrypt is a secure hashing algorithm designed for password storage
/// Work factor of 12 provides good security/performance balance
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    public string HashPassword(string password)
    {
        // BCrypt automatically generates a salt and includes it in the hash
        // Work factor determines computational cost (higher = more secure but slower)
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    public bool VerifyPassword(string password, string hash)
    {
        try
        {
            // BCrypt.Verify extracts the salt from the hash and compares
            // This is why we don't need to store the salt separately
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            // Invalid hash format
            return false;
        }
    }

    public bool NeedsRehash(string hash)
    {
        try
        {
            // Check if the hash was created with a different work factor
            return BCrypt.Net.BCrypt.PasswordNeedsRehash(hash, WorkFactor);
        }
        catch
        {
            // If we can't determine, assume it needs rehashing
            return true;
        }
    }
}
