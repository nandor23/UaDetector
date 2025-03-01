using System.Text.RegularExpressions;

using UADetector.Results;

namespace UADetector;

public partial class UADetector : IUADetector
{
    private const string ClientHintsFragmentPattern = @"Android (?:10[.\d]*; K(?: Build/|[;)])|1[1-5]\)) AppleWebKit";
    private const string DesktopFragmentPattern = "(?:Windows (?:NT|IoT)|X11; Linux x86_64)";

    private const string ExcludeDesktopFragmentPattern =
        "CE-HTML|" +
        "Mozilla/|Andr[o0]id|Tablet|Mobile|iPhone|Windows Phone|ricoh|OculusBrowser|" +
        "PicoBrowser|Lenovo|compatible; MSIE|Trident/|Tesla/|XBOX|FBMD/|ARM; ?([^)]+)";

    private const string LinuxDesktopFragmentPattern = "X11; Linux x86_64";


#if NET7_0_OR_GREATER
    [GeneratedRegex(ClientHintsFragmentPattern, RegexOptions.IgnoreCase)]
    private static partial Regex ClientHintsFragmentRegex();

    [GeneratedRegex(DesktopFragmentPattern)]
    private static partial Regex DesktopFragmentRegex();
    
    [GeneratedRegex(ExcludeDesktopFragmentPattern)]
    private static partial Regex ExcludeDesktopFragmentRegex();
        
    [GeneratedRegex(LinuxDesktopFragmentPattern)]
    private static partial Regex LinuxDesktopFragmentRegex();
#else
    private static readonly Regex ClientHintsFragmentRegexInstance =
        new(ClientHintsFragmentPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex DesktopFragmentRegexInstance = new(DesktopFragmentPattern, RegexOptions.Compiled);

    private static readonly Regex ExcludeDesktopFragmentRegexInstance =
        new(ExcludeDesktopFragmentPattern, RegexOptions.Compiled);

    private static readonly Regex LinuxDesktopFragmentRegexInstance =
        new(LinuxDesktopFragmentPattern, RegexOptions.Compiled);


    private static Regex ClientHintsFragmentRegex() => ClientHintsFragmentRegexInstance;
    private static Regex DesktopFragmentRegex() => DesktopFragmentRegexInstance;
    private static Regex ExcludeDesktopFragmentRegex() => ExcludeDesktopFragmentRegexInstance;
    private static Regex LinuxDesktopFragmentRegex() => LinuxDesktopFragmentRegexInstance;

#endif

    private static bool HasUserAgentClientHintsFragment(string userAgent)
    {
        return ClientHintsFragmentRegex().IsMatch(userAgent);
    }

    private static bool HasUserAgentDesktopFragment(string userAgent)
    {
        return DesktopFragmentRegex().IsMatch(userAgent) && !ExcludeDesktopFragmentRegex().IsMatch(userAgent);
    }


    private static bool TryRestoreUserAgentFromClientHints(
        string userAgent,
        ClientHints? clientHints,
        out string? updatedUserAgent
    )
    {
        if (clientHints?.Model is null)
        {
            updatedUserAgent = null;
            return false;
        }

        if (HasUserAgentClientHintsFragment(userAgent))
        {
            userAgent =
                $"Android {(string.IsNullOrEmpty(clientHints.PlatformVersion) ? "10" : clientHints.PlatformVersion)}; {clientHints.Model}";
        }

        if (HasUserAgentDesktopFragment(userAgent))
        {
            updatedUserAgent = LinuxDesktopFragmentRegex()
                .Replace(userAgent, $"{LinuxDesktopFragmentPattern}; {clientHints.Model}");
            return true;
        }

        updatedUserAgent = null;
        return false;
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
