using Azure.Storage.Blobs.Models;
using Domain.Common.Helper.Filename;
using Domain.Entities;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using File = Domain.Entities.File;

namespace API.IntegrationTests.Common;

public class Utils
{
    public static IDictionary<string, BlobDownloadResult[]> Repository;
    public static byte[] SampleBytes = new byte[] { 00, 50, 00, 00, 40, 00, 03, 00, 00, 00, 00, 10 };
    public static Guid DefaultId = Guid.NewGuid();
    public static IDictionary<string, string> FileNames = new Dictionary<string, string>()
    {
        { "sample1", NameHelper.GenerateOriginal(DefaultId.ToString(), NameHelper.GenerateHashedFilename("sample1.png")) },
        { "miniature1sample1",  NameHelper.GenerateMiniature(DefaultId.ToString(), "300x300", NameHelper.GenerateHashedFilename("sample1.jpeg")) },
        { "miniature2sample1", NameHelper.GenerateMiniature(DefaultId.ToString(), "200x200", NameHelper.GenerateHashedFilename("sample1.jpeg")) },
        { "sample2", NameHelper.GenerateOriginal(DefaultId.ToString(), NameHelper.GenerateHashedFilename("sample2.png")) },
        { "miniature1sample2", NameHelper.GenerateMiniature(DefaultId.ToString(), "400x400", NameHelper.GenerateHashedFilename("sample2.jpeg")) },
        { "sample3", NameHelper.GenerateOriginal(DefaultId.ToString(), NameHelper.GenerateHashedFilename("sample3.png")) },
        { "miniature1sample3", NameHelper.GenerateMiniature(DefaultId.ToString(), "300x300", NameHelper.GenerateHashedFilename("sample3.jpeg")) }
    };

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
        return BlobsModelFactory.BlobItemProperties(true, null, contentType, contentEncoding);
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

    public static void InitializeDbForTests(JAAADbContext context)
    {
        context.Users.Add(
            new User
            {
                Id = DefaultId,
                Password = "12345",
                Username = "Default"
            });
        context.Files.AddRange(new[]{
            new File()
            {
                Filename = FileNames["sample1"],
                OriginalName = "sample1.png",
                UserId = DefaultId
            },
            new File()
            {
                Filename = FileNames["sample2"],
                OriginalName = "sample2.png",
                UserId = DefaultId
            },
            new File()
            {
                Filename = FileNames["sample3"],
                OriginalName = "sample3.png",
                UserId = DefaultId
            },
            new File()
            {
                Filename = FileNames["miniature1sample1"],
                OriginalName = NameHelper.GenerateHashedFilename("sample1.jpeg"),
                UserId = DefaultId
            },
            new File()
            {
                Filename = FileNames["miniature2sample1"],
                OriginalName = NameHelper.GenerateHashedFilename("sample1.jpeg"),
                UserId = DefaultId
            },
            new File()
            {
                Filename = FileNames["miniature1sample2"],
                OriginalName = NameHelper.GenerateHashedFilename("sample2.jpeg"),
                UserId = DefaultId
            },
            new File()
            {
                Filename = FileNames["miniature1sample3"],
                OriginalName = NameHelper.GenerateHashedFilename("sample3.jpeg"),
                UserId = DefaultId
            },
        });
        context.SaveChanges();
    }
}