namespace Application.Images.Queries.GetUserFiles;

public class UserFilesListVm
{
    /// <summary>
    /// User files
    /// </summary>
    public IEnumerable<FileDto> Files { get; set; }
}