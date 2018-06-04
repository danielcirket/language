using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Compiler;
using Compiler.Parsing;
using Compiler.Semantics;
using Newtonsoft.Json;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var startMemory = GC.GetTotalMemory(true);
            var afterParser = 0L;
            var entryAssemblyLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var currentDirectory = Directory.GetCurrentDirectory();

            var files = Directory.GetFiles(currentDirectory, "*.lang")
                .Concat(Directory.GetFiles(Path.Combine(entryAssemblyLocation, "StandardLibrary"), "*.lang", SearchOption.AllDirectories))
                .Select(f => new SourceFile(f, File.ReadAllText(f)));

            var parser = new SyntaxParser();
            var sematicAnalyzer = new SematicAnalyzer(parser.ErrorSink);

            var stopwatch = new Stopwatch();
            var totalTimeStopwatch = new Stopwatch();

            try
            {
                stopwatch.Start();

                var compilationRoot = parser.Parse(files);

                stopwatch.Stop();

                Console.WriteLine($"Parser took {stopwatch.ElapsedMilliseconds}ms to generate AST from {files.Sum(f => f.Lines.Count())} lines");

                afterParser = GC.GetTotalMemory(false);

                Console.WriteLine($"Memory used: {(afterParser - startMemory) / 1000}Kb");

                stopwatch.Start();

                if (compilationRoot != null)
                {
                    var start = stopwatch.ElapsedMilliseconds;

                    //try
                    //{
                    sematicAnalyzer.Analyze(compilationRoot);
                    //}
                    //catch (Exception ex) when (!(ex is NotImplementedException))
                    //{
                    //
                    //}

                    stopwatch.Stop();

                    Console.WriteLine($"Semantic passes took {stopwatch.ElapsedMilliseconds - start}ms");

                }

                var afterSemantics = GC.GetTotalMemory(false);

                Console.WriteLine($"Memory used: {(afterSemantics - afterParser) / 1000}Kb");

                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("----------- INTERNAL COMPILER ERROR -----------");
                Console.WriteLine();
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                Console.WriteLine("-----------------------------------------------");
            }
            finally
            {
                var endMemory = GC.GetTotalMemory(false);

                Console.WriteLine($"Finished in {stopwatch.ElapsedMilliseconds}ms");

                Console.WriteLine();

                Console.WriteLine($"Memory used: {(endMemory - startMemory) / 1000}Kb");

                Console.WriteLine();
                Console.WriteLine();

                if (parser.ErrorSink.Any())
                {
                    if (parser.ErrorSink.HasErrors)
                    {
                        //Console.WriteLine("----------- ERRORS: -----------");

                        foreach (var error in parser.ErrorSink.Where(x => x.Severity == Severity.Error))
                            PrettyPrintError(error);

                        //Console.WriteLine();
                    }
                    if (parser.ErrorSink.HasWarnings)
                    {
                        //Console.WriteLine("---------- WARNINGS: ----------");
                        //Console.WriteLine();

                        foreach (var error in parser.ErrorSink.Where(x => x.Severity == Severity.Warning))
                            PrettyPrintError(error);

                        //Console.WriteLine();
                    }
                    if (parser.ErrorSink.HasMessage)
                    {
                        Console.WriteLine("------------ INFO: ------------");
                        Console.WriteLine();

                        foreach (var error in parser.ErrorSink.Where(x => x.Severity == Severity.Message))
                            PrettyPrintError(error);

                        //Console.WriteLine();
                    }
                }
            }

            Console.ReadLine();
        }

        static void PrettyPrintError(Error error)
        {
            // TODO(Dan): Handle errors spanning multiple lines better!
            Console.BackgroundColor = error.Severity == Severity.Error ? ConsoleColor.DarkRed : error.Severity == Severity.Warning ? ConsoleColor.DarkYellow : ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;

            var level = $"  {error.Severity.ToString()}: ";
            
            Console.Write(level);

            Console.BackgroundColor = ConsoleColor.Black;

            Console.WriteLine($" {error.Message}");

            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;

            var fileLabel = level.Length - 4;
            var padding = fileLabel / 2;

            Console.Write($"{string.Join("", Enumerable.Range(0, padding).Select(x => " "))}File:{string.Join("", Enumerable.Range(0, padding).Select(x => " "))}");

            Console.BackgroundColor = ConsoleColor.Black;

            

            Console.WriteLine($" {error.FilePart.FilePath}:{error.FilePart.Start.LineNumber}.");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Gray;

            var line = error.FilePart.Lines.First();

            var trimmedLine = line.TrimStart();
            var start = error.FilePart.Start.Column - 1;

            var index = 0;

            while (line.Length > 0 && line[index] == '\t')
            {
                start -= 1;

                index++;
            }

            //if (line.Length != trimmedLine.Length)
            //    start = start - (line.Length - trimmedLine.Length);

            var end = error.FilePart.Start.Column - (line.Length - trimmedLine.Length);
                //? error.FilePart.Start.Column - (line.Length - trimmedLine.Length)
                //: error.FilePart.Start.Column - (line.Length - trimmedLine.Length);

            var length = (end - start).Min(0);

            var skip = string.Join("", Enumerable.Range(0, start.Min(0)).Select((x, i) => "-"));
            var dashes = string.Join("", Enumerable.Range(0, line.Length != trimmedLine.Length ? 0 : length).Select(x => "-"));
            var upArrows = string.Join("", Enumerable.Range(0, (length).Min(1)).Select(x => "^"));

            var arrow = skip + dashes;

            Console.WriteLine($"{error.FilePart.Start.LineNumber, 5} | {line.TrimStart()}");
            Console.WriteLine($"{"", 5} | {arrow}{upArrows}");

            foreach(var l in error.Lines.Skip(1))
                Console.WriteLine($"    | {l.TrimStart()}");

            Console.WriteLine();
            Console.WriteLine();
        }
    }

    public class Test<TClass>
    {
        public TClass Property { get; set; }

        public T Something<T>(T a)
        {
            return default;
        }

        //public Test(int value)
        //{
        //    Property = value;
        //}
    }

    //public class Array<T>
    //{
    //    public int? IndexOf<T>(T value)
    //    {
    //        return null;
    //    }
    //}

    public interface Interface<T>
    {
        T Something<T>(T value);
    }

}
