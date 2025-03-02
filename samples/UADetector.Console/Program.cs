// See https://aka.ms/new-console-template for more information
using UADetector.Parsers;
using UADetector.Regexes.Models;

Console.WriteLine("Hello, World!");

var result = ParserExtensions.LoadRegexes<Os>("Regexes.Resources.operating_systems.yml", RegexPatternType.UserAgent);

Console.WriteLine(result.First());
