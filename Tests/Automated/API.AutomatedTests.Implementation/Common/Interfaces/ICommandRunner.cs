using System.Diagnostics;

namespace API.AutomatedTests.Implementation.Common.Interfaces;

public interface ICommandRunner
{
    Process Execute(string command, string workingDirectory);
}