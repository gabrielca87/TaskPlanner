namespace TaskPlanner.Api.Common;

public sealed class ErrorResponse
{
    public int StatusCode { get; init; }
    public string Message { get; init; } = string.Empty;
    public IReadOnlyDictionary<string, string[]>? Errors { get; init; }
}
