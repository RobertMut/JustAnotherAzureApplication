using FluentValidation;

namespace Application.Groups.Commands.UpdateGroup;

public class UpdateGroupCommandValidator : AbstractValidator<UpdateGroupCommand>
{
    public UpdateGroupCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage("Name must be not null or empty");
        RuleFor(x => x.Description).NotEmpty().NotNull().WithMessage("Description must be not null or empty");
    }
}