namespace API.AutomatedTests.Implementation.Common.Options;

public class ConnectionStringsOptions
{
    public const string ConnectionStrings = "ConnectionStrings";
    public string JAAADatabase { get; set; }
    public string AzureWebJobsStorage { get; set; }
}