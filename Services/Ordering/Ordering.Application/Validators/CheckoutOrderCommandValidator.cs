using FluentValidation;
using Ordering.Application.Commands;

namespace Ordering.Application.Validators
{
    public class CheckoutOrderCommandValidator : AbstractValidator<CheckoutOrderCommand>
    {
        public CheckoutOrderCommandValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("{UserName} is required.")
                .NotNull()
                .MaximumLength(70).WithMessage("{UserName} must not exceed {MaxLength} characters.");
            RuleFor(x => x.TotalPrice)
                .NotEmpty().WithMessage("{TotalPrice} is required.")
                .NotNull()
                .GreaterThan(-1).WithMessage("{TotalPrice} should not be -ve.");
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("{FirstName} is required.")
                .NotNull()
                .MaximumLength(70).WithMessage("{FirstName} must not exceed {MaxLength} characters.");
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("{LastName} is required.")
                .NotNull()
                .MaximumLength(70).WithMessage("{LastName} must not exceed {MaxLength} characters.");
            RuleFor(x => x.EmailAddress)
                .NotEmpty().WithMessage("{EmailAddress} is required.")
                .NotNull()
                .EmailAddress().WithMessage("{EmailAddress} is not a valid email address.");
        }
    }
}
