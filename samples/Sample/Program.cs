using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Compiler;
using Compiler.Parsing;
using Compiler.Semantics;

namespace Sample
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var startMemory = GC.GetTotalMemory(true);
            var afterParser = 0L;
            var entryAssemblyLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var currentDirectory = Directory.GetCurrentDirectory();

            var files = Directory.GetFiles(currentDirectory, "*.lang")
                .Concat(Directory.GetFiles(Path.Combine(entryAssemblyLocation, "StandardLibrary"), "*.lang", SearchOption.AllDirectories))
                .Select(f => new SourceFile(f, File.ReadAllText(f)));

            var parser = new LanguageParser();
            var sematicAnalyzer = new SematicAnalyzer(parser.ErrorSink);

            var stopwatch = new Stopwatch();
            var totalTimeStopwatch = new Stopwatch();

            var buffer = new StringBuilder();

            try
            {
                stopwatch.Start();

                var compilationRoot = await parser.ParseAsync(files);

                stopwatch.Stop();

                buffer.AppendLine();
                buffer.AppendLine($"Parser took {stopwatch.ElapsedMilliseconds / 1000.0}s to generate AST from {files.Sum(f => f.Lines.Count())} lines");

                afterParser = GC.GetTotalMemory(false);

                buffer.AppendLine($"Memory used: {(afterParser - startMemory) / 1000}Kb");

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

                    buffer.AppendLine();
                    buffer.AppendLine($"Semantic passes took {(stopwatch.ElapsedMilliseconds - start) / 1000.0}s");

                }

                var afterSemantics = GC.GetTotalMemory(false);

                buffer.AppendLine();
                buffer.AppendLine($"Memory used: {(afterSemantics - afterParser) / 1000}Kb");
                buffer.AppendLine();
            }
            catch (Exception ex)
            {
                buffer.AppendLine();
                buffer.AppendLine("----------- INTERNAL COMPILER ERROR -----------");
                buffer.AppendLine();
                buffer.AppendLine(ex.Message);
                buffer.AppendLine();
                buffer.AppendLine(ex.StackTrace);
                buffer.AppendLine();
                buffer.AppendLine("-----------------------------------------------");
            }
            finally
            {
                var endMemory = GC.GetTotalMemory(false);

                buffer.AppendLine($"Finished in {stopwatch.ElapsedMilliseconds}ms");

                buffer.AppendLine();

                buffer.AppendLine($"Memory used: {(endMemory - startMemory) / 1000}Kb");

                buffer.AppendLine();
                buffer.AppendLine();

                Console.Write(buffer);

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
            var colour = error.Severity == Severity.Error ? ConsoleColor.DarkRed : error.Severity == Severity.Warning ? ConsoleColor.DarkYellow : ConsoleColor.DarkBlue;
            var foreground = error.Severity == Severity.Error ? ConsoleColor.Red : error.Severity == Severity.Warning ? ConsoleColor.Yellow : ConsoleColor.Blue;

            Console.BackgroundColor = colour;
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
            Console.WriteLine($" {error.FilePart.FilePath}");

            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Write($"{string.Join("", Enumerable.Range(0, padding - 2).Select(x => " "))} Line No:{string.Join("", Enumerable.Range(0, padding - 2).Select(x => " "))}");
            
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine($" {error.FilePart.Start.LineNumber}");
            Console.WriteLine();

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;

            var buffer = new StringBuilder(error.FilePart.Lines.First());
            var start = error.FilePart.Start.Column - 1;

            var index = 0;

            while (buffer.Length > 0 && buffer[index] == '\t')
            {
                buffer[index] = ' ';
                index++;
            }

            var line = buffer.ToString();
            var length = (error.FilePart.End.Column - error.FilePart.Start.Column).Min(1);

            var spaces = string.Join("", Enumerable.Range(0, start).Select(x => " "));
            var upArrows = string.Join("", Enumerable.Range(0, length.Min(1)).Select(x => "^"));

            Console.WriteLine($"{" ", 5} | ");
            Console.Write($"{error.FilePart.Start.LineNumber,5} | ");

            for (int i = 0; i < line.Length; i++)
            {
                var @char = line[i];

                if (i >= spaces.Length && i < spaces.Length + upArrows.Length)
                {
                    Console.ForegroundColor = foreground;
                    Console.Write(@char);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(@char);
                }
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            Console.Write($"{"", 5} | {spaces}");

            Console.ForegroundColor = foreground;
            Console.WriteLine($"{upArrows}");
            Console.ForegroundColor = ConsoleColor.Gray;

            foreach (var l in error.Lines.Skip(1))
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
