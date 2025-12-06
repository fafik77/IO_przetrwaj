using FluentValidation;
using MediatR;

namespace Przetrwaj.Application.ValidationPipeline;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse> // Or ICommand/IQuery interfaces
{
	private readonly IEnumerable<IValidator<TRequest>> _validators;

	public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
	{
		_validators = validators;
	}

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		if (!_validators.Any())
		{
			return await next(); // No validators found, proceed
		}

		var context = new ValidationContext<TRequest>(request);

		var validationResults = await Task.WhenAll(
			_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

		var failures = validationResults
			.Where(r => r.Errors.Any())
			.SelectMany(r => r.Errors)
			.ToList();

		if (failures.Any())
		{
			// You should implement a custom exception here (e.g., ValidationException)
			// that your API layer can catch and map to a 400 Bad Request response.
			var errorMessages = string.Join(" | ", failures.Select(f => f.ErrorMessage));
			throw new ValidationException(failures); // FluentValidation's built-in exception
		}

		return await next();
	}
}

