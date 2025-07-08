using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace TaskManager;

public class ErrorHandlingFilterService(ILogger<ErrorHandlingFilterService> logger) : IExceptionFilter
{
    private const string errorMessage = "An error occurred while processing your request.";
    public void OnException(ExceptionContext context)
    {
        ProblemDetails problemDetails = new()
        {
            Title = errorMessage,
            Status = (int)HttpStatusCode.InternalServerError,
        };

        context.Result = new ObjectResult(problemDetails);

        context.ExceptionHandled = true;

        logger.LogError(context.Exception, errorMessage);
    }
}
