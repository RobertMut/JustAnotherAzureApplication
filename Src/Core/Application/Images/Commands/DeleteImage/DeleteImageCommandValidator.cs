using Application.Images.Commands.UpdateImage;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.Images.Commands.DeleteImage
{
    public class DeleteImageCommandValidator : AbstractValidator<DeleteImageCommand>
    {
        /// <summary>
        /// Checks if filename not empty.
        /// Checks if size is "any" or empty or matches image size pattern like 100x100
        /// </summary>
        public DeleteImageCommandValidator()
        {
            RuleFor(x => x.Filename).NotEmpty();
            RuleFor(x => x.Size).Must(x => x == "any" || string.IsNullOrEmpty(x) || Regex.IsMatch(x, @"^-?\d+x\d+"));
        }
    }
}
