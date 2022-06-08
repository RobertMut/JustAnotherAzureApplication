using System.Diagnostics.CodeAnalysis;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Application.Common.Models.File;
using Application.Images.Queries.GetSharedFile;
using Application.UnitTests.Common.Fakes;
using Azure.Storage.Blobs.Models;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UnitTests.Images.Queries.GetSharedFile
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class GetSharedFileQueryTests
    {
        private Mock<IBlobManagerService> _service;
        private IUnitOfWork _unitOfWork;

        [SetUp]
        public async Task SetUp()
        {
            var blob = new Mock<BlobDownloadResult>();
            _service = new Mock<IBlobManagerService>();
            _unitOfWork = new FakeUnitOfWork();

            _service.Setup(x => x.DownloadAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(blob.Object);
        }

        [Test]
        public async Task GetSharedFile()
        {
            var handler = new GetSharedFileQueryHandler(_service.Object, _unitOfWork);
            var query = new GetSharedFileQuery()
            {
                Filename = "miniature_300x300_test.Png",
                UserId = DbSets.UserId.ToString(),
                OtherUserId = DbSets.SecondUserId.ToString(),
                GroupId = DbSets.GroupId.ToString()
            };

            Assert.DoesNotThrowAsync(async () =>
            {
                var responseFromHandler = await handler.Handle(query, CancellationToken.None);

                Assert.IsInstanceOf<FileVm>(responseFromHandler);
            });
        }

        [Test]
        public async Task GetSharedFileNotFound()
        {
            var handler = new GetSharedFileQueryHandler(_service.Object, _unitOfWork);
            var query = new GetSharedFileQuery()
            {
                Filename = "miniature_300x300_notshared.Png",
                UserId = DbSets.UserId.ToString(),
                OtherUserId = DbSets.SecondUserId.ToString(),
                GroupId = DbSets.GroupId.ToString()
            };

            Assert.ThrowsAsync<ShareNotFoundException>(async () =>
            {
                var responseFromHandler = await handler.Handle(query, CancellationToken.None);

                Assert.IsInstanceOf<FileVm>(responseFromHandler);
            });
        }
    }
}
