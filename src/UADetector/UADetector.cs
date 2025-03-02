using System.Text.RegularExpressions;

using UADetector.Results;

namespace UADetector;

public partial class UADetector : IUADetector
{
    private readonly UADetectorOptions _options;
    private const string ClientHintsFragmentMatchPattern = @"Android (?:10[.\d]*; K(?: Build/|[;)])|1[1-5]\)) AppleWebKit";
    private const string ClientHintsFragmentReplacementPattern = @"Android (?:10[\.\d]*; K|1[1-5])";
    private const string DesktopFragmentMatchPattern = "(?:Windows (?:NT|IoT)|X11; Linux x86_64)";
    private const string DesktopFragmentReplacementPattern = "X11; Linux x86_64";

    private const string DesktopFragmentExclusionPattern =
        "CE-HTML|" +
        "Mozilla/|Andr[o0]id|Tablet|Mobile|iPhone|Windows Phone|ricoh|OculusBrowser|" +
        "PicoBrowser|Lenovo|compatible; MSIE|Trident/|Tesla/|XBOX|FBMD/|ARM; ?([^)]+)";

#if NET7_0_OR_GREATER
    [GeneratedRegex(ClientHintsFragmentMatchPattern, RegexOptions.IgnoreCase)]
    private static partial Regex ClientHintsFragmentMatchRegex();
    
    [GeneratedRegex(ClientHintsFragmentReplacementPattern)]
    private static partial Regex ClientHintsFragmentReplacementRegex();

    [GeneratedRegex(DesktopFragmentMatchPattern)]
    private static partial Regex DesktopFragmentMatchRegex();
    
    [GeneratedRegex(DesktopFragmentReplacementPattern)]
    private static partial Regex DesktopFragmentReplacementRegex();
    
    [GeneratedRegex(DesktopFragmentExclusionPattern)]
    private static partial Regex DesktopFragmentExclusionRegex();
#else
    private static readonly Regex ClientHintsFragmentMatchInstance =
        new(ClientHintsFragmentMatchPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex ClientHintsFragmentReplacementInstance =
        new(ClientHintsFragmentReplacementPattern, RegexOptions.Compiled);

    private static readonly Regex DesktopFragmentMatchInstance =
        new(DesktopFragmentMatchPattern, RegexOptions.Compiled);

    private static readonly Regex DesktopFragmentReplacementInstance =
        new(DesktopFragmentReplacementPattern, RegexOptions.Compiled);

    private static readonly Regex DesktopFragmentExclusionInstance =
        new(DesktopFragmentExclusionPattern, RegexOptions.Compiled);


    private static Regex ClientHintsFragmentMatchRegex() => ClientHintsFragmentMatchInstance;
    private static Regex ClientHintsFragmentReplacementRegex() => ClientHintsFragmentReplacementInstance;
    private static Regex DesktopFragmentMatchRegex() => DesktopFragmentMatchInstance;
    private static Regex DesktopFragmentReplacementRegex() => DesktopFragmentReplacementInstance;
    private static Regex DesktopFragmentExclusionRegex() => DesktopFragmentExclusionInstance;
#endif


    public UADetector(UADetectorOptions? options = null)
    {
        _options = options ?? new UADetectorOptions();
    }

    private static bool HasUserAgentClientHintsFragment(string userAgent)
    {
        return ClientHintsFragmentMatchRegex().IsMatch(userAgent);
    }

    private static bool HasUserAgentDesktopFragment(string userAgent)
    {
        return DesktopFragmentMatchRegex().IsMatch(userAgent) && !DesktopFragmentExclusionRegex().IsMatch(userAgent);
    }


    private static bool TryRestoreUserAgentFromClientHints(
        string userAgent,
        ClientHints? clientHints,
        out string? result
    )
    {
        result = null;

        if (clientHints?.Model is null)
        {
            return false;
        }

        if (HasUserAgentClientHintsFragment(userAgent))
        {
            var platformVersion =
                string.IsNullOrEmpty(clientHints.PlatformVersion) ? "10" : clientHints.PlatformVersion;

            result = ClientHintsFragmentReplacementRegex()
                .Replace(userAgent, $"Android {platformVersion}; {clientHints.Model}");
        }

        if (HasUserAgentDesktopFragment(userAgent))
        {
            result = DesktopFragmentReplacementRegex()
                .Replace(userAgent, $"{DesktopFragmentReplacementPattern}; {clientHints.Model}");
        }

        return !string.IsNullOrEmpty(result);
    }

    public UserAgentInfo Parse(string userAgent, ClientHints clientHints)
    {
        if (TryRestoreUserAgentFromClientHints(userAgent, clientHints, out var restoredUserAgent) &&
            restoredUserAgent is not null)
        {
            userAgent = restoredUserAgent;
        }

        throw new NotImplementedException();
    }
}
