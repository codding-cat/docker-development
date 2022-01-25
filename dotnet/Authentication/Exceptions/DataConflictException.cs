namespace Authentication.Exceptions;

public class DataConflictException : Exception
{
    public DataConflictException(string message) : base(message) { }

}