using CleanInk.OnCall.Application.Patients.DTOs;
using CleanInk.OnCall.Domain.Entities;
using CleanInk.OnCall.Domain.Repositories;
using CleanInk.OnCall.Domain.ValueObjects;
using CleanInk.OnCall.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanInk.OnCall.Application.Patients.Commands;

/// <summary>
/// Registers a new patient in the system.
/// All PII fields are optional at registration; they can be added later via consent update.
///
/// RGPD note: the call center may register a patient with minimal data (LastName + FirstName + DOB)
/// and collect NIR + contact details only after explicit consent is obtained.
/// </summary>
/// <param name="LastName">Last name (required).</param>
/// <param name="FirstName">First name (required).</param>
/// <param name="DateOfBirth">Date of birth in ISO 8601 format (required).</param>
/// <param name="Nir">Optional NIR (15-digit French SS number).</param>
/// <param name="Phone">Optional phone in E.164 or French format.</param>
/// <param name="Email">Optional email address.</param>
public sealed record RegisterPatientCommand(
    string LastName,
    string FirstName,
    DateOnly DateOfBirth,
    string? Nir,
    string? Phone,
    string? Email) : IRequest<Result<PatientDto>>;

/// <summary>Handler for <see cref="RegisterPatientCommand"/>.</summary>
public sealed class RegisterPatientCommandHandler : IRequestHandler<RegisterPatientCommand, Result<PatientDto>>
{
    private readonly IPatientRepository _patients;
    private readonly ILogger<RegisterPatientCommandHandler> _logger;

    /// <summary>Initializes a new instance of <see cref="RegisterPatientCommandHandler"/>.</summary>
    public RegisterPatientCommandHandler(
        IPatientRepository patients,
        ILogger<RegisterPatientCommandHandler> logger)
    {
        _patients = patients;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<PatientDto>> Handle(
        RegisterPatientCommand request, CancellationToken cancellationToken)
    {
        // Resolve optional value objects from raw strings.
        NirNumber? nir = null;
        PhoneNumber? phone = null;
        EmailAddress? email = null;

        if (!string.IsNullOrWhiteSpace(request.Nir))
        {
            var nirResult = NirNumber.Create(request.Nir);
            if (!nirResult.IsSuccess) return Result<PatientDto>.Failure(nirResult.Error);
            nir = nirResult.Value;
        }

        if (!string.IsNullOrWhiteSpace(request.Phone))
        {
            var phoneResult = PhoneNumber.Create(request.Phone);
            if (!phoneResult.IsSuccess) return Result<PatientDto>.Failure(phoneResult.Error);
            phone = phoneResult.Value;
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var emailResult = EmailAddress.Create(request.Email);
            if (!emailResult.IsSuccess) return Result<PatientDto>.Failure(emailResult.Error);
            email = emailResult.Value;
        }

        var patientResult = Patient.Register(
            request.LastName,
            request.FirstName,
            request.DateOfBirth,
            nir,
            phone,
            email);

        if (!patientResult.IsSuccess) return Result<PatientDto>.Failure(patientResult.Error);

        var patient = patientResult.Value;
        await _patients.AddAsync(patient, cancellationToken);
        await _patients.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Patient {PatientId} registered: {LastName} {FirstName}",
            patient.Id, patient.LastName, patient.FirstName);

        return Result<PatientDto>.Success(PatientDto.FromEntity(patient));
    }
}
