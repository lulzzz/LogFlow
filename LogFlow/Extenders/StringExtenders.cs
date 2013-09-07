using System.IO;

namespace LogFlow
{
    internal static class StringExtenders
    {
        public static bool IsPath(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            try
            {
                var path = Path.GetFullPath(value);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
