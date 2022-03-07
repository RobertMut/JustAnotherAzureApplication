using Application.Common.Exceptions;
using FluentValidation.Results;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Application.UnitTests.Common.Exception
{
    public class ValidationExceptionTests
    {
        [Test]
        public void DefaultConstructorCreatesEmptyDictionary()
        {
            var actual = new ValidationException().Errors;
            Assert.True(actual.Count == 0);
        }
        [Test]
        public void ValidationFailureAddElementToDictionary()
        {
            var failures = new List<ValidationFailure>
            {
                new ValidationFailure("Image", "wrong extension")
            };
            var actual = new ValidationException(failures).Errors;
            Assert.True(actual.Keys.First() == "Image");
            Assert.True(actual["Image"].First() == "wrong extension");
        }
    }
}
