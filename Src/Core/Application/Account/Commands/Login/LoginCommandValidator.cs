using Domain.Constants.Image;
using FluentValidation;

namespace Application.Account.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.LoginModel.UserName).NotNull().NotEmpty();
        RuleFor(x => x.LoginModel.Password).NotNull().NotEmpty();
        RuleFor(x => x.LoginModel).NotNull();
    }
}