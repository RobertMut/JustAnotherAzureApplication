using FluentValidation;

namespace Application.GroupShares.Commands.AddGroupShare;

public class AddGroupShareCommandValidator : AbstractValidator<AddGroupShareCommand>
{
    public AddGroupShareCommandValidator()
    {
        RuleFor(x => x.GroupId).NotEmpty().NotNull().WithMessage("GroupId msut be not empty or null");
        RuleFor(x => x.Filename).NotEmpty().NotNull().WithMessage("Filename must be not null or empty");
        RuleFor(x => x.PermissionId).NotNull().WithMessage("PermissionId must be not null");
    }
}