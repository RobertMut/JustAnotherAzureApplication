using FluentValidation;

namespace Application.UserShares.Commands.AddUserShare
{
    public class AddUserShareCommandValidator : AbstractValidator<AddUserShareCommand>
    {
        public AddUserShareCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().NotNull();
            RuleFor(x => x.Filename).NotEmpty().NotNull();
            RuleFor(x => x.PermissionId).NotNull();
        }
    }
}
