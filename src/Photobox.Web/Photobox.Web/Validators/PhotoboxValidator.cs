using FluentValidation;
using Photobox.Web.Models;

namespace Photobox.Web.Validators;

public class PhotoboxValidator : AbstractValidator<PhotoBox>
{
    public PhotoboxValidator()
    {
        RuleFor(p => p.Id).NotEmpty().WithMessage("PhotoBox primary key is required.");

        RuleFor(p => p.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(256)
            .WithMessage("Name cannot exceed 256 characters.");

        RuleFor(p => p.HardwareId)
            .NotEmpty()
            .WithMessage("HardwareId is required.")
            .MaximumLength(52)
            .WithMessage("HardwareId cannot exceed 52 characters.");

        RuleFor(p => p.ApplicationUserId)
            .NotEmpty()
            .WithMessage("ApplicationUserId is required.")
            .MaximumLength(50)
            .WithMessage("ApplicationUserId cannot exceed 50 characters.");

        RuleFor(p => p.ApplicationUser)
            .NotNull()
            .WithMessage("ApplicationUser navigation property must be set.");
    }
}
