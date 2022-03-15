using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace API.IntegrationTests.Common
{
    public class Utils
    {
        public static IDictionary<string, BlobDownloadResult[]> Repository;
        public static byte[] SampleBytes = new byte[] { 00, 50, 00, 00, 40, 00, 03, 00, 00, 00, 00, 10 };

        public static MultipartFormDataContent CreateSampleFile(byte[] imageBytes, string contentType, string filename)
        {
            var requestContent = new MultipartFormDataContent();
            var imageContent = new ByteArrayContent(imageBytes);

            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
            requestContent.Add(imageContent, "file", filename);

            return requestContent;
        }

        public static BlobItemProperties MakeFakeBlobProperties(string contentType, string contentEncoding)
        {
            return BlobsModelFactory.BlobItemProperties(true, null, contentType, contentEncoding, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
        }

        public static BlobDownloadResult MakeFakeDownloadResult(Stream fileStream, string filename, string contentType, IDictionary<string, string>? metadata = null, string version = "0")
        {
            using (var memStream = new MemoryStream())
            {
                fileStream.CopyTo(memStream);
                var bytes = new BinaryData(memStream.ToArray());
                var details = BlobsModelFactory.BlobDownloadDetails(BlobType.Block, memStream.Length, contentType, new byte[] { 00 }, DateTimeOffset.Now, metadata, null, null, null, null, null, 1, DateTimeOffset.Now, null, null, null, null, CopyStatus.Success, LeaseDurationType.Infinite, LeaseState.Available, LeaseStatus.Unlocked, null, 1, false, null, null, new byte[] { 00 }, 0, version, false, null, null);

                return BlobsModelFactory.BlobDownloadResult(bytes, details);
            }
        }
    }
}
