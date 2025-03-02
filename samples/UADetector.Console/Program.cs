// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;

using UADetector.Parsers;
using UADetector.Regexes.Models;

Console.WriteLine("Hello, World!");

var result = ParserExtensions.LoadRegexes<Os>("Regexes.Resources.oss.yml", RegexPatternType.UserAgent);

Console.WriteLine(result.First());
