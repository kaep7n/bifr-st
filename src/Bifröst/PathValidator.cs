using System;
using System.Linq;

namespace Bifröst
{
    public static class PathValidator
    {
        private static char[] validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~:/?#[]@!$&'()*+,;=".ToCharArray();

        public static void ValidateAndThrow(string fragment)
        {
            if (string.IsNullOrEmpty(fragment))
            {
                throw new ArgumentException($"Argument '{fragment}' must be not null or empty");
            }
            if (ContainsSlash(fragment))
            {
                throw new ArgumentException($"Argument '{fragment}' contains a slash");
            }
            if (ContainsInvalidChar(fragment))
            {
                throw new ArgumentException($"Fragment '{fragment}' contains invalid characters", nameof(fragment));
            }
        }

        public static bool ContainsSlash(string fragment)
        {
            if (fragment is null)
            {
                throw new ArgumentNullException(nameof(fragment));
            }

            return fragment.Contains("/", StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContainsInvalidChar(string fragment)
        {
            if (fragment is null)
            {
                throw new ArgumentNullException(nameof(fragment));
            }

            return !fragment.All(c => validChars.Contains(c));
        }
    }
}
