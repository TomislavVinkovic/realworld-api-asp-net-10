namespace RealWorld.Exceptions;

public class ConduitValidationException : Exception
{
    public string Field { get; }
    public ConduitValidationException(string field, string message) : base(message)
    {
        Field = field;
    }
}