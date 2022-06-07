using FluentValidation;

namespace Application.UserShares.Commands.DeleteUserShare
{
    /// <summary>
    /// Class DeleteUserShareCommandValidator
    /// </summary>
    public class DeleteUserShareCommandValidator : AbstractValidator<DeleteUserShareCommand>
    {
        /// <summary>
        /// Initializes new instance of <see cref="DeleteUserShareCommandValidator" /> class.
        /// </summary>
        public DeleteUserShareCommandValidator()
        {
            RuleFor(x => x.UserId).NotNull();
            RuleFor(x => x.Filename).NotEmpty();
        }
    }
}
