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
}
