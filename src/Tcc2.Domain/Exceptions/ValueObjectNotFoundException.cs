using System.Runtime.Serialization;

namespace Tcc2.Domain.Exceptions;

[Serializable]
public class ValueObjectNotFoundException : Exception
{
    public ValueObjectNotFoundException()
    {
    }

    public ValueObjectNotFoundException(string? message) : base(message)
    {
    }

    public ValueObjectNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ValueObjectNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
