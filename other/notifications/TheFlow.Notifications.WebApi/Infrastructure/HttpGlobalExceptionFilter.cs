using System.Linq;
using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using TheFlow.Notifications.Application.Infrastructure.Mediatr.Exceptions;

namespace TheFlow.Notifications.WebApi.Infrastructure
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;

        public HttpGlobalExceptionFilter(
            IHostingEnvironment env,
            ILogger<HttpGlobalExceptionFilter> logger
            )
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                context.Exception.Message);

            if (context.Exception.GetType() == typeof(MediatrPipelineException))
            {
                if (context.Exception.InnerException is ValidationException validationException)
                {
                    var json = new JsonErrorResponse
                    {
                        Messages = validationException.Errors
                            .Select(e => e.ErrorMessage)
                            .ToArray()
                    };

                    context.Result = new BadRequestObjectResult(json);
                }
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else
            {
                var json = new JsonErrorResponse
                {
                    Messages = new[]
                    {
                        "Internal Error. Try again later.",
                        context.Exception.GetType().ToString(),
                        context.Exception.Message
                    }
                };

                context.Result = new ObjectResult(json) { StatusCode = 500 };
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            context.ExceptionHandled = true;
        }

        public class JsonErrorResponse
        {
            public string[] Messages { get; set; }
        }
    }
}
