using Microsoft.AspNetCore.Mvc;
using Tcc2.Application.Exceptions;
using Tcc2.Domain.Exceptions;

namespace Tcc2.Api.Middlewares;

public class ErrorHandlerMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (exception is ValidationModelException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        else if (exception is EntityNotFoundException)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }

        if (context.RequestServices.GetService<IProblemDetailsService>() is { } problemDetailsService)
        {
            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = Produce(exception)
            });
        }
    }

    private static ProblemDetails Produce(Exception exception)
    {
        var problemDetails = new ProblemDetails
        {
            Detail = exception.Message
        };

        if (exception is ValidationModelException validationModelException)
        {
            var invalidFields = validationModelException.ValidationResult.InvalidFields;

            problemDetails.Title = "Invalid input model";
            problemDetails.Extensions.Add("invalid-fields", invalidFields);
            return problemDetails;
        }
        else if (exception is HttpRequestException)
        {
            problemDetails.Title = "Http request exception";
            return problemDetails;
        }
        else if (exception is ValueObjectNotFoundException)
        {
            problemDetails.Title = "Value object not found";
            return problemDetails;
        }
        else if (exception is EntityNotFoundException)
        {
            problemDetails.Title = "Resource not found";
            return problemDetails;
        }

        problemDetails.Title = "Internal error";
        return problemDetails;
    }
}
