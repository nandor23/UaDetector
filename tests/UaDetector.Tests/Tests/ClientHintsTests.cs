using Shouldly;

using TUnit.Core;

namespace UaDetector.Tests.Tests;

public class ClientHintsTests
{
    [Test]
    public void AllHeaderNameSets_ShouldUseOrdinalIgnoreCaseComparer()
    {
        ClientHints.ArchitectureHeaderNames.Comparer.ShouldBe(StringComparer.OrdinalIgnoreCase);
        ClientHints.BitnessHeaderNames.Comparer.ShouldBe(StringComparer.OrdinalIgnoreCase);
        ClientHints.MobileHeaderNames.Comparer.ShouldBe(StringComparer.OrdinalIgnoreCase);
        ClientHints.ModelHeaderNames.Comparer.ShouldBe(StringComparer.OrdinalIgnoreCase);
        ClientHints.PlatformHeaderNames.Comparer.ShouldBe(StringComparer.OrdinalIgnoreCase);
        ClientHints.PlatformVersionHeaderNames.Comparer.ShouldBe(StringComparer.OrdinalIgnoreCase);
        ClientHints.UaFullVersionHeaderNames.Comparer.ShouldBe(StringComparer.OrdinalIgnoreCase);
        ClientHints.PrimaryFullVersionListHeaderNames.Comparer.ShouldBe(
            StringComparer.OrdinalIgnoreCase
        );
        ClientHints.SecondaryFullVersionListHeaderNames.Comparer.ShouldBe(
            StringComparer.OrdinalIgnoreCase
        );
        ClientHints.AppHeaderNames.Comparer.ShouldBe(StringComparer.OrdinalIgnoreCase);
        ClientHints.FormFactorsHeaderNames.Comparer.ShouldBe(StringComparer.OrdinalIgnoreCase);
    }

    [Test]
    [MethodDataSource(nameof(HeadersTestData))]
    public void Create_ShouldCorrectlyParseHeaders(
        Dictionary<string, string?> headers,
        ClientHintsResult result
    )
    {
        var clientHints = ClientHints.Create(headers);

        clientHints.IsMobile.ShouldBe(result.IsMobile);
        clientHints.Platform.ShouldBe(result.Platform);
        clientHints.PlatformVersion.ShouldBe(result.PlatformVersion);
        clientHints.FullVersionList.ShouldBe(result.FullVersionList);
        clientHints.Model.ShouldBe(result.Model);
        clientHints.FormFactors.ShouldBe(result.FormFactors);
    }

    [Test]
    public void Create_ShouldIgnoreInvalidEntriesInVersionList()
    {
        var headers = new Dictionary<string, string?>
        {
            {
                "Sec-CH-UA-Full-Version-List",
                """
                    " Not A;Brand";v="99.0.0.0", "Chromium";v="99.0.4844.51", v="98.0.4758.109"
                    """
            },
        };

        var clientHints = ClientHints.Create(headers);

        clientHints.FullVersionList.ShouldBe(
            new Dictionary<string, string>
            {
                { " Not A;Brand", "99.0.0.0" },
                { "Chromium", "99.0.4844.51" },
            }
        );
    }

    public static IEnumerable<
        Func<(Dictionary<string, string?>, ClientHintsResult)>
    > HeadersTestData()
    {
        yield return () =>
            (
                new Dictionary<string, string?>
                {
                    {
                        "sec-ch-ua",
                        """
                        "Opera";v="83", " Not;A Brand";v="99", "Chromium";v="98"
                        """
                    },
                    { "sec-ch-ua-mobile", "?0" },
                    { "sec-ch-ua-platform", "Windows" },
                    { "sec-ch-ua-platform-version", "14.0.0" },
                },
                new ClientHintsResult
                {
                    IsMobile = false,
                    Platform = "Windows",
                    PlatformVersion = "14.0.0",
                    FullVersionList = new Dictionary<string, string>
                    {
                        { "Opera", "83" },
                        { " Not;A Brand", "99" },
                        { "Chromium", "98" },
                    },
                }
            );

        yield return () =>
            (
                new Dictionary<string, string?>
                {
                    {
                        "HTTP_SEC_CH_UA_FULL_VERSION_LIST",
                        """
                        " Not A;Brand";v="99.0.0.0", "Chromium";v="98.0.4758.82", "Opera";v="98.0.4758.82"
                        """
                    },
                    {
                        "HTTP_SEC_CH_UA",
                        """
                        " Not A;Brand";v="99", "Chromium";v="98", "Opera";v="84"
                        """
                    },
                    { "HTTP_SEC_CH_UA_MOBILE", "?1" },
                    { "HTTP_SEC_CH_UA_MODEL", "DN2103" },
                    { "HTTP_SEC_CH_UA_PLATFORM", "Ubuntu" },
                    { "HTTP_SEC_CH_UA_PLATFORM_VERSION", "3.7" },
                    { "HTTP_SEC_CH_UA_FULL_VERSION", "98.0.14335.105" },
                    { "HTTP_SEC_CH_UA_FORM_FACTORS", "\"Desktop\"" },
                },
                new ClientHintsResult
                {
                    IsMobile = true,
                    Platform = "Ubuntu",
                    PlatformVersion = "3.7",
                    FullVersionList = new Dictionary<string, string>
                    {
                        { " Not A;Brand", "99.0.0.0" },
                        { "Chromium", "98.0.4758.82" },
                        { "Opera", "98.0.4758.82" },
                    },
                    Model = "DN2103",
                    FormFactors = ["Desktop"],
                }
            );
    }

    public class ClientHintsResult
    {
        public required bool IsMobile { get; init; }
        public required string? Platform { get; init; }
        public required string? PlatformVersion { get; init; }
        public required Dictionary<string, string> FullVersionList { get; init; }
        public string? Model { get; init; }
        public List<string> FormFactors { get; init; } = [];
    }
}
