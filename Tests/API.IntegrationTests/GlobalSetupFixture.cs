using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using API.Controllers;
using API.IntegrationTests.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace API.IntegrationTests;

/// <summary>
/// Class GlobalSetupFixture
/// </summary>
[SetUpFixture]
[ExcludeFromCodeCoverage]
public class GlobalSetupFixture
{
    public static HttpClient AuthenticatedHttpClient;
    private static int _processId;
    
    /// <summary>
    /// Setups WebAppliationFactory and fake storage before tests
    /// </summary>
    [OneTimeSetUp]
    public static void RunBeforeAnyTests()
    {
        var factory = new CustomWebApplicationFactory<AccountsController>();
        var configuration = factory.Server.Services.GetRequiredService<IConfiguration>();
        AuthenticatedHttpClient = factory.GetAuthenticatedClient().Result;
        
        _processId = RunFakeStorage(configuration);
    }

    /// <summary>
    /// Kills and clean after tests
    /// </summary>
    [OneTimeTearDown]
    public static void RunAfterTests()
    {
        var process = Process.GetProcessById(_processId);
        process.Kill();
    }
    
    /// <summary>
    /// Starts fake storage
    /// </summary>
    /// <param name="configuration"><see cref="IConfiguration"/></param>
    /// <returns>Process id</returns>
    private static int RunFakeStorage(IConfiguration configuration)
    {
        string storageEmulatorPath = configuration.GetValue<string>("StorageEmulator");
        string storageEmulatorArgs = configuration.GetValue<string>("StorageEmulatorArgs");
        Process storageEmulatorProcess = new Process();
            
        storageEmulatorProcess.StartInfo = new ProcessStartInfo()
        {
            FileName = storageEmulatorPath,
            WindowStyle = ProcessWindowStyle.Hidden,
            Arguments = storageEmulatorArgs
        };
            
        storageEmulatorProcess.Start();

        return storageEmulatorProcess.Id;
    }
}