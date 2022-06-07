using FluentValidation;

namespace Application.GroupShares.Commands.DeleteGroupShare
{
    /// <summary>
    /// Class DeleteGroupShareCommandValidator
    /// </summary>
    public class DeleteGroupShareCommandValidator : AbstractValidator<DeleteGroupShareCommand>
    {
        /// <summary>
        /// Initializes new instance of <see cref="DeleteGroupShareCommandValidator" /> class.
        /// </summary>
        public DeleteGroupShareCommandValidator()
        {
            RuleFor(x => x.GroupId).NotEmpty();
        }
    }
}
