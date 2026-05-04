namespace CleanInk.OnCall.Domain.Events;

public sealed record PatientRegisteredEvent(Guid PatientId, DateTime OccurredAt) : IDomainEvent;
public sealed record PatientConsentGivenEvent(Guid PatientId, DateTime OccurredAt) : IDomainEvent;
public sealed record PatientPseudonymizedEvent(Guid PatientId, DateTime OccurredAt) : IDomainEvent;
public sealed record UserCreatedEvent(Guid UserId, Guid TenantId, string Email, string Role, DateTime OccurredAt) : IDomainEvent;
public sealed record UserDeactivatedEvent(Guid UserId, Guid TenantId, DateTime OccurredAt) : IDomainEvent;
public sealed record TenantCreatedEvent(Guid TenantId, string Name, string Slug, DateTime OccurredAt) : IDomainEvent;
public sealed record TenantActivatedEvent(Guid TenantId, DateTime OccurredAt) : IDomainEvent;
public sealed record EncounterStartedEvent(Guid EncounterId, Guid PatientId, Guid PractitionerId, DateTime OccurredAt) : IDomainEvent;
public sealed record EncounterFinishedEvent(Guid EncounterId, Guid PatientId, Guid PractitionerId, DateTime OccurredAt) : IDomainEvent;
public sealed record InvoiceCreatedEvent(Guid InvoiceId, Guid PatientId, int TotalCents, DateTime OccurredAt) : IDomainEvent;
public sealed record InvoicePaidEvent(Guid InvoiceId, Guid PatientId, int AmountCents, string PaymentMethod, DateTime OccurredAt) : IDomainEvent;
