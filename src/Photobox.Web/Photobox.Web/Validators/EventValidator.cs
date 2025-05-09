using FluentValidation;
using Photobox.Web.Models;

namespace Photobox.Web.Validators;

public class EventValidator : AbstractValidator<Event>
{
    public EventValidator()
    {
        RuleFor(p => p.Id).NotEmpty().WithMessage("Event primary key is required.");

        RuleFor(e => e.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(256)
            .WithMessage("Name cannot exceed 256 characters.");

        RuleFor(e => e.StartDate)
            .LessThanOrEqualTo(e => e.EndDate)
            .WithMessage("StartDate must be before or equal to EndDate.");

        RuleFor(e => e.ApplicationUserId)
            .MaximumLength(50)
            .WithMessage("ApplicationUserId cannot exceed 50 characters.")
            .When(e => !string.IsNullOrWhiteSpace(e.ApplicationUserId));
    }
}
