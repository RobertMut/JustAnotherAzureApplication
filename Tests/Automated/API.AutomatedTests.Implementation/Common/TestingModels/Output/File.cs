namespace API.AutomatedTests.Implementation.Common.TestingModels.Output;

public class File
{
    public string Filename { get; set; }

    public bool IsOwned { get; set; }

    public string OriginalName { get; set; }
    
    public string Permission { get; set; }
}