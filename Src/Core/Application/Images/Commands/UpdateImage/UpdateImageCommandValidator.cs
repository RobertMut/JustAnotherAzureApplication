using FluentValidation;

namespace Application.Images.Commands.UpdateImage;

public class UpdateImageCommandValidator : AbstractValidator<UpdateImageCommand>
{
    public UpdateImageCommandValidator()
    {
        RuleFor(x => x.Width).NotNull().GreaterThan(0).WithMessage("Width must be greater than 0");
        RuleFor(x => x.Height).NotNull().GreaterThan(0).WithMessage("Height must be greater than 0");
    }
}