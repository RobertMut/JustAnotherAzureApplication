using System;
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
        var factory = new CustomWebApplicationFactory<Program>();
        AuthenticatedHttpClient = factory.GetAuthenticatedClient().Result;
    }

    /// <summary>
    /// Kills and clean after tests
    /// </summary>
    [OneTimeTearDown]
    public static void RunAfterTests()
    {
        //var process = Process.GetProcessById(_processId);
        //process.Kill();
    }
    
    /// <summary>
    /// Starts fake storage
    /// </summary>
    /// <returns>Process id</returns>
    private static int RunFakeStorage()
    {
        string storageEmulatorPath = Environment.GetEnvironmentVariable("storageEmulator");
        string storageEmulatorArgs = Environment.GetEnvironmentVariable("StorageEmulatorArgs");
        Process storageEmulatorProcess = new Process();
            
        storageEmulatorProcess.StartInfo = new ProcessStartInfo
        {
            FileName = storageEmulatorPath,
            WindowStyle = ProcessWindowStyle.Hidden,
            Arguments = storageEmulatorArgs,

        };
            
        storageEmulatorProcess.Start();

        return storageEmulatorProcess.Id;
    }
}