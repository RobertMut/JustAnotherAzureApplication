using Domain.Constants.Image;
using FluentValidation;

namespace Application.Account.Commands.Login
{
    /// <summary>
    /// Class LoginCommandValidator
    /// </summary>
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        /// <summary>
        /// Initializes new instance of <see cref="LoginCommandValidator" /> class.
        /// </summary>
        public LoginCommandValidator()
        {
            RuleFor(x => x.LoginModel.UserName).NotNull().NotEmpty();
            RuleFor(x => x.LoginModel.Password).NotNull().NotEmpty();
            RuleFor(x => x.LoginModel).NotNull();
        }
    }
}
