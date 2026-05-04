using CleanInk.OnCall.Domain.Entities;

namespace CleanInk.OnCall.Domain.Repositories;

/// <summary>
/// Repository contract for <see cref="Call"/> aggregate persistence.
/// All queries are scoped to the current tenant schema.
/// </summary>
public interface ICallRepository
{
    Task<Call?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<Call>> GetPagedAsync(
        int page,
        int pageSize,
        Guid? patientId = null,
        Guid? assignedPractitionerId = null,
        CallStatus? status = null,
        CancellationToken ct = default);

    Task<int> CountAsync(
        Guid? patientId = null,
        Guid? assignedPractitionerId = null,
        CallStatus? status = null,
        CancellationToken ct = default);

    Task AddAsync(Call call, CancellationToken ct = default);
    void Update(Call call);
}
