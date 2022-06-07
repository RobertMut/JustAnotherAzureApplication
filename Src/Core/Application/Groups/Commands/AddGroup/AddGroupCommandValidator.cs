using FluentValidation;

namespace Application.Groups.Commands.AddGroup
{
    /// <summary>
    /// Class AddGroupCommandValidator
    /// </summary>
    public class AddGroupCommandValidator : AbstractValidator<AddGroupCommand>
    {
        /// <summary>
        /// Initializes new instance of <see cref="AddGroupCommandValidator" /> class.
        /// </summary>
        public AddGroupCommandValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();
            RuleFor(x => x.Description).NotNull().NotEmpty();
        }
    }
}
