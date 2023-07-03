namespace API.AutomatedTests.Implementation.Common.TestingModels;

public class FileList {
    private IEnumerable<File> Files { get; set; }
}

public class File
{
    public string Filename { get; set; }

    public bool IsOwned { get; set; }

    public string Permission { get; set; }

    public string OriginalName { get; set; }
}