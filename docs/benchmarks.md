Both UaDetector and UaDetector.Lite load regular expressions into memory for parsing.
If memory usage is a concern, UaDetector.Lite uses **5.6 times less memory** (32.38 MB vs 181.88 MB)
than UaDetector while maintaining the same functionality at the cost of parsing speed.

### Library Comparison

| Method             | Mean     | Error     | StdDev    | Ratio | Allocated   | Alloc Ratio |
|--------------------|---------:|----------:|----------:|------:|------------:|------------:|
| UaDetector         | 3.175 ms | 0.5996 ms | 1.6210 ms |  1.00 |     2.65 KB |        1.00 |
| UAParser           | 6.753 ms | 0.1203 ms | 0.1125 ms |  2.13 | 11293.44 KB |    4,264.19 |
| UaDetector.Lite    | 7.201 ms | 0.1412 ms | 0.2651 ms |  2.27 |     4.13 KB |        1.56 |
| DeviceDetector.NET | 8.158 ms | 0.7390 ms | 2.0230 ms |  2.57 |  8621.18 KB |    3,255.19 |

### Individual Parser Performance

#### UaDetector

| Method                 | Mean       | Error     | StdDev      | Allocated |
|----------------------- |-----------:|----------:|------------:|----------:|
| UaDetector_TryParse    | 3,193.1 us | 600.57 us | 1,623.68 us |    2712 B |
| OsParser_TryParse      |   567.6 us |   6.42 us |     6.00 us |    1285 B |
| BrowserParser_TryParse | 1,576.8 us | 303.18 us |   884.40 us |     648 B |
| ClientParser_TryParse  |   635.9 us |  11.34 us |    10.61 us |    1048 B |
| BotParser_TryParse     |   340.5 us |   4.43 us |     4.14 us |     276 B |
| BotParser_IsBot        |   331.0 us |   1.08 us |     1.01 us |     218 B |

#### UaDetector.Lite

| Method                 | Mean       | Error     | StdDev    | Allocated |
|----------------------- |-----------:|----------:|----------:|----------:|
| UaDetector_TryParse    | 7,232.5 us | 142.64 us | 294.58 us |    4172 B |
| OsParser_TryParse      | 1,198.8 us |   9.20 us |   8.16 us |    1287 B |
| BrowserParser_TryParse | 2,109.7 us |  33.37 us |  31.22 us |    1910 B |
| ClientParser_TryParse  |   461.6 us |   2.02 us |   1.89 us |    1049 B |
| BotParser_TryParse     |   284.4 us |   1.01 us |   0.94 us |     276 B |
| BotParser_IsBot        |   269.8 us |   4.11 us |   3.84 us |     218 B |