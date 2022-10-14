using API.AutomatedTests.Implementation.Common.Interfaces;
using BoDi;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace API.AutomatedTests.Implementation.Steps;

[Binding]
public class StorageSteps
{
    private readonly IBlobService blobService;

    public StorageSteps(IObjectContainer objectContainer)
    {
        blobService = objectContainer.Resolve<IBlobService>();
    }
    #region Then

    [Then("I check if file named by '(.*)' exists on blob")]
    public async Task ICheckIfFileNamedByExistsOnBlob(string filename)
    {
        bool exists = await blobService.CheckIfBlobExists(filename, new CancellationToken());

        exists.Should().BeTrue();
    }
    
    #endregion
}