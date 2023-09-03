using Domain.Constants.Image;
using FluentValidation;

namespace Application.Account.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.LoginModel.UserName).NotNull().NotEmpty().WithMessage("Username must be not null or empty");
        RuleFor(x => x.LoginModel.Password).NotNull().NotEmpty().WithMessage("Password must be not null or empty");
    }
}