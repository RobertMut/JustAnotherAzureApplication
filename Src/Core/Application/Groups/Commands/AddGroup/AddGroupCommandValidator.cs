using System.Data;
using FluentValidation;

namespace Application.Groups.Commands.AddGroup;

public class AddGroupCommandValidator : AbstractValidator<AddGroupCommand>
{
    public AddGroupCommandValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Name must be not null or empty");
        RuleFor(x => x.Description).NotNull().NotEmpty().WithMessage("Description must be not null or empty");
        RuleFor(x => x.Name).Must(x => x.Length < 512).WithMessage("Group name is too long.");
        RuleFor(x => x.Description).Must(x => x.Length < 4000).WithMessage("Group description is too long.");
    }
}