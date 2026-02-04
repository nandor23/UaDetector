Both UaDetector and UaDetector.Lite load regular expressions into memory for parsing.
If memory usage is a concern, UaDetector.Lite uses **5.6 times less memory** (32.38 MB vs 181.88 MB)
than UaDetector while maintaining the same functionality at the cost of parsing speed.

### Library Comparison

| Method             | Mean     | Error     | StdDev    | Ratio | Allocated   | Alloc Ratio |
|------------------- |---------:|----------:|----------:|------:|------------:|------------:|
| UaDetector         | 2.635 ms | 0.2718 ms | 0.8014 ms |  1.00 |      2.9 KB |        1.00 |
| UaDetector.Lite    | 7.136 ms | 0.1345 ms | 0.1192 ms |  2.71 |     2.83 KB |        0.98 |
| UAParser           | 6.791 ms | 0.0842 ms | 0.0746 ms |  2.58 | 11309.33 KB |    3,900.46 |
| DeviceDetector.NET | 8.588 ms | 0.7745 ms | 2.1203 ms |  3.26 |  8621.18 KB |    2,972.82 |

### Individual Parser Performance

#### UaDetector

| Method                 | Mean       | Error     | StdDev    | Allocated |
|----------------------- |-----------:|----------:|----------:|----------:|
| UaDetector_TryParse    | 2,649.4 us | 271.72 us | 801.18 us |    2966 B |
| OsParser_TryParse      |   569.5 us |   2.20 us |   2.06 us |    1286 B |
| BrowserParser_TryParse | 1,046.6 us |   4.51 us |   4.22 us |    1895 B |
| ClientParser_TryParse  |   638.8 us |   4.97 us |   4.65 us |    1048 B |
| BotParser_TryParse     |   339.8 us |   4.03 us |   3.77 us |     276 B |
| BotParser_IsBot        |   333.8 us |   1.16 us |   1.08 us |     218 B |

#### UaDetector.Lite

| Method                 | Mean       | Error     | StdDev    | Allocated |
|----------------------- |-----------:|----------:|----------:|----------:|
| UaDetector_TryParse    | 7,196.0 us | 140.41 us | 156.07 us |    2900 B |
| OsParser_TryParse      | 1,195.6 us |  11.64 us |  10.89 us |    1287 B |
| BrowserParser_TryParse | 2,117.9 us |  35.92 us |  33.60 us |    1898 B |
| ClientParser_TryParse  |   462.3 us |   2.27 us |   2.12 us |    1049 B |
| BotParser_TryParse     |   275.6 us |   3.26 us |   3.05 us |     276 B |
| BotParser_IsBot        |   265.3 us |   0.65 us |   0.54 us |     218 B |