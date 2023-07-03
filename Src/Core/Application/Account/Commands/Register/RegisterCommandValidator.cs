using FluentValidation;

namespace Application.Account.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.RegisterModel.Username).NotNull().NotEmpty();
        RuleFor(x => x.RegisterModel.Password).NotNull().NotEmpty();
        RuleFor(x => x.RegisterModel).NotNull();
    }
}