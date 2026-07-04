namespace UaDetector.Parsers;

/// <summary>
/// Compares dot-separated version strings segment by segment. Numeric segments compare by value, and
/// a numeric segment always outranks a non-numeric one (e.g. "8" is greater than "XP"). A longer
/// version outranks a shorter one that shares its prefix (e.g. "1.0" is greater than "1"). A null or
/// empty version is treated as not comparable, so every comparison against one returns false.
/// </summary>
internal static class VersionComparer
{
    public static bool IsGreaterThan(string? first, string? second) => Compare(first, second) is > 0;

    public static bool IsGreaterThanOrEqual(string? first, string? second) =>
        Compare(first, second) is >= 0;

    public static bool IsLessThan(string? first, string? second) => Compare(first, second) is < 0;

    public static bool IsLessThanOrEqual(string? first, string? second) =>
        Compare(first, second) is <= 0;

    public static bool AreEqual(string? first, string? second) => Compare(first, second) is 0;

    /// <returns>
    /// A negative value, zero, or a positive value when <paramref name="first"/> is respectively
    /// lower than, equal to, or greater than <paramref name="second"/>; or null when either version
    /// is null or empty and therefore not comparable.
    /// </returns>
    private static int? Compare(string? first, string? second)
    {
        if (string.IsNullOrEmpty(first) || string.IsNullOrEmpty(second))
        {
            return null;
        }

        ReadOnlySpan<char> firstSpan = first;
        ReadOnlySpan<char> secondSpan = second;

        int offset1 = 0,
            offset2 = 0;

        while (true)
        {
            var segment1 = NextDotSegment(firstSpan, ref offset1, out bool hasSegment1);
            var segment2 = NextDotSegment(secondSpan, ref offset2, out bool hasSegment2);

            // The version with the extra segment ranks higher once the other runs out.
            if (!hasSegment1 || !hasSegment2)
            {
                return hasSegment1 == hasSegment2 ? 0 : (hasSegment1 ? 1 : -1);
            }

            int comparison = CompareSegments(segment1, segment2);

            if (comparison != 0)
            {
                return comparison;
            }
        }
    }

    /// <summary>
    /// Returns the next '.'-delimited segment starting at <paramref name="offset"/>, mirroring
    /// <see cref="string.Split(char[])"/> semantics without allocating. <paramref name="hasSegment"/>
    /// is set to false once the string has been fully consumed.
    /// </summary>
    private static ReadOnlySpan<char> NextDotSegment(
        ReadOnlySpan<char> text,
        ref int offset,
        out bool hasSegment
    )
    {
        if (offset > text.Length)
        {
            hasSegment = false;
            return default;
        }

        hasSegment = true;
        var remaining = text[offset..];
        int dotIndex = remaining.IndexOf('.');

        if (dotIndex < 0)
        {
            offset = text.Length + 1;
            return remaining;
        }

        offset += dotIndex + 1;
        return remaining[..dotIndex];
    }

    private static int CompareSegments(ReadOnlySpan<char> first, ReadOnlySpan<char> second)
    {
        bool firstIsNumeric = TryParseInt(first, out int firstValue);
        bool secondIsNumeric = TryParseInt(second, out int secondValue);

        // Both numeric: compare by value so "9" ranks below "10".
        if (firstIsNumeric && secondIsNumeric)
        {
            return firstValue.CompareTo(secondValue);
        }

        // A numeric segment outranks a non-numeric one, so "8" is greater than "XP" and a named
        // version like "Vista" is never treated as equal to "0".
        if (firstIsNumeric != secondIsNumeric)
        {
            return firstIsNumeric ? 1 : -1;
        }

        // Both non-numeric: compare textually for a stable, predictable ordering.
        return CompareOrdinal(first, second);
    }

    private static bool TryParseInt(ReadOnlySpan<char> segment, out int value)
    {
#if NET6_0_OR_GREATER
        return int.TryParse(segment, out value);
#else
        return int.TryParse(segment.ToString(), out value);
#endif
    }

    private static int CompareOrdinal(ReadOnlySpan<char> first, ReadOnlySpan<char> second)
    {
#if NET6_0_OR_GREATER
        return first.CompareTo(second, StringComparison.Ordinal);
#else
        return string.CompareOrdinal(first.ToString(), second.ToString());
#endif
    }
}
