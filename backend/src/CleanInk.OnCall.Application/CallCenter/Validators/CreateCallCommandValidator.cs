using CleanInk.OnCall.Application.CallCenter.Commands;
using FluentValidation;

namespace CleanInk.OnCall.Application.CallCenter.Validators;

/// <summary>
/// Validates <see cref="CreateCallCommand"/> before it reaches the handler.
/// </summary>
public sealed class CreateCallCommandValidator : AbstractValidator<CreateCallCommand>
{
    /// <summary>Initializes a new instance with all validation rules.</summary>
    public CreateCallCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.");

        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("Subject is required.")
            .MaximumLength(200).WithMessage("Subject cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(5000).WithMessage("Description cannot exceed 5000 characters.");
    }
}
