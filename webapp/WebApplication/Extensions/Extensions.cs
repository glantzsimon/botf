using System.Web;

namespace K9.WebApplication.Extensions
{
    public static class Extensions
    {
        public static string ToHyphenatedString(this string value)
        {
            return string.Join("-", value.Replace(":", "~").ToLower().Split(' '));
        }
        
        public static string ToPreviewText(this string value, int length = 100)
        {
            var valueLength = value.Length;
            var canBeAbbreviated = valueLength > length;
            var substring = value.Substring(0, canBeAbbreviated ? length : valueLength);
            var abbrevationSuffix = canBeAbbreviated ? "..." : string.Empty;
            return $"{substring}{abbrevationSuffix}";
        }
    }
}
