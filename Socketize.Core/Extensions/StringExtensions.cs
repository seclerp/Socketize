using System;

namespace Socketize.Core.Extensions
{
    /// <summary>
    /// Extensions for <see cref="string"/> type.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Combines two strings into one, using specific string as separator.
        /// </summary>
        /// <param name="first">First string to be combined.</param>
        /// <param name="second">Second string to be combined.</param>
        /// <param name="separator">Separator string, placed between first and second strings.</param>
        /// <returns>Combined first and seconds strings, placed between separator.</returns>
        /// <exception cref="ArgumentException">Fires when second string is null, empty or contains only whitespaces.</exception>
        public static string CombineWith(this string first, string second, string separator = "/")
        {
            if (string.IsNullOrWhiteSpace(second))
            {
                throw new ArgumentException("Argument cannot be null, empty string or whitespace", nameof(second));
            }

            return string.IsNullOrWhiteSpace(first)
                ? second
                : CombineWithInternal(first, second, separator);
        }

        private static string CombineWithInternal(string first, string second, string separator)
        {
            return $"{first}{separator}{second}";
        }
    }
}