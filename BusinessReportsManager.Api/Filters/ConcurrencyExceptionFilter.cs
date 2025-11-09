using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace BusinessReportsManager.Api.Filters;

public class ConcurrencyExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ConcurrencyExceptionFilter> _logger;

    public ConcurrencyExceptionFilter(ILogger<ConcurrencyExceptionFilter> logger) => _logger = logger;

    public void OnException(ExceptionContext context)
    {
        if (context.Exception is DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency conflict");
            var problem = new ProblemDetails
            {
                Title = "Concurrency conflict",
                Status = StatusCodes.Status409Conflict,
                Detail = "The record you attempted to update was modified by another user. Reload and retry."
            };
            context.Result = new ObjectResult(problem) { StatusCode = StatusCodes.Status409Conflict };
            context.ExceptionHandled = true;
        }
    }
}