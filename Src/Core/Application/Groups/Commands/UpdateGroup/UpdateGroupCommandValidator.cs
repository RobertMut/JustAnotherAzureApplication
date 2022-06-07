using FluentValidation;

namespace Application.Groups.Commands.UpdateGroup
{
    /// <summary>
    /// Class UpdateGroupCommandValidator
    /// </summary>
    public class UpdateGroupCommandValidator : AbstractValidator<UpdateGroupCommand>
    {
        /// <summary>
        /// Initializes new instance of <see cref="UpdateGroupCommandValidator" /> class.
        /// </summary>
        public UpdateGroupCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().NotNull();
            RuleFor(x => x.Description).NotEmpty().NotNull();
        }
    }
}
