using CleanInk.OnCall.Application.Patients.DTOs;
using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Shared;
using MediatR;

namespace CleanInk.OnCall.Application.Patients.Queries;

/// <summary>Retrieves a single patient by their identifier.</summary>
/// <param name="PatientId">Patient GUID.</param>
public sealed record GetPatientByIdQuery(Guid PatientId) : IRequest<Result<PatientDto>>;

/// <summary>Handler for <see cref="GetPatientByIdQuery"/>.</summary>
public sealed class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, Result<PatientDto>>
{
    private readonly IPatientRepository _patients;

    /// <summary>Initializes a new instance of <see cref="GetPatientByIdQueryHandler"/>.</summary>
    public GetPatientByIdQueryHandler(IPatientRepository patients) => _patients = patients;

    /// <inheritdoc/>
    public async Task<Result<PatientDto>> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        var patient = await _patients.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
            return Result<PatientDto>.Failure(Error.NotFound("Patient.NotFound", $"Patient {request.PatientId} not found."));

        return Result<PatientDto>.Success(PatientDto.FromEntity(patient));
    }
}
