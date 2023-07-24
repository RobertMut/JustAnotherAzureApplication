using System.Data;
using System.Text.RegularExpressions;
using FluentValidation;

namespace Application.Images.Queries.GetFile;

public class GetFileQueryValidator : AbstractValidator<GetFileQuery>
{
    public GetFileQueryValidator()
    {
        RuleFor(x => x.Filename).NotEmpty().NotNull().WithMessage("Filename must me not null or empty.");
        RuleFor(x => x.ExpectedMiniatureSize).NotEmpty().NotNull()
            .WithMessage("Miniature size must be not null or empty");
        RuleFor(x => x.Filename).Must(f => Regex.IsMatch(f, @"[^\\]*\.(\w+)$")).WithMessage("Invalid filename");
        RuleFor(x => x.ExpectedExtension).Must(r => Regex.IsMatch($".{r}", @"^\.[a-zA-Z0-9]+$"))
            .WithMessage("Invalid extension format");
        RuleFor(x => x.ExpectedMiniatureSize).Must(m => Regex.IsMatch(m, @"([0-9]+x[0-9]+)"))
            .WithMessage("Invalid dimension format");
        RuleFor(x => x.ExpectedMiniatureSize).Must(s =>
                s.Split("x").ToList().TrueForAll(d =>
                    int.TryParse(d, out int i) ? i > 0 : throw new ValidationException("Cannot parse int")))
            .WithMessage("Dimensions must be numeric and greater than 0");
    }
}