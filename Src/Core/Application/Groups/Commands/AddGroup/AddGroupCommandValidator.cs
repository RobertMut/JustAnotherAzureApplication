using FluentValidation;

namespace Application.Groups.Commands.AddGroup
{
    public class AddGroupCommandValidator : AbstractValidator<AddGroupCommand>
    {
        public AddGroupCommandValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();
            RuleFor(x => x.Description).NotNull().NotEmpty();
        }
    }
}
