namespace Application.Common.Exceptions;

public class DuplicatedException : Exception
{
    public DuplicatedException(string name) 
        : base($"{name} already exist!")
    {
    }
}