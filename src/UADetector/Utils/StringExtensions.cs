namespace UADetector.Utils;

public static class StringExtensions
{
    /// <summary>
    /// Collapses multiple spaces in the input string into a single space and trims leading/trailing spaces.
    /// </summary>
    public static string CollapseSpaces(this string text)
    {
        Span<char> buffer = new char[text.Length];
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

        return buffer[..(count - 1)].ToString();
    }

    /// <summary>
    /// Reports the zero-based index of the n-th occurrence of the specified Unicode character in this string
    /// </summary>
    /// <returns>The zero-based index position of value if that character is found, or -1 if it is not.</returns>
    public static int IndexOfNthOccurrence(this string text, char value, int n)
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
