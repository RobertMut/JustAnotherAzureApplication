using FluentValidation;

namespace Application.Account.Commands.Register
{
    /// <summary>
    /// Class RegisterCommandValidator
    /// </summary>
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        /// <summary>
        /// Initializes new instance of <see cref="RegisterCommandValidator" /> class.
        /// </summary>
        public RegisterCommandValidator()
        {
            RuleFor(x => x.RegisterModel.Username).NotNull().NotEmpty();
            RuleFor(x => x.RegisterModel.Password).NotNull().NotEmpty();
            RuleFor(x => x.RegisterModel).NotNull();
        }
    }
}
