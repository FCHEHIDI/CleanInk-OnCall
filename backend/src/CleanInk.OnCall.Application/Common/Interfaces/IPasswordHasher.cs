namespace CleanInk.OnCall.Application.Common.Interfaces;

/// <summary>
/// Contract for password hashing and verification.
/// Implemented in Infrastructure using BCrypt to keep hashing algorithm
/// details out of the Application layer.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Produces a BCrypt hash of the given plaintext password.
    /// </summary>
    /// <param name="plaintext">The plaintext password to hash.</param>
    /// <returns>The BCrypt hash string (includes salt).</returns>
    string Hash(string plaintext);

    /// <summary>
    /// Verifies a plaintext password against a stored BCrypt hash.
    /// Uses timing-safe comparison to prevent timing attacks.
    /// </summary>
    /// <param name="plaintext">The plaintext candidate password.</param>
    /// <param name="hash">The stored BCrypt hash.</param>
    /// <returns><c>true</c> if the password matches the hash.</returns>
    bool Verify(string plaintext, string hash);
}
