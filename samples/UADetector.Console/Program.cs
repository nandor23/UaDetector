// See https://aka.ms/new-console-template for more information

using UADetector.Parsers;
using UADetector.Regexes.Models;

Console.WriteLine("Hello, World!");

var result = ParserExtensions.LoadRegexes<OsRegex>("Regexes.Resources.operating_systems.yml");


Console.WriteLine(result);
