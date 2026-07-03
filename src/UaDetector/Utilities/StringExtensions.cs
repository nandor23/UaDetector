namespace UaDetector.Utilities;

internal static class StringExtensions
{
    // Upper bound, in bytes, for stack allocation. The Microsoft docs use 1024 but recommend choosing a
    // conservative value (https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/stackalloc).
    // A low value suffices here since the inputs are short
    private const int MaxStackallocBytes = 256;

    extension(string text)
    {
        /// <summary>
        /// Collapses multiple spaces in the input string into a single space and trims leading/trailing spaces.
        /// </summary>
        public string CollapseSpaces()
        {
            Span<char> buffer =
                text.Length <= MaxStackallocBytes / sizeof(char)
                    ? stackalloc char[text.Length]
                    : new char[text.Length];

            int count = 0;
            bool isSpace = true;

            foreach (char c in text)
            {
                if (c == ' ')
                {
                    if (!isSpace)
                    {
                        buffer[count++] = ' ';
                        isSpace = true;
                    }
                }
                else
                {
                    buffer[count++] = c;
                    isSpace = false;
                }
            }

            return buffer[count - 1] == ' '
                ? buffer[..(count - 1)].ToString()
                : buffer[..count].ToString();
        }

        /// <summary>
        /// Removes all spaces from the input string.
        /// </summary>
        public string RemoveSpaces()
        {
            Span<char> buffer =
                text.Length <= MaxStackallocBytes / sizeof(char)
                    ? stackalloc char[text.Length]
                    : new char[text.Length];

            int count = 0;

            foreach (char c in text)
            {
                if (c != ' ')
                {
                    buffer[count++] = c;
                }
            }

            return buffer[..count].ToString();
        }

        /// <summary>
        /// Reports the zero-based index of the n-th occurrence of the specified Unicode character in this string
        /// </summary>
        /// <returns>The zero-based index position of value if that character is found, or -1 if it is not.</returns>
        public int IndexOfNthOccurrence(char value, int n)
        {
            int count = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == value)
                {
                    count++;

                    if (count == n)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }
    }
}
