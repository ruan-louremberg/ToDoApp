using FluentResults;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace ToDoApp.Api.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult FromErrors(IEnumerable<IError> errors)
    {
        var errorList = errors.ToList();
        var statusCode = GetStatusCode(errorList);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = GetTitle(statusCode),
            Detail = string.Join(" ", errorList.Select(error => error.Message)),
            Instance = HttpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] = HttpContext.TraceIdentifier;

        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode,
            ContentTypes = { "application/problem+json" }
        };
    }

    protected IActionResult FromValidationErrors(IEnumerable<ValidationFailure> errors)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Erro de validação",
            Detail = string.Join(" ", errors.Select(error => error.ErrorMessage)),
            Instance = HttpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] = HttpContext.TraceIdentifier;

        return new ObjectResult(problemDetails)
        {
            StatusCode = StatusCodes.Status400BadRequest,
            ContentTypes = { "application/problem+json" }
        };
    }

    private static int GetStatusCode(IReadOnlyCollection<IError> errors)
    {
        var statusValue = errors
            .SelectMany(error => error.Metadata)
            .FirstOrDefault(metadata => metadata.Key == "statusCode")
            .Value;

        return statusValue is null
            ? StatusCodes.Status400BadRequest
            : Convert.ToInt32(statusValue);
    }

    private static string GetTitle(int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status400BadRequest => "Erro de validação",
            StatusCodes.Status404NotFound => "Recurso não encontrado",
            StatusCodes.Status409Conflict => "Conflito",
            _ => "Erro na requisição"
        };
    }
}
