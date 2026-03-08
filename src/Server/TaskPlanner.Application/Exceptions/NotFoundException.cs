namespace TaskPlanner.Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string resource, string identifier)
        : base($"{resource} '{identifier}' was not found.")
    {
        Resource = resource;
        Identifier = identifier;
    }

    public string Resource { get; }

    public string Identifier { get; }
}
