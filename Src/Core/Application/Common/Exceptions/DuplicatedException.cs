namespace Application.Common.Exceptions
{
    /// <summary>
    /// Class DuplicatedException
    /// </summary>
    public class DuplicatedException : Exception
    {
        /// <summary>
        /// Initializes new instance of <see cref="DuplicatedException" /> class.
        /// </summary>
        /// <param name="name">Duplicated filename</param>
        public DuplicatedException(string name) 
            : base($"{name} already exist!")
        {
        }
    }
}
