namespace Application.Common.Exceptions
{
    public class FileNotFoundException : Exception
    {
        public FileNotFoundException(string filename) 
            : base($"File {filename} not found!")
        {
        }
    }
}
