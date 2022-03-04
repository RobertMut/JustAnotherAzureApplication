﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Images.Commands
{
    public class AddImageCommandValidator : AbstractValidator<AddImageCommand>
    {
        public AddImageCommandValidator()
        {
            RuleFor(x => x.File).NotNull();
            RuleFor(x => x.File.Length).GreaterThan(0);
            RuleFor(x => x.Width.Value).GreaterThan(0);
            RuleFor(x => x.Height.Value).GreaterThan(0);
            RuleFor(x => x.ContentType).Must(x => x.StartsWith("image/"));
        }
    }
}