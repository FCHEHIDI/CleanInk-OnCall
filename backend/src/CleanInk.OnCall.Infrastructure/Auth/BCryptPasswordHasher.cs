using CleanInk.OnCall.Application.Common.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace CleanInk.OnCall.Infrastructure.Auth;

/// <summary>
/// BCrypt-based password hasher.
/// Work factor 12 provides ~250ms hash time on modern hardware —
/// a deliberate slowdown that makes brute-force attacks expensive.
/// </summary>
public sealed class BCryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    /// <inheritdoc/>
    public string Hash(string plaintext)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(plaintext);
        return BC.HashPassword(plaintext, workFactor: WorkFactor);
    }

    /// <inheritdoc/>
    public bool Verify(string plaintext, string hash)
    {
        if (string.IsNullOrEmpty(plaintext) || string.IsNullOrEmpty(hash))
            return false;

        try
        {
            return BC.Verify(plaintext, hash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            // Invalid hash format — treat as verification failure.
            return false;
        }
    }
}
