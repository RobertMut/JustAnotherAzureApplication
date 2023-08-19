using FluentValidation;

namespace Application.Account.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.RegisterModel.Username).NotNull().NotEmpty().WithMessage("Username must be not null or empty");
        RuleFor(x => x.RegisterModel.Password).NotNull().NotEmpty().WithMessage("Password must be not null or empty");
    }
}