using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Compiler;
using Compiler.Parsing;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.lang").Select(f => new SourceFile(f, File.ReadAllText(f)));

            var parser = new SyntaxParser();

            var stopwatch = new Stopwatch();

            stopwatch.Start();

            var compilationUnit = parser.Parse(files.First());

            stopwatch.Stop();

            Console.WriteLine($"Parser took {stopwatch.ElapsedMilliseconds}ms to generate AST from {files.Sum(f => f.Lines.Count())} lines");

            Console.WriteLine();
            Console.WriteLine();

            if (parser.ErrorSink.HasErrors)
            {
                Console.WriteLine("----------- ERRORS: -----------");

                foreach (var error in parser.ErrorSink.Where(x => x.Severity == Severity.Error))
                    Console.WriteLine($"Message: {error.Message}. Location: (Start LineNo) {error.FilePart.Start.LineNumber} (Start Col) {error.FilePart.Start.Column}, (End LineNo) {error.FilePart.End.LineNumber}, (End Col) {error.FilePart.End.Column}. Value: {string.Join(" ", error.FilePart.Lines ?? new[] { string.Empty })}");

                Console.WriteLine();
                Console.WriteLine("---------- WARNINGS: ----------");
                Console.WriteLine();

                foreach (var error in parser.ErrorSink.Where(x => x.Severity == Severity.Warning))
                    Console.WriteLine($"Message: {error.Message}. Location: (Start LineNo) {error.FilePart.Start.LineNumber} (Start Col) {error.FilePart.Start.Column}, (End LineNo) {error.FilePart.End.LineNumber}, (End Col) {error.FilePart.End.Column}. Value: {string.Join(" ", error.FilePart.Lines ?? new[] { string.Empty })}");

                Console.WriteLine();
                Console.WriteLine("------------ INFO: ------------");
                Console.WriteLine();

                foreach (var error in parser.ErrorSink.Where(x => x.Severity == Severity.Message))
                    Console.WriteLine($"Message: {error.Message}. Location: (Start LineNo) {error.FilePart.Start.LineNumber} (Start Col) {error.FilePart.Start.Column}, (End LineNo) {error.FilePart.End.LineNumber}, (End Col) {error.FilePart.End.Column}. Value: {string.Join(" ", error.FilePart.Lines ?? new[] { string.Empty })}");

                Console.WriteLine();
            }

            Console.WriteLine();
        }
    }
}
