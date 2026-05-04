using CleanInk.OnCall.Domain.Common;

namespace CleanInk.OnCall.Domain.Entities;

/// <summary>
/// FHIR R4 AuditEvent — append-only compliance record.
///
/// FHIR mapping: https://www.hl7.org/fhir/auditevent.html
///
/// HDS (Hébergeur de Données de Santé) requirements:
/// - All access to and modifications of health data MUST be logged.
/// - Logs must be retained for at least 20 years per French health data regulations (Art. R. 1112-7).
/// - Audit log entries are NEVER modified or deleted — append-only by construction.
/// - In production: stored in an immutable table with INSERT-only DB role.
///
/// Domain invariants:
/// - This entity has NO mutation methods — it is append-only by construction.
/// - TenantId is stored explicitly here even though we're in a schema-per-tenant model
///   to support cross-tenant compliance queries by the Global Compliance officer.
/// - EmergencyAccess events are flagged separately for mandatory review.
///
/// Replaces: AuditLog entity from v1.
/// </summary>
public sealed class AuditEvent : Entity<Guid>
{
    private AuditEvent() { }

    // ── FHIR AuditEvent.type ─────────────────────────────────────────────────

    /// <summary>
    /// Audit event type code.
    /// DICOM terminology: "110100" (Application Activity), "110110" (Patient Record), etc.
    /// Internal extension: "EMERGENCY_ACCESS", "LOGIN", "EXPORT", "ROLE_CHANGE", etc.
    /// </summary>
    public string EventType { get; private set; } = string.Empty;

    /// <summary>Specific action: "C" (Create), "R" (Read), "U" (Update), "D" (Delete), "E" (Execute).</summary>
    public string Action { get; private set; } = string.Empty;

    // ── FHIR AuditEvent.recorded ─────────────────────────────────────────────

    /// <summary>UTC timestamp when the event was recorded. Immutable.</summary>
    public DateTime RecordedAt { get; private set; }

    // ── FHIR AuditEvent.outcome ──────────────────────────────────────────────

    /// <summary>Outcome: "0" (Success), "4" (Minor failure), "8" (Serious failure), "12" (Major failure).</summary>
    public string Outcome { get; private set; } = "0";

    // ── FHIR AuditEvent.agent ────────────────────────────────────────────────

    /// <summary>ID of the user who performed the action. Null for system events.</summary>
    public Guid? ActorId { get; private set; }

    /// <summary>Email of the actor at event time (retained even if account is deleted).</summary>
    public string ActorEmail { get; private set; } = string.Empty;

    /// <summary>Role of the actor at the time of the event.</summary>
    public string ActorRole { get; private set; } = string.Empty;

    // ── FHIR AuditEvent.entity ───────────────────────────────────────────────

    /// <summary>Type of resource affected: "Patient", "Encounter", "Invoice", "User", etc.</summary>
    public string ResourceType { get; private set; } = string.Empty;

    /// <summary>ID of the affected resource.</summary>
    public Guid? ResourceId { get; private set; }

    // ── Emergency access ─────────────────────────────────────────────────────

    /// <summary>
    /// Whether this event represents an emergency clinical access override.
    /// Emergency access events require mandatory review within 24h.
    /// </summary>
    public bool IsEmergencyAccess { get; private set; }

    /// <summary>Mandatory justification for emergency access events.</summary>
    public string? EmergencyJustification { get; private set; }

    // ── Context ──────────────────────────────────────────────────────────────

    /// <summary>Tenant ID — explicit even in schema-per-tenant model for compliance queries.</summary>
    public Guid TenantId { get; private set; }

    /// <summary>Serialized additional details (JSON). Never contains PII in plaintext.</summary>
    public string? Details { get; private set; }

    /// <summary>Source IP address of the request.</summary>
    public string? IpAddress { get; private set; }

    // ── Factory (the ONLY way to create an AuditEvent) ──────────────────────

    /// <summary>
    /// Creates a standard audit event.
    /// This is the only mutation point — the entity has no update methods by design.
    /// </summary>
    public static AuditEvent Record(
        Guid tenantId,
        string eventType,
        string action,
        string resourceType,
        Guid? actorId = null,
        string actorEmail = "",
        string actorRole = "",
        Guid? resourceId = null,
        string? details = null,
        string? ipAddress = null,
        string outcome = "0")
    {
        return new AuditEvent
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            EventType = eventType,
            Action = action,
            ResourceType = resourceType,
            ActorId = actorId,
            ActorEmail = actorEmail,
            ActorRole = actorRole,
            ResourceId = resourceId,
            Details = details,
            IpAddress = ipAddress,
            Outcome = outcome,
            RecordedAt = DateTime.UtcNow,
            IsEmergencyAccess = false,
        };
    }

    /// <summary>
    /// Creates an emergency access audit event (Médecin override on restricted patient data).
    /// Mandatory justification required per HDS compliance.
    /// </summary>
    public static AuditEvent RecordEmergencyAccess(
        Guid tenantId,
        Guid actorId,
        string actorEmail,
        Guid patientId,
        string justification,
        string? ipAddress = null)
    {
        if (string.IsNullOrWhiteSpace(justification))
            throw new ArgumentException("Emergency access justification is mandatory.", nameof(justification));

        return new AuditEvent
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            EventType = "EMERGENCY_ACCESS",
            Action = "R",
            ResourceType = "Patient",
            ActorId = actorId,
            ActorEmail = actorEmail,
            ActorRole = "Medecin",
            ResourceId = patientId,
            IsEmergencyAccess = true,
            EmergencyJustification = justification,
            IpAddress = ipAddress,
            Outcome = "0",
            RecordedAt = DateTime.UtcNow,
        };
    }
}

/// <summary>
/// Backward compatibility alias. Use <see cref="AuditEvent"/> in new code.
/// </summary>
[Obsolete("Use AuditEvent instead of AuditLog.")]
public sealed class AuditLog : Entity<Guid>
{
    private AuditLog() { }
    public Guid? ActorId { get; private set; }
    public string ActorEmail { get; private set; } = string.Empty;
    public string Action { get; private set; } = string.Empty;
    public string ResourceType { get; private set; } = string.Empty;
    public Guid? ResourceId { get; private set; }
    public string? Details { get; private set; }
    public DateTime Timestamp { get; private set; }
}
