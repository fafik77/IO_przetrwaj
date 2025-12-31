using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Przetrwaj.Presentation;

public class GlobalExceptionHandler : IExceptionHandler
{
	private readonly ILogger<GlobalExceptionHandler> _logger;

	public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
	{
		_logger = logger;
	}

	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken cancellationToken)
	{
		// Check if the exception is a cancellation
		if (exception is OperationCanceledException || exception is TaskCanceledException)
		{
			// 499 is a common convention for 'Client Closed Request'
			httpContext.Response.StatusCode = 499;

			return true; // Return true to signal that this exception is handled
		}
		// For other exceptions, you can still use your ExceptionCasting logic here
		return false; // Return false to let other handlers or default middleware deal with it
	}
}
