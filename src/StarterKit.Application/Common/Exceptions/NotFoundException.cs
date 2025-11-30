namespace StarterKit.Application.Common.Exceptions;

public class NotFoundException : ApiException
{
    public NotFoundException(string name, object key)
        : base(404, $"Entity \"{name}\" ({key}) was not found.")
    {
    }
    
    public NotFoundException(string message) : base(404, message)
    {
    }
}