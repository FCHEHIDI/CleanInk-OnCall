using CleanInk.OnCall.Domain.Common;
using CleanInk.OnCall.Domain.Events;
using CleanInk.OnCall.Shared;
using CleanInk.OnCall.Shared.Fhir;

namespace CleanInk.OnCall.Domain.Entities;

/// <summary>
/// Call Center aggregate root — represents an inbound patient or emergency call.
///
/// Domain invariants:
/// 1. "Resolved" or "Cancelled" call → status is IMMUTABLE.
/// 2. A call can only be assigned to a Medecin or Infirmier role.
/// 3. A "Pending" call cannot be escalated — must be InProgress first.
/// 4. Only Medecin + Admin roles can escalate (enforced in command handlers).
/// 5. Every status transition raises a domain event for the audit trail.
///
/// FHIR: Calls loosely map to FHIR Communication or ServiceRequest resources.
/// For calls that result in an encounter, <see cref="EncounterId"/> is set.
/// </summary>
public sealed class Call : Entity<Guid>
{
    private Call() { }

    // ── Relations ────────────────────────────────────────────────────────────

    /// <summary>Patient linked to this call. May be null until identified during triage.</summary>
    public Guid? PatientId { get; private set; }

    /// <summary>Call-center agent who created the call.</summary>
    public Guid CreatedByUserId { get; private set; }

    /// <summary>Practitioner (Medecin or Infirmier) assigned to handle the call.</summary>
    public Guid? AssignedPractitionerId { get; private set; }

    /// <summary>Encounter created from this call, if it resulted in a clinical interaction.</summary>
    public Guid? EncounterId { get; private set; }

    // ── Content ──────────────────────────────────────────────────────────────

    /// <summary>Short subject/reason line (max 200 chars).</summary>
    public string Subject { get; private set; } = string.Empty;

    /// <summary>Full transcribed or typed description.</summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>Triage priority: "routine" | "urgent" | "asap" | "stat".</summary>
    public string Priority { get; private set; } = "routine";

    // ── Status ───────────────────────────────────────────────────────────────

    /// <summary>Current lifecycle status of the call.</summary>
    public CallStatus Status { get; private set; }

    // ── AI enrichment ────────────────────────────────────────────────────────

    /// <summary>AI-generated triage classification tag (e.g. "cardiologie", "urgence").</summary>
    public string? AiTriageTag { get; private set; }

    /// <summary>AI-generated summary of the conversation.</summary>
    public string? AiSummary { get; private set; }

    // ── Metadata ─────────────────────────────────────────────────────────────

    /// <summary>UTC timestamp of call creation.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>UTC timestamp of the last update.</summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Factory — creates a new pending call.
    /// Raises <see cref="CallCreatedEvent"/>.
    /// </summary>
    public static Result<Call> Create(
        Guid createdByUserId,
        string subject,
        string description,
        Guid? patientId = null,
        string priority = "routine")
    {
        if (createdByUserId == Guid.Empty)
            return Error.Validation(nameof(CreatedByUserId), "Created-by user ID is required.");

        if (string.IsNullOrWhiteSpace(subject))
            return Error.Validation(nameof(Subject), "Subject is required.");

        if (subject.Length > 200)
            return Error.Validation(nameof(Subject), "Subject cannot exceed 200 characters.");

        var validPriorities = new[] { "routine", "urgent", "asap", "stat" };
        if (!validPriorities.Contains(priority))
            return Error.Validation(nameof(Priority),
                $"Priority must be one of: {string.Join(", ", validPriorities)}.");

        var now = DateTime.UtcNow;
        var call = new Call
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            CreatedByUserId = createdByUserId,
            Subject = subject.Trim(),
            Description = description.Trim(),
            Priority = priority,
            Status = CallStatus.Pending,
            CreatedAt = now,
            UpdatedAt = now,
        };

        call.RaiseDomainEvent(new CallCreatedEvent(call.Id, patientId, subject, now));

        return call;
    }

    /// <summary>
    /// Assigns a practitioner (Medecin or Infirmier) to handle the call.
    /// Invariant: only those roles may be assigned (enforced by the caller/command handler).
    /// </summary>
    public Result AssignTo(Guid practitionerId)
    {
        if (IsTerminal)
            return Result.Failure(new Error("Call.Immutable", "A resolved or cancelled call cannot be reassigned."));

        if (practitionerId == Guid.Empty)
            return Result.Failure(Error.Validation(nameof(practitionerId), "Practitioner ID is required."));

        AssignedPractitionerId = practitionerId;
        Status = CallStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new CallAssignedEvent(Id, practitionerId, DateTime.UtcNow));

        return Result.Success;
    }

    /// <summary>
    /// Resolves the call. Once resolved, status is IMMUTABLE.
    /// Raises <see cref="CallResolvedEvent"/>.
    /// </summary>
    public Result Resolve(string? resolutionNote = null)
    {
        if (IsTerminal)
            return Result.Failure(new Error("Call.Immutable", "A resolved or cancelled call cannot be resolved again."));

        Status = CallStatus.Resolved;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new CallResolvedEvent(Id, PatientId, DateTime.UtcNow));

        return Result.Success;
    }

    /// <summary>
    /// Escalates the call to a higher tier.
    /// Invariant: a Pending call cannot be escalated — must be InProgress first.
    /// </summary>
    public Result Escalate(string reason)
    {
        if (IsTerminal)
            return Result.Failure(new Error("Call.Immutable", "A resolved or cancelled call cannot be escalated."));

        if (Status == CallStatus.Pending)
            return Result.Failure(new Error("Call.NotAssigned",
                "A call must be InProgress before it can be escalated."));

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure(Error.Validation(nameof(reason), "Escalation reason is required."));

        Status = CallStatus.Escalated;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new CallEscalatedEvent(Id, PatientId, reason, DateTime.UtcNow));

        return Result.Success;
    }

    /// <summary>
    /// Cancels the call.
    /// Invariant: resolved calls cannot be cancelled.
    /// </summary>
    public Result Cancel(string reason)
    {
        if (Status == CallStatus.Resolved)
            return Result.Failure(new Error("Call.Immutable", "A resolved call cannot be cancelled."));

        if (Status == CallStatus.Cancelled)
            return Result.Failure(new Error("Call.AlreadyCancelled", "This call is already cancelled."));

        Status = CallStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success;
    }

    /// <summary>Sets the patient ID once identified during triage.</summary>
    public void LinkPatient(Guid patientId)
    {
        PatientId = patientId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Links the encounter created from this call.</summary>
    public void LinkEncounter(Guid encounterId)
    {
        EncounterId = encounterId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Updates AI enrichment fields. Called by the AI orchestrator after analysis.</summary>
    public void SetAiEnrichment(string? triageTag, string? summary)
    {
        AiTriageTag = triageTag;
        AiSummary = summary;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>True if the call is in a terminal state (Resolved or Cancelled).</summary>
    private bool IsTerminal =>
        Status is CallStatus.Resolved or CallStatus.Cancelled;
}

/// <summary>Call lifecycle status codes.</summary>
public enum CallStatus
{
    /// <summary>Waiting to be assigned to a practitioner.</summary>
    Pending = 0,

    /// <summary>Being handled by an assigned practitioner.</summary>
    InProgress = 1,

    /// <summary>Resolved successfully. IMMUTABLE.</summary>
    Resolved = 2,

    /// <summary>Escalated to a higher tier or specialist.</summary>
    Escalated = 3,

    /// <summary>Cancelled before resolution. IMMUTABLE.</summary>
    Cancelled = 4,
}
