using CleanInk.OnCall.Domain.Common;
using CleanInk.OnCall.Shared;

namespace CleanInk.OnCall.Domain.Entities;

/// <summary>
/// Represents a user account in the CleanInk OnCall system.
/// A user can be a customer, an operator, or an admin.
/// </summary>
public sealed class User : Entity<Guid>
{
    private User() { }

    /// <summary>Gets the user's email address (used as login).</summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>Gets the user's first name.</summary>
    public string FirstName { get; private set; } = string.Empty;

    /// <summary>Gets the user's last name.</summary>
    public string LastName { get; private set; } = string.Empty;

    /// <summary>Gets the bcrypt-hashed password. Never expose this via API.</summary>
    public string PasswordHash { get; private set; } = string.Empty;

    /// <summary>Gets the user's role (e.g. "Customer", "Operator", "Admin").</summary>
    public string Role { get; private set; } = string.Empty;

    /// <summary>Gets a value indicating whether this account is active.</summary>
    public bool IsActive { get; private set; }

    /// <summary>Gets the UTC timestamp of account creation.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>Gets the UTC timestamp of the last profile update.</summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>Gets the user's full name (FirstName + LastName).</summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Factory method to create a new <see cref="User"/>.
    /// </summary>
    /// <param name="email">Valid email address.</param>
    /// <param name="firstName">First name (max 100 chars).</param>
    /// <param name="lastName">Last name (max 100 chars).</param>
    /// <param name="passwordHash">Pre-hashed password (bcrypt). Never pass plaintext.</param>
    /// <param name="role">Role string (e.g. "Customer", "Operator", "Admin").</param>
    /// <returns>A <see cref="Result{User}"/> with the new user or validation errors.</returns>
    public static Result<User> Create(
        string email,
        string firstName,
        string lastName,
        string passwordHash,
        string role)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            return Error.Validation(nameof(Email), "A valid email address is required.");

        if (string.IsNullOrWhiteSpace(firstName))
            return Error.Validation(nameof(FirstName), "First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            return Error.Validation(nameof(LastName), "Last name is required.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            return Error.Validation(nameof(PasswordHash), "Password hash is required.");

        if (string.IsNullOrWhiteSpace(role))
            return Error.Validation(nameof(Role), "Role is required.");

        var now = DateTime.UtcNow;
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email.Trim().ToLowerInvariant(),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            PasswordHash = passwordHash,
            Role = role,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        return user;
    }

    /// <summary>Updates the user's profile information.</summary>
    /// <param name="firstName">New first name.</param>
    /// <param name="lastName">New last name.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    public Result UpdateProfile(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure(Error.Validation(nameof(FirstName), "First name is required."));

        if (string.IsNullOrWhiteSpace(lastName))
            return Result.Failure(Error.Validation(nameof(LastName), "Last name is required."));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        UpdatedAt = DateTime.UtcNow;
        return Result.Success;
    }

    /// <summary>Deactivates this user account.</summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Reactivates this user account.</summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
