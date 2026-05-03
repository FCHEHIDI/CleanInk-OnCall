using CleanInk.OnCall.Domain.Common;

namespace CleanInk.OnCall.Domain.Entities;

/// <summary>
/// Immutable audit log entry — records every significant state change in the system.
///
/// HDS (Hébergeur de Données de Santé) requirement: all access to and modifications
/// of health data must be logged with actor identity, timestamp, resource, and change details.
/// Audit logs must be retained for at least 10 years per French health data regulations.
///
/// Design notes:
/// - Audit log entries are NEVER modified or deleted programmatically.
/// - Stored in a separate table with append-only insert privileges in production.
/// - Sensitive field values are pseudonymized before logging (PIIMasker).
/// </summary>
public sealed class AuditLog : Entity<Guid>
{
    // Private parameterless ctor for EF Core.
    private AuditLog() { }

    /// <summary>ID of the user who performed the action (null for system operations).</summary>
    public Guid? ActorId { get; private set; }

    /// <summary>Email of the actor at the time of the action (for historical audit even if account is deleted).</summary>
    public string ActorEmail { get; private set; } = string.Empty;

    /// <summary>
    /// The action performed: "Create", "Update", "Delete", "Login", "EscalateCall", "CloseCall", etc.
    /// </summary>
    public string Action { get; private set; } = string.Empty;

    /// <summary>Entity type that was affected: "Call", "Patient", "User", "Invoice".</summary>
    public string EntityType { get; private set; } = string.Empty;

    /// <summary>ID of the affected entity.</summary>
    public string EntityId { get; private set; } = string.Empty;

    /// <summary>
    /// JSON-serialized snapshot of the entity BEFORE the change.
    /// Null for Create operations.
    /// PII fields are pseudonymized before storage.
    /// </summary>
    public string? OldValues { get; private set; }

    /// <summary>
    /// JSON-serialized snapshot of the entity AFTER the change.
    /// Null for Delete operations.
    /// PII fields are pseudonymized before storage.
    /// </summary>
    public string? NewValues { get; private set; }

    /// <summary>Client IP address at the time of the operation (for forensic audit trail).</summary>
    public string? IpAddress { get; private set; }

    /// <summary>UTC timestamp when the audit entry was created.</summary>
    public DateTime OccurredAt { get; private set; }

    /// <summary>
    /// Factory method: creates a new immutable audit log entry.
    /// </summary>
    /// <param name="actorId">ID of the user performing the action (null for system).</param>
    /// <param name="actorEmail">Email of the actor.</param>
    /// <param name="action">Verb describing the action (e.g. "Create", "Update", "EscalateCall").</param>
    /// <param name="entityType">Name of the affected entity type.</param>
    /// <param name="entityId">String representation of the entity ID.</param>
    /// <param name="oldValues">Optional JSON of old state (PII-masked).</param>
    /// <param name="newValues">Optional JSON of new state (PII-masked).</param>
    /// <param name="ipAddress">Optional client IP address.</param>
    /// <returns>A fully initialized <see cref="AuditLog"/> entry.</returns>
    public static AuditLog Create(
        Guid? actorId,
        string actorEmail,
        string action,
        string entityType,
        string entityId,
        string? oldValues = null,
        string? newValues = null,
        string? ipAddress = null)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            ActorId = actorId,
            ActorEmail = actorEmail,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            IpAddress = ipAddress,
            OccurredAt = DateTime.UtcNow
        };
    }
}
