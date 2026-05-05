using CleanInk.OnCall.Domain.Entities;
using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CleanInk.OnCall.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IEncounterRepository"/>.
/// </summary>
public sealed class EncounterRepository : IEncounterRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<EncounterRepository> _logger;

    /// <summary>Initializes a new instance of <see cref="EncounterRepository"/>.</summary>
    public EncounterRepository(AppDbContext db, ILogger<EncounterRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Encounter?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogDebug("Fetching encounter {EncounterId}", id);
        return await _db.Encounters.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Encounter>> GetByPatientAsync(
        Guid patientId,
        EncounterStatus? status = null,
        CancellationToken ct = default)
    {
        var query = _db.Encounters.AsNoTracking().Where(e => e.PatientId == patientId);
        if (status.HasValue)
            query = query.Where(e => e.Status == status.Value);
        return await query.OrderByDescending(e => e.Period.Start).ToListAsync(ct);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Encounter>> GetByPractitionerAsync(
        Guid practitionerId,
        EncounterStatus? status = null,
        CancellationToken ct = default)
    {
        var query = _db.Encounters.AsNoTracking().Where(e => e.PractitionerId == practitionerId);
        if (status.HasValue)
            query = query.Where(e => e.Status == status.Value);
        return await query.OrderByDescending(e => e.Period.Start).ToListAsync(ct);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Encounter>> GetAllAsync(
        EncounterStatus? status = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = _db.Encounters.AsNoTracking();
        if (status.HasValue)
            query = query.Where(e => e.Status == status.Value);
        return await query
            .OrderByDescending(e => e.Period.Start)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    /// <inheritdoc/>
    public async Task AddAsync(Encounter encounter, CancellationToken ct = default)
    {
        await _db.Encounters.AddAsync(encounter, ct);
    }

    /// <inheritdoc/>
    public void Update(Encounter encounter)
    {
        _db.Encounters.Update(encounter);
    }

    /// <inheritdoc/>
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
