using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.Images.Commands.DeleteImage
{
    /// <summary>
    /// Class DeleteImageCommandValidator
    /// </summary>
    public class DeleteImageCommandValidator : AbstractValidator<DeleteImageCommand>
    {
        /// <summary>
        /// Initializes new instance of <see cref="DeleteImageCommandValidator" /> class.
        /// </summary>
        public DeleteImageCommandValidator()
        {
            RuleFor(x => x.Filename).NotEmpty();
            RuleFor(x => x.Size).Must(x => x == "any" || string.IsNullOrEmpty(x) || Regex.IsMatch(x, @"^-?\d+x\d+"));
        }
    }
}
