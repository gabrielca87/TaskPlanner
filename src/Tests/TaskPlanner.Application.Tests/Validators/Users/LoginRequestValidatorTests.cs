using TaskPlanner.Application.Requests.Users;
using TaskPlanner.Application.Validators.Users;

namespace TaskPlanner.Application.Tests.Validators.Users;

public class LoginRequestValidatorTests
{
    [Fact]
    public void Validate_WhenRequestIsValid_HasNoErrors()
    {
        var validator = new LoginRequestValidator();
        var request = new LoginRequest
        {
            Email = "planner@taskplanner.dev",
            Password = "Planner@123"
        };

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WhenEmailIsInvalid_HasErrorForEmail()
    {
        var validator = new LoginRequestValidator();
        var request = new LoginRequest
        {
            Email = "invalid-email",
            Password = "Planner@123"
        };

        var result = validator.Validate(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(LoginRequest.Email));
    }

    [Fact]
    public void Validate_WhenPasswordIsEmpty_HasErrorForPassword()
    {
        var validator = new LoginRequestValidator();
        var request = new LoginRequest
        {
            Email = "planner@taskplanner.dev",
            Password = string.Empty
        };

        var result = validator.Validate(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(LoginRequest.Password));
    }
}
