using Domain.Constants.Image;
using FluentValidation;

namespace Application.Images.Commands.AddImage
{
    /// <summary>
    /// Class AddImageComamndValidator
    /// </summary>
    public class AddImageCommandValidator : AbstractValidator<AddImageCommand>
    {
        /// <summary>
        /// Initializes new instance of <see cref="AddImageCommandValidator" /> class.
        /// </summary>
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
