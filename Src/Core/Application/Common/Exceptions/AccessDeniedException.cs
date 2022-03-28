namespace Application.Common.Exceptions
{
    public class AccessDeniedException : Exception
    {
        public AccessDeniedException(string user, string obj) : 
            base($"Access denied for {user} to {obj}")
        {
        }
    }
}
