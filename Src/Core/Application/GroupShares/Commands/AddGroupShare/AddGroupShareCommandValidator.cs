using FluentValidation;

namespace Application.GroupShares.Commands.AddGroupShare
{
    public class AddGroupShareCommandValidator : AbstractValidator<AddGroupShareCommand>
    {
        public AddGroupShareCommandValidator()
        {
            RuleFor(x => x.GroupId).NotEmpty().NotNull();
            RuleFor(x => x.Filename).NotEmpty().NotNull();
            RuleFor(x => x.PermissionId).NotNull();
        }
    }
}
