using Domain.Constants.Image;
using FluentValidation;

namespace Application.Images.Commands.AddImage;

public class AddImageCommandValidator : AbstractValidator<AddImageCommand>
{
    public AddImageCommandValidator()
    {
        RuleFor(x => x.File.Length).GreaterThan(0).WithMessage("Invalid file");
        RuleFor(x => x.Width).GreaterThan(0).WithMessage("Width must be greater than 0");
        RuleFor(x => x.Height).GreaterThan(0).WithMessage("Height must be greater than 0");
        RuleFor(x => x.ContentType).Must(x => x.StartsWith(Prefixes.ImageFormat)).WithMessage("Invalid content type");
    }
}