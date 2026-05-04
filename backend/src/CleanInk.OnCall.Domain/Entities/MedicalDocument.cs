using CleanInk.OnCall.Domain.Common;
using CleanInk.OnCall.Shared;
using CleanInk.OnCall.Shared.Fhir;

namespace CleanInk.OnCall.Domain.Entities;

/// <summary>
/// FHIR R4 DocumentReference — metadata for a medical document stored externally.
///
/// FHIR mapping: https://www.hl7.org/fhir/documentreference.html
///
/// Architecture:
/// - Only METADATA is stored in PostgreSQL. Binary content is in blob storage (S3/Azure Blob).
/// - The storage key (<see cref="StorageKey"/>) is a reference to the blob — never a URL.
/// - URLs are generated on-demand by the DocumentService using short-lived pre-signed URLs.
/// - This prevents direct access to storage and enforces access control at the API layer.
///
/// RGPD / HDS:
/// - Medical documents are health data — they fall under HDS hosting requirements.
/// - Access to documents generates an AuditEvent automatically.
/// - Soft-delete only: no physical deletion.
/// </summary>
public sealed class MedicalDocument : Entity<Guid>
{
    private MedicalDocument() { }

    // ── FHIR DocumentReference.subject ──────────────────────────────────────

    /// <summary>Patient this document belongs to.</summary>
    public Guid PatientId { get; private set; }

    // ── FHIR DocumentReference.context.encounter ────────────────────────────

    /// <summary>Encounter during which this document was created or referenced.</summary>
    public Guid? EncounterId { get; private set; }

    // ── FHIR DocumentReference.author ───────────────────────────────────────

    /// <summary>Practitioner who authored or uploaded the document.</summary>
    public Guid AuthoredByUserId { get; private set; }

    // ── FHIR DocumentReference.type ─────────────────────────────────────────

    /// <summary>Document type: "ordonnance" | "compte-rendu" | "imagerie" | "biologie" | "consentement" | "autre".</summary>
    public string DocumentType { get; private set; } = string.Empty;

    // ── FHIR DocumentReference.content ──────────────────────────────────────

    /// <summary>Original filename as uploaded by the user.</summary>
    public string FileName { get; private set; } = string.Empty;

    /// <summary>MIME type (e.g. "application/pdf", "image/jpeg").</summary>
    public string MimeType { get; private set; } = string.Empty;

    /// <summary>File size in bytes.</summary>
    public long SizeBytes { get; private set; }

    /// <summary>
    /// Opaque blob storage key (e.g. "tenants/abc123/patients/xyz/documents/uuid.pdf").
    /// NEVER a public URL — URLs are generated on-demand via pre-signed tokens.
    /// </summary>
    public string StorageKey { get; private set; } = string.Empty;

    // ── FHIR DocumentReference.status ───────────────────────────────────────

    /// <summary>Document status: "current" | "superseded" | "entered-in-error".</summary>
    public string Status { get; private set; } = "current";

    // ── Metadata ─────────────────────────────────────────────────────────────

    /// <summary>UTC timestamp when the document was created.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>Soft-delete flag.</summary>
    public bool IsDeleted { get; private set; }

    /// <summary>
    /// Factory — creates a new document reference after a successful blob upload.
    /// </summary>
    public static Result<MedicalDocument> Create(
        Guid patientId,
        Guid authoredByUserId,
        string documentType,
        string fileName,
        string mimeType,
        long sizeBytes,
        string storageKey,
        Guid? encounterId = null)
    {
        if (patientId == Guid.Empty)
            return Error.Validation(nameof(PatientId), "Patient ID is required.");

        if (authoredByUserId == Guid.Empty)
            return Error.Validation(nameof(AuthoredByUserId), "Author user ID is required.");

        if (string.IsNullOrWhiteSpace(storageKey))
            return Error.Validation(nameof(StorageKey), "Storage key is required.");

        if (string.IsNullOrWhiteSpace(fileName))
            return Error.Validation(nameof(FileName), "File name is required.");

        if (sizeBytes <= 0)
            return Error.Validation(nameof(SizeBytes), "File size must be positive.");

        return new MedicalDocument
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            EncounterId = encounterId,
            AuthoredByUserId = authoredByUserId,
            DocumentType = documentType.Trim().ToLowerInvariant(),
            FileName = fileName.Trim(),
            MimeType = mimeType.Trim().ToLowerInvariant(),
            SizeBytes = sizeBytes,
            StorageKey = storageKey,
            Status = "current",
            CreatedAt = DateTime.UtcNow,
        };
    }

    /// <summary>Marks the document as deleted (soft-delete). No physical removal.</summary>
    public void SoftDelete() => IsDeleted = true;
}
