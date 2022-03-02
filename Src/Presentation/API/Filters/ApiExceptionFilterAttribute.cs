using Microsoft.AspNetCore.Mvc;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;

namespace API.Filters
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;
        public ApiExceptionFilterAttribute()
        {
            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                {typeof(NullReferenceException), HandleException},
                {typeof(ArgumentException), HandleException}
            };
        }

        private void HandleException(ExceptionContext obj)
        {

            obj.Response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
            {
                ReasonPhrase = "An error occurred while processing your request.\n https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };
        }

        private void HandleNullException(ExceptionContext obj)
        {
            throw new NotImplementedException();
        }
    }
}
