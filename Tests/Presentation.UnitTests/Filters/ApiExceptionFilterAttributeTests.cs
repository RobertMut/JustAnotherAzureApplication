using System.Diagnostics.CodeAnalysis;
using System.Net;
using API.Filters;
using Application.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace Presentation.UnitTests.Filters;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ApiExceptionFilterAttributeTests
{
    [TestCase(typeof(NullReferenceException), HttpStatusCode.InternalServerError, "nullRef")]
    [TestCase(typeof(ArgumentException), HttpStatusCode.InternalServerError, "ArgumentEx", "stack")]
    [TestCase(typeof(OperationFailedException), HttpStatusCode.InternalServerError, HttpStatusCode.Continue,
        HttpStatusCode.Ambiguous, "where", "inner")]
    [TestCase(typeof(ValidationException), HttpStatusCode.BadRequest, "ValEx")]
    [TestCase(typeof(UnauthorizedException), HttpStatusCode.Unauthorized, "UnauEx")]
    [TestCase(typeof(DuplicatedException), HttpStatusCode.Conflict, "Duplicated")]
    [TestCase(typeof(UserNotFoundException), HttpStatusCode.NotFound, "NotFound")]
    [TestCase(typeof(GroupNotFoundException), HttpStatusCode.NotFound, "GroupNotFound")]
    [TestCase(typeof(ShareNotFoundException), HttpStatusCode.NotFound, "ShareNotFound", "id")]
    [TestCase(typeof(FileNotFoundException), HttpStatusCode.NotFound, "FileNotFound")]
    public async Task ExceptionFilterAttributeReturnsExpectedErrorMessages(Type exceptionType,
        HttpStatusCode expectedStatusCode, params object[] args)
    {
        ApiExceptionFilterAttribute exceptionFilterAttribute = new ApiExceptionFilterAttribute();
        var context = new ActionContext(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor());

        Exception ex = Activator.CreateInstance(exceptionType, args) as Exception;

        var executedContext = new ResultExecutingContext(context, Array.Empty<IFilterMetadata>(),
            new ObjectResult(string.Empty), Mock.Of<Microsoft.AspNetCore.Mvc.Controller>());

        var exceptionContext = new ExceptionContext(executedContext, Array.Empty<IFilterMetadata>())
        {
            Exception = ex,
        };

        Assert.IsNull(exceptionContext.Result);
        exceptionFilterAttribute.OnException(exceptionContext);

        Assert.True(exceptionContext.Exception.GetType() == exceptionType);
        Assert.True((exceptionContext.Result as IStatusCodeActionResult).StatusCode.ToString() ==
                    Convert.ToInt32(expectedStatusCode).ToString());
        Assert.True(exceptionContext.Exception.Message.Contains(args[0].ToString()));
    }
}