using Application.Images.Commands.AddImage;
using Domain.Constants.Image;
using FluentValidation;

namespace Application.Images.Commands
{
    public class AddImageCommandValidator : AbstractValidator<AddImageCommand>
    {
        public AddImageCommandValidator()
        {
            RuleFor(x => x.File).NotNull();
            RuleFor(x => x.File.Length).GreaterThan(0);
            RuleFor(x => x.Width).GreaterThan(0);
            RuleFor(x => x.Height).GreaterThan(0);
            RuleFor(x => x.ContentType).Must(x => x.StartsWith(Prefixes.ImageFormat));
        }
    }
}
