using CleanInk.OnCall.Application.Billing.Commands;
using FluentValidation;

namespace CleanInk.OnCall.Application.Billing.Validators;

/// <summary>
/// Validates <see cref="CreateInvoiceCommand"/> before it reaches the handler.
/// </summary>
public sealed class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    /// <summary>Initializes a new instance with all validation rules.</summary>
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.");

        RuleFor(x => x.Reference)
            .NotEmpty().WithMessage("Reference is required.")
            .MaximumLength(100).WithMessage("Reference cannot exceed 100 characters.");

        RuleFor(x => x.AmountCents)
            .GreaterThan(0).WithMessage("Amount must be positive.");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("DueDate must be in the future.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .Length(3).WithMessage("Currency must be a 3-letter ISO 4217 code.");
    }
}
