using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.Exceptions;

namespace SMAIAXBackend.API.Middlewares;

public class ExceptionHandlerMiddleware : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails { Detail = exception.Message };

        switch (exception)
        {
            case MetadataNotFoundException:
            case PolicyRequestNotFoundException:
            case SmartMeterNotFoundException:
            case UserNotFoundException:
                problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.5";
                problemDetails.Status = StatusCodes.Status404NotFound;
                problemDetails.Title = "Not Found";
                break;
            case UnauthorizedAccessException:
            case InvalidTokenException:
            case InvalidLoginException:
                problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.2";
                problemDetails.Status = StatusCodes.Status401Unauthorized;
                problemDetails.Title = "Unauthorized";
                break;
            case RegistrationException:
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Registration Error";
                break;
            case ExistingPoliciesException:
            case MetadataAlreadyExistsException:
                problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8";
                problemDetails.Status = StatusCodes.Status409Conflict;
                problemDetails.Title = "Conflict";
                break;
            case InvalidTimeRangeException:
            case MetadataIdMismatchException:
            case SmartMeterIdMismatchException:
            case InsufficientLocationDataException:
            case PolicyNameMissingException:
            case SmartMeterNameRequiredException:
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Bad Request";
                break;
            default:
                problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.1";
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Internal Server Error";
                // Override details to not expose internal error details
                problemDetails.Detail = "Something went wrong.";
                break;
        }

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        await httpContext
            .Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}