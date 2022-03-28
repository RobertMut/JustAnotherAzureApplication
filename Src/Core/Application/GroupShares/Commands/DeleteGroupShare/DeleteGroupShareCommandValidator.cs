using FluentValidation;

namespace Application.GroupShares.Commands.DeleteGroupShare
{
    public class DeleteGroupShareCommandValidator : AbstractValidator<DeleteGroupShareCommand>
    {
        public DeleteGroupShareCommandValidator()
        {
            RuleFor(x => x.GroupId).NotEmpty();
        }
    }
}
