using TaskPlanner.Application.Requests.Tasks;
using TaskPlanner.Application.Validators.Tasks;

namespace TaskPlanner.Application.Tests.Validators.Tasks;

public class UpdateTaskItemRequestValidatorTests
{
    [Fact]
    public void Validate_WhenRequestIsValid_HasNoErrors()
    {
        var validator = new UpdateTaskItemRequestValidator();
        var request = new UpdateTaskItemRequest
        {
            Id = Guid.NewGuid(),
            Title = "Refine persistence layer",
            Description = "Finalize integration tests"
        };

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_HasErrorForId()
    {
        var validator = new UpdateTaskItemRequestValidator();
        var request = new UpdateTaskItemRequest
        {
            Id = Guid.Empty,
            Title = "Refine persistence layer",
            Description = "Finalize integration tests"
        };

        var result = validator.Validate(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateTaskItemRequest.Id));
    }

    [Fact]
    public void Validate_WhenTitleIsEmpty_HasErrorForTitle()
    {
        var validator = new UpdateTaskItemRequestValidator();
        var request = new UpdateTaskItemRequest
        {
            Id = Guid.NewGuid(),
            Title = string.Empty,
            Description = "Finalize integration tests"
        };

        var result = validator.Validate(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateTaskItemRequest.Title));
    }

    [Fact]
    public void Validate_WhenTitleAndDescriptionExceedsLimit_HasErrorForTitleAndDescription()
    {
        var validator = new UpdateTaskItemRequestValidator();
        var request = new UpdateTaskItemRequest
        {
            Id = Guid.NewGuid(),
            Title = new string('x', 251),
            Description = new string('x', 1001),
        };

        var result = validator.Validate(request);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateTaskItemRequest.Title));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateTaskItemRequest.Description));
    }
}
