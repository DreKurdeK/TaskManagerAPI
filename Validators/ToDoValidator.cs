using FluentValidation;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Validators;

public class ToDoValidator : AbstractValidator<ToDo>
{
    public ToDoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title cannot be longer than 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description cannot be longer than 500 characters.");

        RuleFor(x => x.Expiry)
            .GreaterThan(DateTimeOffset.Now).WithMessage("Expiry Date must be in the future.");

        RuleFor(x => x.PercentComplete)
            .InclusiveBetween(0, 100).WithMessage("Percent Complete must be between 0 and 100.");
    }
}