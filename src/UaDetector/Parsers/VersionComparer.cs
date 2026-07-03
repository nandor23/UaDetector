namespace UaDetector.Parsers;

/// <summary>
/// Compares dot-separated numeric version strings. A longer version outranks a shorter one that
/// shares its prefix (e.g. "1.0" is greater than "1"). A null or empty version is treated as not
/// comparable, so every comparison against one returns false and callers don't need to pre-check.
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

            TryParseVersionSegment(segment1, out int value1);
            TryParseVersionSegment(segment2, out int value2);

            int comparison = value1.CompareTo(value2);

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

    private static bool TryParseVersionSegment(ReadOnlySpan<char> segment, out int value)
    {
#if NET6_0_OR_GREATER
        return int.TryParse(segment, out value);
#else
        return int.TryParse(segment.ToString(), out value);
#endif
    }
}
