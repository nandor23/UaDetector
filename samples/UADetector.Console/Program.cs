// See https://aka.ms/new-console-template for more information

using System.Collections.Frozen;

using UADetector.Parsers;
using UADetector.Regexes.Models;
using UADetector.Utils;

Console.WriteLine("Hello, World!");

var result = ParserExtensions.LoadRegexes<OperatingSystemRegex>("Regexes.operating_systems.yml");

var a = new FrozenBiDictionary<string, string>(new Dictionary<string, string> { });


Console.WriteLine(result);
