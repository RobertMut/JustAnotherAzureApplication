namespace Application.Common.Exceptions
{
    /// <summary>
    /// Class FileNotFoundException
    /// </summary>
    public class FileNotFoundException : Exception
    {
        /// <summary>
        /// Initializes new instance of <see cref="FileNotFoundException" /> class.
        /// </summary>
        /// <param name="filename">Not found filename</param>
        public FileNotFoundException(string filename)
            : base($"File {filename} not found!")
        {
        }
    }
}
