namespace Application.Common.Exceptions
{
    public class ShareNotFoundException : Exception
    {
        public ShareNotFoundException(string shareName, string id)
            : base($"Share {shareName} was not found for id {id}")
        {
        }
    }
}
