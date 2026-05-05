using CleanInk.OnCall.Domain.Entities;

namespace CleanInk.OnCall.Domain.Repositories;

/// <summary>
/// Repository contract for <see cref="Invoice"/> aggregate persistence.
/// All queries are scoped to the current tenant schema.
/// </summary>
public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Invoice?> GetByReferenceAsync(string reference, CancellationToken ct = default);
    Task<bool> ReferenceExistsAsync(string reference, CancellationToken ct = default);

    Task<IReadOnlyList<Invoice>> GetByPatientAsync(
        Guid patientId,
        InvoiceStatus? status = null,
        CancellationToken ct = default);

    Task<IReadOnlyList<Invoice>> GetAllAsync(
        InvoiceStatus? status = null,
        int page = 1,
        int pageSize = 100,
        CancellationToken ct = default);

    Task AddAsync(Invoice invoice, CancellationToken ct = default);
    void Update(Invoice invoice);
}
