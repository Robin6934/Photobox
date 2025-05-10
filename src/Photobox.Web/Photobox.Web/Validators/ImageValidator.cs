using FluentValidation;
using Image = Photobox.Web.Models.Image;

namespace Photobox.Web.Validators;

public class ImageValidator : AbstractValidator<Image>
{
    public ImageValidator()
    {
        RuleFor(i => i.Id).NotEmpty().WithMessage("Image ID is required.");

        RuleFor(i => i.ImageName)
            .NotEmpty()
            .WithMessage("Image name is required.")
            .MaximumLength(64)
            .WithMessage("Image name must not exceed 64 characters.");

        RuleFor(i => i.UniqueImageName)
            .NotEmpty()
            .WithMessage("Unique image name is required.")
            .MaximumLength(45)
            .WithMessage("Unique image name must not exceed 45 characters.");

        RuleFor(i => i.DownscaledImageName)
            .NotEmpty()
            .WithMessage("Downscaled image name is required.")
            .MaximumLength(45)
            .WithMessage("Downscaled image name must not exceed 45 characters.");

        RuleFor(i => i.TakenAt)
            .NotEmpty()
            .WithMessage("TakenAt must be set.")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("TakenAt cannot be in the future.");
    }
}
