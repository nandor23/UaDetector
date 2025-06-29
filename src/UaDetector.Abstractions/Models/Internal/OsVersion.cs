using System.Text.RegularExpressions;

namespace UaDetector.Abstractions.Models.Internal;

internal sealed class OsVersion
{
    public required Regex Regex { get; init; }
    public required string Version { get; init; }
}
