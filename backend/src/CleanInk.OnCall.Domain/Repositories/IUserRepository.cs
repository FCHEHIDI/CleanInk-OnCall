using CleanInk.OnCall.Domain.Entities;

namespace CleanInk.OnCall.Domain.Repositories;

/// <summary>
/// Repository contract for <see cref="User"/> aggregate persistence.
/// All queries are automatically scoped to the current tenant schema via <see cref="CleanInk.OnCall.Application.Common.Interfaces.ITenantContext"/>.
/// </summary>
public interface IUserRepository
{
    /// <summary>Retrieves a user by their unique identifier.</summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The user, or <c>null</c> if not found.</returns>
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Retrieves a user by their email address.</summary>
    /// <param name="email">The email address (case-insensitive).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The user, or <c>null</c> if not found.</returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);

    /// <summary>Checks whether an email is already registered.</summary>
    /// <param name="email">Email address to check.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns><c>true</c> if the email already exists.</returns>
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);

    /// <summary>Adds a new user to the repository.</summary>
    /// <param name="user">The user aggregate to persist.</param>
    /// <param name="ct">Cancellation token.</param>
    Task AddAsync(User user, CancellationToken ct = default);

    /// <summary>Updates an existing user.</summary>
    /// <param name="user">The user aggregate to update.</param>
    void Update(User user);

    /// <summary>Persists all pending changes.</summary>
    /// <param name="ct">Cancellation token.</param>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
