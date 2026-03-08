using FluentValidation;
using TaskPlanner.Application.Requests.Users;

namespace TaskPlanner.Application.Validators.Users;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(request => request.DisplayName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.Password)
            .NotEmpty()
            .MaximumLength(1000);
    }
}
