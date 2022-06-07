using Domain.Constants.Image;
using System.Security.Cryptography;
using System.Text;

namespace Domain.Common.Helper.Filename
{
    /// <summary>
    /// Class NameHelper
    /// </summary>
    public static class NameHelper
    {
        /// <summary>
        /// Generate name with "original_" prefix and user id
        /// </summary>
        /// <param name="userId">User guid</param>
        /// <param name="filename">filename</param>
        /// <returns>String with "original_" prefix, guid and filename</returns>
        public static string GenerateOriginal(string userId, string filename)
        {
            return $"{Prefixes.OriginalImage}{userId}{Name.Delimiter}{filename}";
        }

        /// <summary>
        /// Generate name with "miniature_" prefix, user id and size
        /// </summary>
        /// <param name="userId">User guid</param>
        /// <param name="size">Image size</param>
        /// <param name="filename">Filename</param>
        /// <returns>String with "miniature_" prefix, guid and filename</returns>
        public static string GenerateMiniature(string userId, string size, string filename)
        {
            return $"{Prefixes.MiniatureImage}{size}{Name.Delimiter}{userId}{Name.Delimiter}{filename}";
        }
        
        /// <summary>
        /// Hashes filename and replaces original filename with first 10 characters from hash
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <returns>Hashed filename with extension</returns>
        public static string GenerateHashedFilename(string filename)
        {
            string nameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
            string extension = Path.GetExtension(filename);
            var hashedString = SHA256.HashData(Encoding.UTF8.GetBytes(nameWithoutExtension)).Take(10).ToArray();
            return BitConverter.ToString(hashedString).Replace("-", string.Empty).ToLower() + extension;
        }
    }
}
