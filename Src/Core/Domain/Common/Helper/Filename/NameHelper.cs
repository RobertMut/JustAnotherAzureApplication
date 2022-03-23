using Domain.Constants.Image;

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
    }
}
