using System;
using System.Linq;
using Compiler;
using Compiler.Parsing;
using FluentAssertions;
using Xunit;

namespace Parser.Tests
{
    public class Parse
    {
        private static SyntaxParser CreateDefaultParser() => new SyntaxParser();
        private static SourceFile CreateSourceFile(string name, string contents) => new SourceFile(name + ".lang", contents);

        [Fact]
        public void WhenSourceFileIsNullThenShouldThrowArgumentNullException()
        {
            Action act = () =>
            {
                var parser = CreateDefaultParser();

                parser.Parse(sourceFile: null);
            };

            act.ShouldThrow<ArgumentNullException>();
        }

        public class ImportStatement
        {
            [Fact]
            public void WhenNoImportStatementsThenShouldNotThrowSyntaxException()
            {
                Action act = () =>
                {
                    var parser = CreateDefaultParser();
                    var file = CreateSourceFile(nameof(WhenNoImportStatementsThenShouldNotThrowSyntaxException), "");

                    var result = parser.Parse(file);
                };

                act.ShouldNotThrow<SyntaxException>();
            }
            [Fact]
            public void WhenNoImportStatementsThenCompilationUnitShouldContainNoImports()
            {
                var parser = CreateDefaultParser();
                var file = CreateSourceFile(nameof(WhenNoImportStatementsThenShouldNotThrowSyntaxException), "");

                var result = parser.Parse(file);

                result.CompilationUnits.Should().HaveCount(1);

                result.CompilationUnits.First().Imports.Any().Should().Be(false);
            }
            [Fact]
            public void WhenImportStatementProvidedThenCompilationUnitShouldContainImport()
            {
                var parser = CreateDefaultParser();
                var file = CreateSourceFile(nameof(WhenNoImportStatementsThenShouldNotThrowSyntaxException), "import SomeOtherModule;");

                var result = parser.Parse(file);

                result.CompilationUnits.Should().HaveCount(1);

                result.CompilationUnits.First().Imports.Count().Should().Be(1);
                result.CompilationUnits.First().Imports.First().Name.Should().Be("SomeOtherModule");
            }
            [Fact]
            public void WhenSuppliedWithMultipleImportsThenCompilationUnitShouldContainMatchingNumberOfImports()
            {
                var parser = CreateDefaultParser();
                var file = CreateSourceFile(nameof(WhenNoImportStatementsThenShouldNotThrowSyntaxException), "import SomeOtherModule; import AnotherModule;");

                var result = parser.Parse(file);

                result.CompilationUnits.Should().HaveCount(1);

                result.CompilationUnits.First().Imports.Count().Should().Be(2);
                result.CompilationUnits.First().Imports.Last().Name.Should().Be("AnotherModule");
            }
            [Fact]
            public void WhenSuppliedWithImportStatementContainingDotsThenShouldParseSuccessfully()
            {
                var parser = CreateDefaultParser();
                var file = CreateSourceFile(nameof(WhenNoImportStatementsThenShouldNotThrowSyntaxException), "import SomeOtherModule.SubModule;");

                var result = parser.Parse(file);

                result.CompilationUnits.Should().HaveCount(1);

                result.CompilationUnits.First().Imports.Count().Should().Be(1);
                result.CompilationUnits.First().Imports.First().Name.Should().Be("SomeOtherModule.SubModule");
            }
            [Fact]
            public void WhenImportStatementNotAtStartOfFileThenShouldContainTopLevelStatementError()
            {
                var parser = CreateDefaultParser();
                var file = CreateSourceFile(nameof(WhenImportStatementNotAtStartOfFileThenShouldContainTopLevelStatementError), "module MyModule {} import SomeOtherModule.SubModule;");

                var result = parser.Parse(file);

                parser.ErrorSink.HasErrors.Should().Be(true);
                parser.ErrorSink.Errors.First().Message.Should().Be("Top-level statements are not permitted. Statements must be part of a module with the exception of import statements which are at the start of the file");
                parser.ErrorSink.Errors.First().Severity.Should().Be(Severity.Error);
            }
        }

        public class ModuleDeclaration
        {
            [Fact]
            public void WhenSuppliedWithNoModuleStatementThenShouldNotThrow()
            {
                Action act = () =>
                {
                    var parser = CreateDefaultParser();
                    var file = CreateSourceFile(nameof(WhenSuppliedWithNoModuleStatementThenShouldNotThrow), "import SomeModule;");

                    var result = parser.Parse(file);
                };

                act.ShouldNotThrow<SyntaxException>();
            }
        }
    }
}
