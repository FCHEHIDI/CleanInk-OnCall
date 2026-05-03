using CleanInk.OnCall.Domain.Common;
using CleanInk.OnCall.Domain.Events;
using CleanInk.OnCall.Shared;

namespace CleanInk.OnCall.Domain.Entities;

/// <summary>
/// Represents the status of a call in the call-center workflow.
/// </summary>
public enum CallStatus
{
    /// <summary>Call is waiting to be assigned.</summary>
    Pending = 0,

    /// <summary>Call is currently being handled by an operator.</summary>
    InProgress = 1,

    /// <summary>Call has been resolved successfully.</summary>
    Resolved = 2,

    /// <summary>Call was escalated to a higher tier.</summary>
    Escalated = 3,

    /// <summary>Call was cancelled before resolution.</summary>
    Cancelled = 4
}

/// <summary>
/// Aggregate root representing an inbound customer call in the CleanInk OnCall system.
/// Tracks the full lifecycle from reception through resolution.
/// </summary>
public sealed class Call : Entity<Guid>
{
    // Private constructor required by EF Core.
    private Call() { }

    /// <summary>Gets the ID of the customer who placed the call.</summary>
    public Guid CustomerId { get; private set; }

    /// <summary>
    /// Optional link to the identified <see cref="Patient"/> aggregate.
    /// Null until the patient is identified during triage.
    /// </summary>
    public Guid? PatientId { get; private set; }

    /// <summary>Gets the ID of the operator handling the call, if assigned.</summary>
    public Guid? OperatorId { get; private set; }

    /// <summary>Gets the short subject/reason of the call.</summary>
    public string Subject { get; private set; } = string.Empty;

    /// <summary>Gets the full transcribed or typed description of the call.</summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>Gets the current lifecycle status of the call.</summary>
    public CallStatus Status { get; private set; }

    /// <summary>Gets the AI-generated triage classification tag (e.g. "billing", "technical").</summary>
    public string? AiTriageTag { get; private set; }

    /// <summary>Gets the AI-generated summary of the conversation.</summary>
    public string? AiSummary { get; private set; }

    /// <summary>Gets the UTC timestamp when the call was created.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>Gets the UTC timestamp of the last update.</summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Factory method to create a new <see cref="Call"/> aggregate.
    /// </summary>
    /// <param name="customerId">ID of the customer.</param>
    /// <param name="subject">Short subject line (max 200 chars).</param>
    /// <param name="description">Full call description.</param>
    /// <returns>A <see cref="Result{Call}"/> with the new call or a validation error.</returns>
    public static Result<Call> Create(Guid customerId, string subject, string description)
    {
        if (customerId == Guid.Empty)
            return Error.Validation(nameof(CustomerId), "CustomerId cannot be empty.");

        if (string.IsNullOrWhiteSpace(subject))
            return Error.Validation(nameof(Subject), "Subject is required.");

        if (subject.Length > 200)
            return Error.Validation(nameof(Subject), "Subject cannot exceed 200 characters.");

        var now = DateTime.UtcNow;
        var call = new Call
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Subject = subject.Trim(),
            Description = description?.Trim() ?? string.Empty,
            Status = CallStatus.Pending,
            CreatedAt = now,
            UpdatedAt = now
        };

        call.RaiseDomainEvent(new CallCreatedEvent(call.Id, null, call.Subject, now));
        return call;
    }

    /// <summary>Assigns the call to an operator and moves it to <see cref="CallStatus.InProgress"/>.</summary>
    /// <param name="operatorId">ID of the operator to assign.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    public Result Assign(Guid operatorId)
    {
        if (operatorId == Guid.Empty)
            return Result.Failure(Error.Validation(nameof(OperatorId), "OperatorId cannot be empty."));

        if (Status != CallStatus.Pending)
            return Result.Failure(new Error("Call.InvalidTransition",
                $"Cannot assign a call in status '{Status}'."));

        OperatorId = operatorId;
        Status = CallStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new CallAssignedEvent(Id, operatorId, UpdatedAt));
        return Result.Success;
    }

    /// <summary>Marks the call as resolved.</summary>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    public Result Resolve()
    {
        if (Status != CallStatus.InProgress)
            return Result.Failure(new Error("Call.InvalidTransition",
                $"Cannot resolve a call in status '{Status}'."));

        Status = CallStatus.Resolved;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success;
    }

    /// <summary>
    /// Escalates the call to a senior clinician or doctor.
    /// Raises <see cref="CallEscalatedEvent"/>.
    /// </summary>
    /// <param name="escalatedBy">ID of the operator triggering escalation.</param>
    /// <param name="reason">Clinical reason for escalation (mandatory).</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    public Result Escalate(Guid escalatedBy, string reason)
    {
        if (Status is CallStatus.Resolved or CallStatus.Cancelled)
            return Result.Failure(new Error("Call.InvalidTransition",
                $"Cannot escalate a call in status '{Status}'."));

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure(Error.Validation(nameof(reason), "Escalation reason is required."));

        Status = CallStatus.Escalated;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new CallEscalatedEvent(Id, escalatedBy, reason, UpdatedAt));
        return Result.Success;
    }

    /// <summary>
    /// Closes the call with a mandatory clinical summary.
    /// Raises <see cref="CallClosedEvent"/>.
    /// </summary>
    /// <param name="closedBy">ID of the operator closing the call.</param>
    /// <param name="summary">Mandatory summary of the call outcome (min 10 chars).</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    public Result Close(Guid closedBy, string summary)
    {
        if (Status is CallStatus.Cancelled)
            return Result.Failure(new Error("Call.InvalidTransition", "Cannot close a cancelled call."));

        if (Status is CallStatus.Resolved)
            return Result.Failure(new Error("Call.InvalidTransition", "Call is already closed."));

        if (string.IsNullOrWhiteSpace(summary) || summary.Length < 10)
            return Result.Failure(Error.Validation(nameof(summary),
                "Call closure requires a summary of at least 10 characters."));

        AiSummary = summary;
        Status = CallStatus.Resolved;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new CallClosedEvent(Id, closedBy, summary, UpdatedAt));
        return Result.Success;
    }

    /// <summary>Attaches AI triage and summary results to this call.</summary>
    /// <param name="triageTag">Classification tag produced by <c>TriageAgent</c>.</param>
    /// <param name="summary">Summary produced by <c>SummaryAgent</c>.</param>
    public void AttachAiInsights(string? triageTag, string? summary)
    {
        AiTriageTag = triageTag;
        AiSummary = summary;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Links this call to an identified patient.</summary>
    /// <param name="patientId">The identified patient's ID.</param>
    public void LinkPatient(Guid patientId)
    {
        PatientId = patientId;
        UpdatedAt = DateTime.UtcNow;
    }
}
