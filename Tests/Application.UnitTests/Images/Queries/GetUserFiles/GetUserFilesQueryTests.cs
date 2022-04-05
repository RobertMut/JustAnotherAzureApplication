using Application.Common.Interfaces.Blob;
using Application.Common.Interfaces.Database;
using Application.Images.Queries.GetUserFiles;
using Application.UnitTests.Common.Fakes;
using Azure.Storage.Blobs.Models;
using Domain.Entities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UnitTests.Images.Queries.GetUserFiles
{
    public class GetUserFilesQueryTests
    {
        private IUnitOfWork _unitOfWork;

        [SetUp]
        public async Task SetUp()
        {
            _unitOfWork = new FakeUnitOfWork();
        }

        [Test]
        public async Task GetSharedFile()
        {
            var handler = new GetUserFilesQueryHandler(_unitOfWork);
            var query = new GetUserFilesQuery()
            {
                UserId = DbSets.UserId.ToString(),
            };

            Assert.DoesNotThrowAsync(async () =>
            {
                var responseFromHandler = await handler.Handle(query, CancellationToken.None);

                Assert.IsInstanceOf<UserFilesListVm>(responseFromHandler);
                Assert.IsTrue(responseFromHandler.Files.Count() > 0);
            });
        }
    }
}
