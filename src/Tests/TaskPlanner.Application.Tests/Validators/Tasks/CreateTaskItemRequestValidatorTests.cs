using TaskPlanner.Application.Requests.Tasks;
using TaskPlanner.Application.Validators.Tasks;

namespace TaskPlanner.Application.Tests.Validators.Tasks;

public class CreateTaskItemRequestValidatorTests
{
    [Fact]
    public void Validate_WhenRequestIsValid_HasNoErrors()
    {
        var validator = new CreateTaskItemRequestValidator();
        var request = new CreateTaskItemRequest
        {
            UserId = Guid.NewGuid(),
            Title = "Create architecture tests",
            Description = "Validate application behaviors"
        };

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WhenUserIdIsEmpty_HasErrorForUserId()
    {
        var validator = new CreateTaskItemRequestValidator();
        var request = new CreateTaskItemRequest
        {
            UserId = Guid.Empty,
            Title = "Create architecture tests",
            Description = "Validate application behaviors"
        };

        var result = validator.Validate(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateTaskItemRequest.UserId));
    }

    [Fact]
    public void Validate_WhenTitleIsEmpty_HasErrorForTitle()
    {
        var validator = new CreateTaskItemRequestValidator();
        var request = new CreateTaskItemRequest
        {
            UserId = Guid.NewGuid(),
            Title = string.Empty,
            Description = "Validate application behaviors"
        };

        var result = validator.Validate(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateTaskItemRequest.Title));
    }

    [Fact]
    public void Validate_WhenTitleAndDescriptionExceedsLimit_HasErrorForTitleAndDescription()
    {
        var validator = new CreateTaskItemRequestValidator();
        var request = new CreateTaskItemRequest
        {
            UserId = Guid.NewGuid(),
            Title = new string('x', 251),
            Description = new string('x', 1001),
        };

        var result = validator.Validate(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateTaskItemRequest.Title));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateTaskItemRequest.Description));
    }
}
