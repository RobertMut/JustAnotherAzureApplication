using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Behaviours;
using Application.Common.Interfaces.Blob;
using Application.Common.Models.File;
using Application.GroupShares.Queries.GetSharesByGroup;
using Application.Images.Queries.GetSharedFile;
using Infrastructure.Services.Blob;
using MediatR;
using Moq;
using NUnit.Framework;

namespace Application.UnitTests.Common.Behaviours;

[ExcludeFromCodeCoverage]
[TestFixture]
public class RequestSemaphoreBehaviourTests
{
    private Mock<IBlobLeaseManager> _blobLeaseManagerMock;
    
    [SetUp]
    public async Task SetUp()
    {
        _blobLeaseManagerMock = new Mock<IBlobLeaseManager>();
    }

    [Test]
    public async Task RequestSemaphoreBehaviourExecutesWithUserId()
    {
        Guid leaseId = Guid.NewGuid();
        
        _blobLeaseManagerMock.Setup(x => x.ReleaseLease(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _blobLeaseManagerMock.Setup(x => x.GetLease(It.IsAny<CancellationToken>())).ReturnsAsync(leaseId.ToString());
        _blobLeaseManagerMock.Setup(x => x.SetBlobName(It.IsAny<string>()))
            .Returns(_blobLeaseManagerMock.Object);

        var behaviour = new RequestSemaphoreBehaviour<GetSharedFileQuery, FileVm>(_blobLeaseManagerMock.Object);
        
        Assert.DoesNotThrowAsync(async () =>
        {
            await behaviour.Handle(new GetSharedFileQuery
            {
                Filename = "file",
                Id = 1,
                UserId = Guid.NewGuid().ToString()
            }, new CancellationToken(), Mock.Of<RequestHandlerDelegate<FileVm>>());
        });
    }
    
    [Test]
    public async Task RequestSemaphoreBehaviourExecutesWithGroupId()
    {
        Guid leaseId = Guid.NewGuid();
        
        _blobLeaseManagerMock.Setup(x => x.ReleaseLease(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _blobLeaseManagerMock.Setup(x => x.GetLease(It.IsAny<CancellationToken>())).ReturnsAsync(leaseId.ToString());
        _blobLeaseManagerMock.Setup(x => x.SetBlobName(It.IsAny<string>()))
            .Returns(_blobLeaseManagerMock.Object);

        var behaviour = new RequestSemaphoreBehaviour<GetSharesByGroupQuery, GroupSharesListVm>(_blobLeaseManagerMock.Object);
        
        Assert.DoesNotThrowAsync(async () =>
        {
            await behaviour.Handle(new GetSharesByGroupQuery
            {
                GroupId = Guid.NewGuid().ToString()
            }, new CancellationToken(), Mock.Of<RequestHandlerDelegate<GroupSharesListVm>>());
        });
    }
}