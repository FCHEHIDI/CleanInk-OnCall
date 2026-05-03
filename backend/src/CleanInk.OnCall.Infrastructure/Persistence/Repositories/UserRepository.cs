using CleanInk.OnCall.Domain.Entities;
using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Infrastructure.Persistence;
using CleanInk.OnCall.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CleanInk.OnCall.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IUserRepository"/>.
/// </summary>
public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<UserRepository> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="UserRepository"/>.
    /// </summary>
    /// <param name="db">The EF Core database context.</param>
    /// <param name="logger">Logger instance.</param>
    public UserRepository(AppDbContext db, ILogger<UserRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogDebug("Fetching user {UserId}", id);
        return await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    /// <inheritdoc/>
    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var normalized = email.ToLowerInvariant();
        return await _db.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == normalized, ct);
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
    {
        var normalized = email.ToLowerInvariant();
        return await _db.Users.AnyAsync(u => u.Email == normalized, ct);
    }

    /// <inheritdoc/>
    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await _db.Users.AddAsync(user, ct);
    }

    /// <inheritdoc/>
    public void Update(User user)
    {
        _db.Users.Update(user);
    }

    /// <inheritdoc/>
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return _db.SaveChangesAsync(ct);
    }
}
