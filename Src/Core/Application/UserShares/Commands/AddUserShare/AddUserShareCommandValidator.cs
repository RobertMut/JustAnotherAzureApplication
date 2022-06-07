using FluentValidation;

namespace Application.UserShares.Commands.AddUserShare
{
    /// <summary>
    /// Class AddUserShareCommandValidator
    /// </summary>
    public class AddUserShareCommandValidator : AbstractValidator<AddUserShareCommand>
    {
        /// <summary>
        /// Initializes new instance of <see cref="AddUserShareCommandValidator" /> class.
        /// </summary>
        public AddUserShareCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().NotNull();
            RuleFor(x => x.Filename).NotEmpty().NotNull();
            RuleFor(x => x.PermissionId).NotNull();
        }
    }
}
