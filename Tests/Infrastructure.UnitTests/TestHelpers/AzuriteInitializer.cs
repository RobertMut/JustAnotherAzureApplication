using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Azure.Storage.Blobs;

namespace Infrastructure.UnitTests.TestHelpers;

[ExcludeFromCodeCoverage]
public class AzuriteInitializer
{
    public const string DefaultConnectionString =
        @"DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";

    public const string DefaultContainer = "unittest";
    
    private readonly Process _process;
    private BlobContainerClient _blobContainerClient;
    private List<string> _uploadedFiles;
    
    public AzuriteInitializer()
    {
        Process process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "CMD.exe",
                WorkingDirectory = Directory.GetCurrentDirectory(),
                Arguments = $"/c azurite",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Normal,
                RedirectStandardOutput = false,
            }
        };
        
        if (process.Start())
        {
            _process = process;
            Thread.Sleep(1000);
        }

        _uploadedFiles = new List<string>();
    }
    public async Task CreateUnitTestContainer(string connectionString = DefaultConnectionString, string container = DefaultContainer, bool createAgain = true)
    {
        _blobContainerClient = new BlobContainerClient(connectionString, container);

        if (createAgain)
        {
            await _blobContainerClient.DeleteIfExistsAsync();
        }
        
        await _blobContainerClient.CreateIfNotExistsAsync();
    }

    public async Task<BlobClient> CreateSampleFile(string name, string connectionString = DefaultConnectionString, string container = DefaultContainer)
    {
        if (_blobContainerClient == null)
        {
            await CreateUnitTestContainer(DefaultConnectionString, DefaultContainer);
        }
        
        var blobClient = _blobContainerClient.GetBlobClient(name);
        
        await blobClient.DeleteIfExistsAsync();
        await blobClient.UploadAsync(new MemoryStream(new byte[] { 255, 255, 255 }));
        
        _uploadedFiles.Add(name);
        return blobClient;
    }

    public async Task<BlobClient> GetBlobClient(string name) => _blobContainerClient.GetBlobClient(name);

    public async Task Kill()
    {
        _process.Kill();
        await KillAzuriteNodeProcess();
    }

    private async Task KillAzuriteNodeProcess()
    {
        Process powershell = new Process();
        powershell.StartInfo = new ProcessStartInfo
        {
            Arguments = "Get-Process -Id (Get-NetTCPConnection -LocalPort 10000).OwningProcess | foreach {$_.Kill()}",
            CreateNoWindow = true,
            FileName = "powershell.exe",
            WindowStyle = ProcessWindowStyle.Hidden,
        };

        powershell.Start();
    }
}