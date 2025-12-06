using FluentValidation;

namespace Przetrwaj.Application.Commands.Register;

public class RegisterEmailCommandValidator : AbstractValidator<RegisterEmailCommand>
{
	public RegisterEmailCommandValidator()
	{
		// 1. Email Validation
		RuleFor(x => x.Email)
			.NotEmpty().WithMessage("Email address is required.")
			.EmailAddress().WithMessage("A valid email address is required.");

		// 2. Name and Surname Validation
		RuleFor(x => x.Name)
			.NotEmpty().WithMessage("Name is required.");

		RuleFor(x => x.Surname)
			.NotEmpty().WithMessage("Surname is required.");

		// 3. Password Strength Validation
		RuleFor(x => x.Password)
			.NotEmpty().WithMessage("Password is required.")
			.MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
			.Matches("[A-Z]").WithMessage("Password must contain at least one UPPERCASE letter.")
			.Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
			.Matches("[0-9]").WithMessage("Password must contain at least one Number.")
			.Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character (e.g., !@#$%^&*).");
	}
}
