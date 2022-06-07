using FluentValidation;

namespace Application.Images.Commands.UpdateImage
{
    /// <summary>
    /// Class UpdateImageCommandValidator
    /// </summary>
    public class UpdateImageCommandValidator : AbstractValidator<UpdateImageCommand>
    {
        /// <summary>
        /// Initializes new instance of <see cref="UpdateImageCommandValidator" /> class.
        /// </summary>
        public UpdateImageCommandValidator()
        {
            RuleFor(x => x.Width).NotNull().GreaterThan(0);
            RuleFor(x => x.Height).NotNull().GreaterThan(0);
            RuleFor(x => x.TargetType).NotEmpty();
        }
    }
}
