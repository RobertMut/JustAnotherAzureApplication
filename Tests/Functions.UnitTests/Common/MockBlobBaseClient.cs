using Azure;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Functions.UnitTests.Common
{
    public class MockBlobBaseClient : BlobBaseClient
    {
        private readonly long fileLenght;

        protected MockBlobBaseClient(long fileLenght)
        {
            this.fileLenght = fileLenght;
        }

        public override Response<BlobProperties> GetProperties(BlobRequestConditions conditions = null, CancellationToken cancellationToken = default)
        {
            var r = BlobsModelFactory.BlobProperties(DateTimeOffset.Now, LeaseStatus.Unlocked, fileLenght, "image/bmp", default, LeaseState.Available, "image/bmp", null, null, null, default, LeaseDurationType.Infinite, null, null, 0, default, default, default, null, null, null, null, null, null, false, null);
            
            return base.GetProperties(conditions, cancellationToken);
        }
    }
}
