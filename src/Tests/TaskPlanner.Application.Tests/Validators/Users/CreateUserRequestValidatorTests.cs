using TaskPlanner.Application.Requests.Users;
using TaskPlanner.Application.Validators.Users;

namespace TaskPlanner.Application.Tests.Validators.Users;

public class CreateUserRequestValidatorTests
{
    [Fact]
    public void Validate_WhenRequestIsValid_HasNoErrors()
    {
        var validator = new CreateUserRequestValidator();
        var request = new CreateUserRequest
        {
            Email = "owner@taskplanner.dev",
            DisplayName = "Owner",
            Password = "P@ssw0rd!"
        };

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WhenEmailIsInvalid_HasErrorForEmail()
    {
        var validator = new CreateUserRequestValidator();
        var request = new CreateUserRequest
        {
            Email = "invalid-email",
            DisplayName = "Owner",
            Password = "P@ssw0rd!"
        };

        var result = validator.Validate(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserRequest.Email));
    }

    [Fact]
    public void Validate_WhenDisplayNameIsEmpty_HasErrorForDisplayName()
    {
        var validator = new CreateUserRequestValidator();
        var request = new CreateUserRequest
        {
            Email = "owner@taskplanner.dev",
            DisplayName = string.Empty,
            Password = "hash-value"
        };

        var result = validator.Validate(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserRequest.DisplayName));
    }

    [Fact]
    public void Validate_WhenPasswordIsEmpty_HasErrorForPassword()
    {
        var validator = new CreateUserRequestValidator();
        var request = new CreateUserRequest
        {
            Email = "owner@taskplanner.dev",
            DisplayName = "Owner",
            Password = string.Empty
        };

        var result = validator.Validate(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateUserRequest.Password));
    }
}
