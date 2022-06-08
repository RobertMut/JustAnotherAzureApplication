using Application.Common.Exceptions;
using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Application.Common.Models.File;
using Application.Images.Queries.GetFile;
using Application.UnitTests.Common.Fakes;
using Azure.Storage.Blobs.Models;
using MediatR;
using Moq;
using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UnitTests.Images.Queries.GetFile
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class GetFileQueryTests
    {
        private Mock<IBlobManagerService> _service;
        private Mock<IMediator> _mediator;
        private IUnitOfWork _unitOfWork;

        [SetUp]
        public async Task SetUp()
        {
            var blob = new Mock<BlobDownloadResult>();
            _service = new Mock<IBlobManagerService>();
            _mediator = new Mock<IMediator>();
            _unitOfWork = new FakeUnitOfWork();

            _service.Setup(x => x.DownloadAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(blob.Object);
            _mediator.Setup(x => x.Send(It.IsAny<GetFileQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new FileVm()
            {
                File = blob.Object
            });
        }

        [Test]
        public async Task GetFile()
        {
            var handler = new GetFileQueryHandler(_service.Object, _unitOfWork);
            var query = new GetFileQuery()
            {
                Filename = "miniature_300x300_test.Png",
                UserId = DbSets.UserId.ToString()
            };

            Assert.DoesNotThrowAsync(async () =>
            {
                var responseFromHandler = await handler.Handle(query, CancellationToken.None);
                var response = await _mediator.Object.Send(query, CancellationToken.None);

                Assert.IsInstanceOf<FileVm>(responseFromHandler);
                Assert.IsInstanceOf<FileVm>(response);
            });
        }

        [Test]
        public async Task GetOtherUserFileThrowsFileNotFoundException()
        {
            var handler = new GetFileQueryHandler(_service.Object, _unitOfWork);
            var query = new GetFileQuery()
            {
                Filename = "miniature_300x300_notshared.Png",
                UserId = Guid.NewGuid().ToString()
            };

            Assert.ThrowsAsync<FileNotFoundException>(async () =>
            {
                var responseFromHandler = await handler.Handle(query, CancellationToken.None);
                var response = await _mediator.Object.Send(query, CancellationToken.None);

                Assert.IsInstanceOf<FileVm>(responseFromHandler);
                Assert.IsInstanceOf<FileVm>(response);
            });
        }
    }
}
