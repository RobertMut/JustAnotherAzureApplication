using FluentValidation;

namespace Application.Groups.Commands.DeleteGroup;

public class DeleteGroupCommandValidator : AbstractValidator<DeleteGroupCommand>
{
    public DeleteGroupCommandValidator()
    {
        RuleFor(x => x.GroupId).NotEmpty();
    }
}