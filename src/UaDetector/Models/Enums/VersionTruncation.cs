namespace UaDetector.Models.Enums;

public enum VersionTruncation
{
    /// <summary>
    /// No truncation; the full version is used.
    /// </summary>
    None = 0,

    /// <summary>
    /// Truncates the version to the major component only.
    /// Examples: 3, 5, 6, 200, 123.
    /// </summary>
    Major = 1,

    /// <summary>
    /// Truncates the version to the minor component.
    /// Examples: 3.4, 5.6, 6.234, 0.200, 1.23.
    /// </summary>
    Minor = 2,

    /// <summary>
    /// Truncates the version to the patch level.
    /// Examples: 3.4.0, 5.6.344, 6.234.2, 0.200.3, 1.2.3.
    /// </summary>
    Patch = 3,

    /// <summary>
    /// Truncates the version to include the build number.
    /// Examples: 3.4.0.12, 5.6.344.0, 6.234.2.3, 0.200.3.1, 1.2.3.0.
    /// </summary>
    Build = 4
}
