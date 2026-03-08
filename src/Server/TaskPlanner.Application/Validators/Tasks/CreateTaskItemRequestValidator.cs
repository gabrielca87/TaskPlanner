using FluentValidation;
using TaskPlanner.Application.Requests.Tasks;

namespace TaskPlanner.Application.Validators.Tasks;

public class CreateTaskItemRequestValidator : AbstractValidator<CreateTaskItemRequest>
{
    public CreateTaskItemRequestValidator()
    {
        RuleFor(request => request.UserId)
            .NotEmpty();

        RuleFor(request => request.Title)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(request => request.Description)
            .MaximumLength(1000);
    }
}
