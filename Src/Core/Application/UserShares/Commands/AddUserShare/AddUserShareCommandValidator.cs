﻿using FluentValidation;

namespace Application.UserShares.Commands.AddUserShare;

public class AddUserShareCommandValidator : AbstractValidator<AddUserShareCommand>
{
    public AddUserShareCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().NotNull().WithMessage("UserId must be not empty or null");
        RuleFor(x => x.Filename).NotEmpty().NotNull().WithMessage("Filename must be not empty or null");
        RuleFor(x => x.PermissionId).NotNull().WithMessage("PermissionId must be not null");
    }
}