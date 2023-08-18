using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using API.AutomatedTests.Implementation.Common.Interfaces;
using API.AutomatedTests.Implementation.Common.Options;

namespace API.AutomatedTests.Infrastructure.CommandRunner;

[ExcludeFromCodeCoverage]
public class LocalCommandRunner : ICommandRunner
{
    private readonly LocalRunnerOptions _options;
    private readonly ConnectionStringsOptions _connectionStringsOptions;

    public LocalCommandRunner(LocalRunnerOptions options, ConnectionStringsOptions connectionStringsOptions)
    {
        _options = options;
        _connectionStringsOptions = connectionStringsOptions;
    }

    public Process Execute(string command, string workingDirectory)
    {
        string path = Path.GetFullPath(workingDirectory, AppDomain.CurrentDomain.BaseDirectory);

        Process process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "CMD.exe",
                WorkingDirectory = path,
                Arguments = $"/c {command}",
                CreateNoWindow = _options.CreateNoWindow,
                WindowStyle = ProcessWindowStyle.Normal,
                RedirectStandardOutput = false,
                Environment =
                {
                    {"ASPNETCORE_ENVIRONMENT", "Testing"},
                    { "JAAADatabase", _connectionStringsOptions.JAAADatabase },
                    { "AzureWebJobsStorage", _connectionStringsOptions.AzureWebJobsStorage },
                    { "ImagesContainer", "jaaa"}
                }
            }
        };
        
        if (process.Start())
        {
            return process;
        }

        throw new Exception("Process function not started!");
    }
}