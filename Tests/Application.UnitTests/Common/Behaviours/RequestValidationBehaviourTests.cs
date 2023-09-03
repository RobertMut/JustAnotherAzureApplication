using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Behaviours;
using Application.Groups.Commands.AddGroup;
using Application.GroupShares.Queries.GetSharesByGroup;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using NUnit.Framework;

namespace Application.UnitTests.Common.Behaviours;

[ExcludeFromCodeCoverage]
[TestFixture]
public class RequestValidationBehaviourTests
{
    [Test]
    public async Task RequestSemaphoreBehaviourExecutesWithValidators()
    {
        var inlineValidatorMock = new Mock<AbstractValidator<AddGroupCommand>>();
        inlineValidatorMock.Setup(x => x.Validate(It.IsAny<ValidationContext<AddGroupCommand>>()))
            .Returns(new ValidationResult());
        var validators = new List<IValidator<AddGroupCommand>>
        {
            inlineValidatorMock.Object
        };

        var behaviour = new RequestValidationBehaviour<AddGroupCommand, string>(validators);
        
        Assert.DoesNotThrowAsync(async () =>
        {
            await behaviour.Handle(new AddGroupCommand()
            {
                
            }, new CancellationToken(), Mock.Of<RequestHandlerDelegate<string>>());
        });
    }

    [Test]
    public async Task RequestSemaphoreBehaviourExecutesWithoutValidators()
    {
        var behaviour = new RequestValidationBehaviour<AddGroupCommand, string>(new List<IValidator<AddGroupCommand>>());
        
        Assert.DoesNotThrowAsync(async () =>
        {
            await behaviour.Handle(new AddGroupCommand()
            {
                
            }, new CancellationToken(), Mock.Of<RequestHandlerDelegate<string>>());
        });
    }
    
    [Test]
    public async Task RequestSemaphoreBehaviourExecutesWithErrors()
    {
        var inlineValidatorMock = new Mock<AbstractValidator<AddGroupCommand>>();
        inlineValidatorMock.Setup(x => x.Validate(It.IsAny<ValidationContext<AddGroupCommand>>()))
            .Returns(new ValidationResult(new List<ValidationFailure>()
            {
                new("Test", "Error")
            }));
        var validators = new List<IValidator<AddGroupCommand>>
        {
            inlineValidatorMock.Object
        };

        var behaviour = new RequestValidationBehaviour<AddGroupCommand, string>(validators);
        
        Assert.ThrowsAsync<ValidationException>(async () => await behaviour.Handle(new AddGroupCommand()
        {
                
        }, new CancellationToken(), Mock.Of<RequestHandlerDelegate<string>>()), "Test: Error Severity: Error");
    }
}