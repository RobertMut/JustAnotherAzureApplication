using Azure;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Functions.UnitTests.Common
{
    public class MockBlobBaseClient : BlobBaseClient
    {
        private readonly long _fileLenght;
        private readonly string _contentType;
        private readonly IDictionary<string, string> _metadata;

        public MockBlobBaseClient(long fileLenght, string contentType, IDictionary<string, string> metadata)
        {
            _fileLenght = fileLenght;
            _contentType = contentType;
            _metadata = metadata;
        }

        public override Response<BlobProperties> GetProperties(BlobRequestConditions? conditions = null, CancellationToken cancellationToken = default)
        {
            var properties = BlobsModelFactory.BlobProperties(default, default, _fileLenght, _contentType, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, _metadata, default, DateTimeOffset.Now);

            return new MockResponse<BlobProperties>(properties);
        }
    }
}
