using FluentValidation;

namespace Application.UserShares.Commands.DeleteUserShare;

public class DeleteUserShareCommandValidator : AbstractValidator<DeleteUserShareCommand>
{
    public DeleteUserShareCommandValidator()
    {
        RuleFor(x => x.UserId).NotNull();
        RuleFor(x => x.Filename).NotEmpty();
    }
}