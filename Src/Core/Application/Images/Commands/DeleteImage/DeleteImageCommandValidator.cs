﻿using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.Images.Commands.DeleteImage;

public class DeleteImageCommandValidator : AbstractValidator<DeleteImageCommand>
{
    public DeleteImageCommandValidator()
    {
        RuleFor(x => x.Filename).NotEmpty();
        RuleFor(x => x.Size).Must(x => x == "any" || string.IsNullOrEmpty(x) || Regex.IsMatch(x, @"^-?\d+x\d+"));
    }
}