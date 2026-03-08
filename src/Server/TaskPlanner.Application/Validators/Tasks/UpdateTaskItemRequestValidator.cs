using FluentValidation;
using TaskPlanner.Application.Requests.Tasks;

namespace TaskPlanner.Application.Validators.Tasks;

public class UpdateTaskItemRequestValidator : AbstractValidator<UpdateTaskItemRequest>
{
    public UpdateTaskItemRequestValidator()
    {
        RuleFor(request => request.Id)
            .NotEmpty();

        RuleFor(request => request.Title)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(request => request.Description)
            .MaximumLength(1000);
    }
}
