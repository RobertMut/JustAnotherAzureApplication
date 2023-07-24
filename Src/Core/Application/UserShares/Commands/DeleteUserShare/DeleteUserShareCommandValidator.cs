using FluentValidation;

namespace Application.UserShares.Commands.DeleteUserShare;

public class DeleteUserShareCommandValidator : AbstractValidator<DeleteUserShareCommand>
{
    public DeleteUserShareCommandValidator()
    {
        RuleFor(x => x.UserId).NotNull().WithMessage("UserId must be not null");
        RuleFor(x => x.Filename).NotEmpty().WithMessage("Filename must be not empty");
    }
}