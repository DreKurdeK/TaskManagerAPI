using FluentValidation;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Validators;

public class ToDoValidator : AbstractValidator<ToDo>
{
    public ToDoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title maximum length is 100.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description maximum length is 500.");

        RuleFor(x => x.Expiry)
            .GreaterThanOrEqualTo(DateTimeOffset.UtcNow.AddDays(-7))
            .WithMessage("Expiry Date must be in the future.");

        RuleFor(x => x.PercentComplete)
            .InclusiveBetween(0, 100)
            .WithMessage("Percent Complete need to be between 0 and 100.");
    }
}