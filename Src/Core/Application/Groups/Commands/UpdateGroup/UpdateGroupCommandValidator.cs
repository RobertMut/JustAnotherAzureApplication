using FluentValidation;

namespace Application.Groups.Commands.UpdateGroup;

public class UpdateGroupCommandValidator : AbstractValidator<UpdateGroupCommand>
{
    public UpdateGroupCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().NotNull();
        RuleFor(x => x.Description).NotEmpty().NotNull();
    }
}