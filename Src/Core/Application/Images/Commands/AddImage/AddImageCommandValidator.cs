using Application.Images.Commands.AddImage;
using Domain.Enums.Image;
using FluentValidation;

namespace Application.Images.Commands
{
    public class AddImageCommandValidator : AbstractValidator<AddImageCommand>
    {
        private const string _imageType = "image/";
        public AddImageCommandValidator()
        {
            RuleFor(x => x.File).NotNull();
            RuleFor(x => x.File.Length).GreaterThan(0);
            RuleFor(x => x.Width.Value).GreaterThan(0);
            RuleFor(x => x.Height.Value).GreaterThan(0);
            RuleFor(x => x.ContentType).Must(x => x.StartsWith(_imageType));
        }
    }
}
