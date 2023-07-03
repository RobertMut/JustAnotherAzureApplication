using System.Diagnostics;
using API.AutomatedTests.Implementation.Common.Interfaces;
using API.AutomatedTests.Implementation.Common.Options;

namespace API.AutomatedTests.Infrastructure.CommandRunner;

public class LocalCommandRunner : ICommandRunner
{
    private readonly LocalRunnerOptions _options;

    public LocalCommandRunner(LocalRunnerOptions options)
    {
        _options = options;
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
                RedirectStandardOutput = true,
                Environment = { {"ASPNETCORE_ENVIRONMENT", "Testing"} }
            }
        };
        
        if (process.Start())
        {
            return process;
        }

        throw new Exception("Process function not started!");
    }
}