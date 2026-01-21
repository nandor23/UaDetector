## Benchmarks

Both UaDetector and UaDetector.Lite load regular expressions into memory for parsing.
If memory usage is a concern, UaDetector.Lite uses **5.6 times less memory** (32.38 MB MB vs 181.88 MB)
than UaDetector while maintaining the same functionality at the cost of parsing speed.

### Library Comparison

| Method          | Mean     | Error     | StdDev    | Ratio |   Allocated | Alloc Ratio |
|-----------------|---------:|----------:|----------:|------:|------------:|------------:|
| UaDetector      | 1.584 ms | 0.0156 ms | 0.0138 ms |  1.00 |     4.04 KB |        1.00 |
| UaDetector.Lite | 4.698 ms | 0.0451 ms | 0.0422 ms |  2.97 |     4.04 KB |        1.00 |
| DeviceDetector  | 5.558 ms | 0.0265 ms | 0.0207 ms |  3.51 |  4597.12 KB |    1,138.17 |
| UAParser        | 6.692 ms | 0.1093 ms | 0.1022 ms |  4.22 | 10919.42 KB |    2,703.45 |

### Individual Parser Performance

#### UaDetector

| Method                 | Mean       | Error    | StdDev   | Allocated |
|----------------------- |-----------:|---------:|---------:|----------:|
| UaDetector_TryParse    | 1,565.6 us | 14.01 us | 13.10 us |    4136 B |
| OsParser_TryParse      |   542.3 us |  6.15 us |  5.75 us |    1520 B |
| BrowserParser_TryParse | 1,104.9 us | 12.45 us | 11.64 us |    1752 B |
| ClientParser_TryParse  |   153.7 us |  2.25 us |  2.11 us |    1264 B |
| BotParser_TryParse     |   321.8 us |  6.15 us |  7.78 us |     576 B |
| BotParser_IsBot        |   318.7 us |  3.67 us |  3.43 us |     256 B |

#### UaDetector.Lite

| Method                 | Mean       | Error    | StdDev   | Allocated |
|----------------------- |-----------:|---------:|---------:|----------:|
| UaDetector_TryParse    | 4,556.6 us | 63.54 us | 59.43 us |    4138 B |
| OsParser_TryParse      | 1,630.2 us | 15.86 us | 14.84 us |    1520 B |
| BrowserParser_TryParse | 2,606.9 us | 26.80 us | 25.07 us |    1752 B |
| ClientParser_TryParse  |   242.2 us |  2.37 us |  2.22 us |    1264 B |
| BotParser_TryParse     |   293.0 us |  3.72 us |  3.48 us |     576 B |
| BotParser_IsBot        |   263.2 us |  3.31 us |  3.09 us |     256 B |