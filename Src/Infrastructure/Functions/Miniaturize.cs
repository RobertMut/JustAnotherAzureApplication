using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Drawing;
using System.Drawing.Imaging;

namespace Functions
{
    public class Miniaturize
    {
        private readonly IBlobManagerService _service;
        public Miniaturize(IBlobManagerService service)
        {
            _service = service;
        }
        [FunctionName("Miniaturize")]
        public async Task Run([BlobTrigger("jaaablob/{name}", Connection = "AzureWebJobsStorage")] Stream myBlob, string name, ILogger log)
        {
            using (var image = Image.FromStream(myBlob))
            {
                var resized = new Bitmap(image, 256, 256);
                using (var memStream = new MemoryStream())
                {
                    resized.Save(memStream, ImageFormat.Png);
                    memStream.Position = 0;
                    await _service.AddAsync(memStream, Path.GetFileNameWithoutExtension(name)+"-miniature.png",
                        "image/png", new CancellationToken());
                }
            }
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes\n");
        }
    }
}
