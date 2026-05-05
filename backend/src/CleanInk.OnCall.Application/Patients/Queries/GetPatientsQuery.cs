using CleanInk.OnCall.Application.Patients.DTOs;
using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanInk.OnCall.Application.Patients.Queries;

/// <summary>
/// Queries the patient list with name-fragment search and pagination.
/// Only non-archived patients are returned.
/// </summary>
/// <param name="NameFragment">Partial last or first name (case-insensitive). Empty string returns all.</param>
/// <param name="Page">1-based page number.</param>
/// <param name="PageSize">Items per page (max 50).</param>
public sealed record GetPatientsQuery(string NameFragment, int Page = 1, int PageSize = 20)
    : IRequest<Result<PagedResult<PatientDto>>>;

/// <summary>Handler for <see cref="GetPatientsQuery"/>.</summary>
public sealed class GetPatientsQueryHandler
    : IRequestHandler<GetPatientsQuery, Result<PagedResult<PatientDto>>>
{
    private readonly IPatientRepository _patients;
    private readonly ILogger<GetPatientsQueryHandler> _logger;

    /// <summary>Initializes a new instance of <see cref="GetPatientsQueryHandler"/>.</summary>
    public GetPatientsQueryHandler(
        IPatientRepository patients,
        ILogger<GetPatientsQueryHandler> logger)
    {
        _patients = patients;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<PagedResult<PatientDto>>> Handle(
        GetPatientsQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _patients.SearchAsync(
            request.NameFragment,
            request.Page,
            request.PageSize,
            cancellationToken);

        var dtos = items.Select(PatientDto.FromEntity).ToList();

        _logger.LogDebug("Patient search for '{Fragment}' — {Count}/{Total} results.",
            request.NameFragment, dtos.Count, total);

        return Result<PagedResult<PatientDto>>.Success(
            new PagedResult<PatientDto>(dtos, request.Page, request.PageSize, total));
    }
}
