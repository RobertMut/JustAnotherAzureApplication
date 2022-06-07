namespace Application.Images.Queries.GetUserFiles
{
    /// <summary>
    /// Class UserFilesListVm
    /// </summary>
    public class UserFilesListVm
    {
        /// <summary>
        /// User files
        /// </summary>
        public IEnumerable<FileDto> Files { get; set; }
    }
}
