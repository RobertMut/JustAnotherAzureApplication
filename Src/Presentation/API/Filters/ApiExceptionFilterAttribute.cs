using Application.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters;

public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;
    private const string Rfc7231 = "https://tools.ietf.org/html/rfc7231";
    
    public ApiExceptionFilterAttribute()
    {
        _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
        {
            { typeof(NullReferenceException), HandleUnknownException },
            { typeof(ArgumentException), HandleUnknownException },
            { typeof(OperationFailedException), HandleUnknownException },
            { typeof(ValidationException), HandleValidationException },
            { typeof(UnauthorizedException), HandleUnauthorizedException },
            { typeof(DuplicatedException), HandleDuplicatedException },
            { typeof(UserNotFoundException), HandleNotFoundException },
            { typeof(GroupNotFoundException), HandleNotFoundException },
            { typeof(ShareNotFoundException), HandleNotFoundException},
            { typeof(FileNotFoundException), HandleNotFoundException}
        };
    }

    /// <summary>
    /// NotFound exception
    /// </summary>
    /// <param name="obj"><see cref="ExceptionContext"/></param>
    private void HandleNotFoundException(ExceptionContext obj)
    {
        var details = new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Not Found",
            Type = $"{Rfc7231}#section-6.5.4",
            Detail = obj.Exception.Message
        };
        
        obj.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status404NotFound,
            Value = obj.Exception.Message
        };

        obj.ExceptionHandled = true;
    }
    
    
    /// <summary>
    /// Handle exception when exception occurs
    /// </summary>
    /// <param name="context"><see cref="ExceptionContext"/></param>
    public override void OnException(ExceptionContext context)
    {
        HandleException(context);
        base.OnException(context);
    }
    
    /// <summary>
    /// Exception handler
    /// </summary>
    /// <param name="context"><see cref="ExceptionContext"/></param>
    private void HandleException(ExceptionContext context)
    {
        Type type = context.Exception.GetType();
        
        if (_exceptionHandlers.ContainsKey(type))
        {
            _exceptionHandlers[type].Invoke(context);
            return;
        }
        
        if (!context.ModelState.IsValid)
        {
            HandleInvalidModelStateException(context);

            return;
        }
        
        HandleUnknownException(context);
    }

    /// <summary>
    /// Conflict exception
    /// </summary>
    /// <param name="obj"><see cref="ExceptionContext"/></param>
    private void HandleDuplicatedException(ExceptionContext obj)
    {
        var details = new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Conflict",
            Type = $"{Rfc7231}#section-6.5.8",
            Detail = obj.Exception.Message
        };

        obj.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status409Conflict,
            Value = obj.Exception.Message
        };

        obj.ExceptionHandled = true;
    }

    /// <summary>
    /// Unauthorized exception
    /// </summary>
    /// <param name="obj"><see cref="ExceptionContext"/></param>
    private void HandleUnauthorizedException(ExceptionContext obj)
    {
        var details = new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Unauthorized",
            Type = $"{Rfc7231}#section-3.1",
            Detail = obj.Exception.Message
        };

        obj.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status401Unauthorized
        };

        obj.ExceptionHandled = true;
    }

    /// <summary>
    /// Invalid model state exception
    /// </summary>
    /// <param name="context"><see cref="ExceptionContext"/></param>
    private void HandleInvalidModelStateException(ExceptionContext context)
    {
        var details = new ValidationProblemDetails(context.ModelState)
        {
            Type = $"{Rfc7231}#section-6.5.1",
            Title = "BadRequest",
            Detail = context.Exception.Message
        };
        
        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Value = context.Exception.Message
        };
        
        context.ExceptionHandled = true;
    }

    /// <summary>
    /// Validation exception
    /// </summary>
    /// <param name="obj"><see cref="ExceptionContext"/></param>
    private void HandleValidationException(ExceptionContext obj)
    {
        var detail = new ValidationProblemDetails(obj.ModelState)
        {
            Title = "BadRequest",
            Type = $"{Rfc7231}#section-6.5.1",
            Detail = obj.Exception.Message,
        };

        obj.Result = new BadRequestObjectResult(detail)
        {
            StatusCode = StatusCodes.Status400BadRequest
        };
        
        obj.ExceptionHandled = true;
    }

    /// <summary>
    /// Unknown exception
    /// </summary>
    /// <param name="obj"><see cref="ExceptionContext"/></param>
    private void HandleUnknownException(ExceptionContext obj)
    {
        var details = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An error occurred while processing your request.",
            Type = $"{Rfc7231}#section-6.6.1",
            Detail = obj.Exception.StackTrace,
            Instance = obj.Exception.Source
        };
        obj.Result = new ObjectResult(details)
        {
            Value = obj.Exception.Message,
            StatusCode = StatusCodes.Status500InternalServerError
        };
        obj.ExceptionHandled = true;
    }
}