using FluentValidation;

namespace Application.GroupShares.Commands.AddGroupShare
{
    /// <summary>
    /// Class AddGroupShareCommandValidator
    /// </summary>
    public class AddGroupShareCommandValidator : AbstractValidator<AddGroupShareCommand>
    {
        /// <summary>
        /// Initializes new instance of <see cref="AddGroupShareCommandValidator" /> class.
        /// </summary>
        public AddGroupShareCommandValidator()
        {
            RuleFor(x => x.GroupId).NotEmpty().NotNull();
            RuleFor(x => x.Filename).NotEmpty().NotNull();
            RuleFor(x => x.PermissionId).NotNull();
        }
    }
}
