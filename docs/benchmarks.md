Both UaDetector and UaDetector.Lite load regular expressions into memory for parsing.
If memory usage is a concern, UaDetector.Lite uses **5.6 times less memory** (32.38 MB vs 181.88 MB)
than UaDetector while maintaining the same functionality at the cost of parsing speed.

> [!NOTE]
> UAParser is faster because it matches against a smaller set of regular expressions. UaDetector
> uses the larger [Matomo Device Detector](https://github.com/matomo-org/device-detector) ruleset,
> trading some speed for broader and more precise detection.
> Detection rules: [Device Detector](https://github.com/matomo-org/device-detector/tree/master/regexes) (used by UaDetector) · [uap-core](https://github.com/ua-parser/uap-core/blob/master/regexes.yaml) (used by UAParser)

### Library Comparison

| Method             | Mean       | Error     | StdDev    | Ratio |  Allocated | Alloc Ratio |
|--------------------|-----------:|----------:|----------:|------:|-----------:|------------:|
| UAParser           |   345.9 us |   6.66 us |   7.13 us |  0.13 |   36.17 KB |       18.07 |
| UaDetector         | 2,635.1 us |  51.19 us |  50.27 us |  1.00 |       2 KB |        1.00 |
| UaDetector.Lite    | 7,795.6 us | 154.42 us | 231.12 us |  2.96 |       2 KB |        1.00 |
| DeviceDetector.NET | 8,009.2 us | 153.57 us | 176.85 us |  3.04 | 5883.39 KB |    2,940.26 |

### Individual Parser Performance

#### UaDetector

| Method                 | Mean       | Error     | StdDev    | Allocated |
|----------------------- |-----------:|----------:|----------:|----------:|
| UaDetector_TryParse    | 2,868.7 us | 299.67 us | 883.59 us |    2174 B |
| OsParser_TryParse      |   609.2 us |  11.85 us |  11.64 us |     583 B |
| BrowserParser_TryParse | 1,106.3 us |  21.43 us |  23.82 us |    1218 B |
| ClientParser_TryParse  |   672.5 us |  13.45 us |  17.00 us |     477 B |
| BotParser_TryParse     |   369.0 us |   7.24 us |   9.66 us |      51 B |
| BotParser_IsBot        |   362.0 us |   7.00 us |   7.19 us |         - |

#### UaDetector.Lite

| Method                 | Mean       | Error     | StdDev    | Allocated |
|----------------------- |-----------:|----------:|----------:|----------:|
| UaDetector_TryParse    | 7,667.4 us | 135.81 us | 127.04 us |    2031 B |
| OsParser_TryParse      | 1,302.6 us |  24.23 us |  24.88 us |     582 B |
| BrowserParser_TryParse | 2,261.4 us |  42.84 us |  49.34 us |    1208 B |
| ClientParser_TryParse  |   490.4 us |   9.34 us |   9.18 us |     478 B |
| BotParser_TryParse     |   297.8 us |   3.66 us |   3.06 us |      51 B |
| BotParser_IsBot        |   282.9 us |   3.83 us |   3.39 us |         - |