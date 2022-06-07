using FluentValidation;

namespace Application.Groups.Commands.DeleteGroup
{
    /// <summary>
    /// Class DeleteGroupCommandValidator
    /// </summary>
    public class DeleteGroupCommandValidator : AbstractValidator<DeleteGroupCommand>
    {
        /// <summary>
        /// Initializes new instance of <see cref="DeleteGroupCommandValidator" /> class.
        /// </summary>
        public DeleteGroupCommandValidator()
        {
            RuleFor(x => x.GroupId).NotEmpty();
        }
    }
}
