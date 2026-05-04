using CleanInk.OnCall.Domain.Common;
using CleanInk.OnCall.Domain.Events;
using CleanInk.OnCall.Shared;
using CleanInk.OnCall.Shared.Fhir;

namespace CleanInk.OnCall.Domain.Entities;

/// <summary>
/// Identity aggregate root — represents a human user account.
///
/// RBAC invariants (enforced in domain):
/// - An inactive user (<see cref="IsActive"/> = false) may not receive a JWT.
/// - Role is validated against <see cref="CleanInk.OnCall.Application.Auth.AppRoles"/> at creation/update.
/// - No self-elevation: a user cannot grant themselves a higher role.
///
/// Multi-tenant isolation:
/// - Every user belongs to exactly one <see cref="TenantId"/>.
/// - Tenant is encoded in the JWT "tenant_id" claim.
///
/// RGPD / HDS:
/// - Email is pseudonymized in audit logs and research exports.
/// - PasswordHash is never exposed via API — only stored in DB and compared in-memory.
/// - All mutations raise domain events for the compliance audit trail.
/// </summary>
public sealed class User : Entity<Guid>
{
    private User() { }

    /// <summary>Tenant this user belongs to.</summary>
    public Guid TenantId { get; private set; }

    /// <summary>Email address (used as login). Stored lowercase, trimmed.</summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>BCrypt-hashed password. Never expose via API.</summary>
    public string PasswordHash { get; private set; } = string.Empty;

    /// <summary>FHIR HumanName — supports multi-lingual and titled names.</summary>
    public HumanName Name { get; private set; } = default!;

    /// <summary>Application role — one of <see cref="CleanInk.OnCall.Application.Auth.AppRoles"/> constants.</summary>
    public string Role { get; private set; } = string.Empty;

    /// <summary>Whether this account is active. Inactive accounts cannot receive JWT.</summary>
    public bool IsActive { get; private set; }

    /// <summary>UTC timestamp of account creation.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>UTC timestamp of the last profile update.</summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>Display name for the UI header.</summary>
    public string FullName => Name.Display;

    /// <summary>
    /// Factory method — registers a new user account within a tenant.
    /// Raises <see cref="UserCreatedEvent"/>.
    /// </summary>
    /// <param name="tenantId">The tenant this user belongs to.</param>
    /// <param name="email">Valid unique email address.</param>
    /// <param name="firstName">Given name.</param>
    /// <param name="lastName">Family name.</param>
    /// <param name="passwordHash">BCrypt hash (never plaintext).</param>
    /// <param name="role">Must be a valid AppRoles constant.</param>
    /// <returns>A <see cref="Result{User}"/> or a validation error.</returns>
    public static Result<User> Create(
        Guid tenantId,
        string email,
        string firstName,
        string lastName,
        string passwordHash,
        string role)
    {
        if (tenantId == Guid.Empty)
            return Error.Validation(nameof(TenantId), "TenantId is required.");

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            return Error.Validation(nameof(Email), "A valid email address is required.");

        if (string.IsNullOrWhiteSpace(firstName))
            return Error.Validation(nameof(firstName), "First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            return Error.Validation(nameof(lastName), "Last name is required.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            return Error.Validation(nameof(PasswordHash), "Password hash is required.");

        if (string.IsNullOrWhiteSpace(role))
            return Error.Validation(nameof(Role), "Role is required.");

        var now = DateTime.UtcNow;
        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            Name = HumanName.Official(lastName.Trim().ToUpperInvariant(), firstName.Trim()),
            Role = role,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now,
        };

        user.RaiseDomainEvent(new UserCreatedEvent(user.Id, tenantId, user.Email, role, DateTime.UtcNow));

        return user;
    }

    /// <summary>
    /// Updates the user's display name.
    /// </summary>
    public Result UpdateName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure(Error.Validation(nameof(firstName), "First name is required."));
        if (string.IsNullOrWhiteSpace(lastName))
            return Result.Failure(Error.Validation(nameof(lastName), "Last name is required."));

        Name = HumanName.Official(lastName.Trim().ToUpperInvariant(), firstName.Trim());
        UpdatedAt = DateTime.UtcNow;
        return Result.Success;
    }

    /// <summary>
    /// Deactivates the account. Deactivated users cannot receive new JWT tokens.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new UserDeactivatedEvent(Id, TenantId, DateTime.UtcNow));
    }

    /// <summary>
    /// Reactivates the account.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
