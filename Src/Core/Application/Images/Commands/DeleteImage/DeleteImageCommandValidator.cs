using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.Images.Commands.DeleteImage;

public class DeleteImageCommandValidator : AbstractValidator<DeleteImageCommand>
{
    public DeleteImageCommandValidator()
    {
        RuleFor(x => x.Filename).NotEmpty().WithMessage("Filename must be not empty");
        When(x => !string.IsNullOrEmpty(x.Size), () =>
        {
            RuleFor(x => x.Size).Must(m => Regex.IsMatch(m, @"([0-9]+x[0-9]+)"))
                .WithMessage("Invalid dimension format");
            RuleFor(x => x.Size).Must(s =>
                    s.Split("x").ToList().TrueForAll(d =>
                        int.TryParse(d, out int i) && i > 0))
                .WithMessage("Dimensions must be numeric and greater than 0");
        });
    }
}