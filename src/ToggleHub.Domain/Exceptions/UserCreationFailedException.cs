namespace ToggleHub.Domain.Exceptions;

public class UserCreationFailedException : Exception
{
    public UserCreationFailedException(string message)
        : base(message)
    {
    }
    public UserCreationFailedException(IEnumerable<string> messages)
        : base(string.Join("; ", messages))
    {
    }
    
}