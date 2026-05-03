using CleanInk.OnCall.Domain.Entities;
using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CleanInk.OnCall.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IPatientRepository"/>.
/// Note: NIR and Phone are stored encrypted — the EncryptionService
/// handles en/decryption transparently via EF value converters (future sprint).
/// For now, values are stored as-is with a TODO marker.
/// </summary>
public sealed class PatientRepository : IPatientRepository
{
    private readonly AppDbContext _context;

    /// <summary>Initializes a new instance of <see cref="PatientRepository"/>.</summary>
    public PatientRepository(AppDbContext context) => _context = context;

    /// <inheritdoc/>
    public async Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    /// <inheritdoc/>
    public async Task<(IReadOnlyList<Patient> Items, int TotalCount)> SearchAsync(
        string nameFragment,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageSize = Math.Min(pageSize, 50);
        var normalizedFragment = nameFragment.Trim().ToUpperInvariant();

        var query = _context.Patients
            .AsNoTracking()
            .Where(p => !p.IsArchived &&
                        (p.LastName.Contains(normalizedFragment) ||
                         p.FirstName.ToUpper().Contains(normalizedFragment)));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <inheritdoc/>
    public async Task AddAsync(Patient patient, CancellationToken cancellationToken = default) =>
        await _context.Patients.AddAsync(patient, cancellationToken);

    /// <inheritdoc/>
    public void Update(Patient patient) => _context.Patients.Update(patient);

    /// <inheritdoc/>
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
