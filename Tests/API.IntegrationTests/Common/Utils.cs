using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace API.IntegrationTests.Common
{
    public class Utils
    {
        public static MultipartFormDataContent CreateSampleFile(byte[] imageBytes, string contentType, string filename)
        {
            var requestContent = new MultipartFormDataContent();
            var imageContent = new ByteArrayContent(imageBytes);

            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
            requestContent.Add(imageContent, "file", filename);

            return requestContent;

        }
    }
}
