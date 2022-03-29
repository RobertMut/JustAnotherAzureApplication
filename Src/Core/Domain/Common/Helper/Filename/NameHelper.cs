using Domain.Constants.Image;
using System.Security.Cryptography;
using System.Text;

namespace Domain.Common.Helper.Filename
{
    public static class NameHelper
    {
        public static string GenerateOriginal(string userId, string filename)
        {
            return $"{Prefixes.OriginalImage}{userId}{Name.Delimiter}{filename}";
        }

        public static string GenerateMiniature(string userId, string size, string filename)
        {
            return $"{Prefixes.MiniatureImage}{size}{Name.Delimiter}{userId}{Name.Delimiter}{filename}";
        }
        
        public static string GenerateHashedFilename(string filename)
        {
            string nameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
            string extension = Path.GetExtension(filename);
            var hashedString = SHA256.HashData(Encoding.UTF8.GetBytes(nameWithoutExtension)).Take(10).ToArray();
            return BitConverter.ToString(hashedString).Replace("-", string.Empty).ToLower() + extension;
        }
    }
}
