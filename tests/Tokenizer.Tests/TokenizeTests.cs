using System;
using System.Linq;
using Compiler;
using Compiler.Lexing;
using FluentAssertions;
using Xunit;

namespace Tokenize.Tests
{
    public class TokenizerTests
    {
        private static Tokenizer CreateDefaultTokenizer()
        {
            return new Tokenizer(TokenizerGrammar.Default);
        }

        public class Constructor
        {
            [Fact]
            public void WhenGrammarIsNullThenShouldThrowArgumentNullException()
            {
                Action act = () => new Tokenizer(null);

                act.ShouldThrow<ArgumentNullException>();
            }
            [Fact]
            public void WhenErrorSinkIsNullThenShouldThrowArgumentNullException()
            {
                Action act = () => new Tokenizer(TokenizerGrammar.Default, null);

                act.ShouldThrow<ArgumentNullException>();
            }
            [Fact]
            public void WhenGrammarAndErrorSinkAreNotNullThenShouldConstructTokenizer()
            {
                var result = new Tokenizer(TokenizerGrammar.Default, new ErrorSink());

                result.Should().NotBeNull();
            }
        }

        public class Tokenize
        {
            [Fact]
            public void WhenSourceFileIsNullThenShouldThrowArgumentNullException()
            {
                Action act = () => CreateDefaultTokenizer().Tokenize((SourceFile)null);

                act.ShouldThrow<ArgumentNullException>();
            }
            [Fact]
            public void WhenSourceTextIsNullThenShouldThrowArgumentNullException()
            {
                Action act = () => CreateDefaultTokenizer().Tokenize((string)null);

                act.ShouldThrow<ArgumentNullException>();
            }
            [Fact]
            public void WhenEmptySourceFileThenShouldReturnEOFToken()
            {
                var result = CreateDefaultTokenizer()
                    .Tokenize(new SourceFile(nameof(WhenEmptySourceFileThenShouldReturnEOFToken), "")).ToList();

                result.Count.Should().Be(1);
                result.First().TokenType.Should().Be(TokenType.EOF);
            }
            [Fact]
            public void WhenEmptyContentThenShouldReturnEOFToken()
            {
                var result = CreateDefaultTokenizer()
                    .Tokenize("").ToList();

                result.Count.Should().Be(1);
                result.First().TokenType.Should().Be(TokenType.EOF);
            }

            public class NewLine
            {
                [Theory]
                [InlineData("\n")]
                public void WhenContentIsNewLineThenShouldReturnNewLineAndEOFTokens(string input)
                {
                    var result = CreateDefaultTokenizer().Tokenize(input).ToList();

                    result.Count.Should().Be(2);
                    result.First().TokenType.Should().Be(TokenType.NewLine);
                    result.Last().TokenType.Should().Be(TokenType.EOF);
                }
            }
            public class WhiteSpace
            {
                [Theory]
                [InlineData(" ")]
                [InlineData("  ")]
                [InlineData("\t")]
                public void WhenContentIsWhiteSpaceThenShouldReturnWhiteSpaceTokenAndEOF(string input)
                {
                    var result = CreateDefaultTokenizer().Tokenize(input).ToList();

                    result.Count.Should().Be(2);
                    result.First().TokenType.Should().Be(TokenType.Whitespace);
                    result.Last().TokenType.Should().Be(TokenType.EOF);
                }
            }
            public class Int
            {
                [Theory]
                [InlineData(0)]
                [InlineData(10)]
                [InlineData(20)]
                [InlineData(int.MaxValue)]
                public void WhenContentsIsPositiveIntThenShouldReturnIntAndEOFToken(int input)
                {
                    var result = CreateDefaultTokenizer().Tokenize(input.ToString()).ToList();

                    result.Count.Should().Be(2);
                    result.First().TokenType.Should().Be(TokenType.IntegerLiteral);
                    result.First().Value.Should().Be(input.ToString());
                    result.Last().TokenType.Should().Be(TokenType.EOF);
                }
                [Theory]
                [InlineData(-1)]
                [InlineData(-10)]
                [InlineData(-20)]
                [InlineData(int.MinValue)]
                public void WhenContentsIsNegativeIntThenShouldReturnMinusIntAndEOFToken(int input)
                {
                    var result = CreateDefaultTokenizer().Tokenize(input.ToString()).ToList();

                    result.Count.Should().Be(3);
                    result.First().TokenType.Should().Be(TokenType.Minus);
                    result[1].TokenType.Should().Be(TokenType.IntegerLiteral);
                    result[1].Value.Should().Be(input.ToString().TrimStart('-'));
                    result.Last().TokenType.Should().Be(TokenType.EOF);
                }

            }
            public class Real
            {
                // TODO(Dan): Write tests for failure cases too.
                [Theory]
                [InlineData("0f")]
                [InlineData("10f")]
                [InlineData("20f")]
                [InlineData("1.0f")]
                public void WhenContentsIsPositiveFloatThenShouldReturnFloatAndEOFToken(string input)
                {
                    var result = CreateDefaultTokenizer().Tokenize(input).ToList();

                    result.Count.Should().Be(2);
                    result.First().TokenType.Should().Be(TokenType.RealLiteral);
                    result.First().Value.Should().Be(input);
                    result.Last().TokenType.Should().Be(TokenType.EOF);
                }
                [Theory]
                [InlineData("-1f")]
                [InlineData("-10f")]
                [InlineData("-20f")]
                [InlineData("-1.0f")]
                public void WhenContentsIsNegativeFloatThenShouldReturnMinusFloatAndEOFToken(string input)
                {
                    var result = CreateDefaultTokenizer().Tokenize(input).ToList();

                    result.Count.Should().Be(3);
                    result.First().TokenType.Should().Be(TokenType.Minus);
                    result[1].TokenType.Should().Be(TokenType.RealLiteral);
                    result[1].Value.Should().Be(input.TrimStart('-'));
                    result.Last().TokenType.Should().Be(TokenType.EOF);
                }
                [Theory]
                [InlineData("0m")]
                [InlineData("10m")]
                [InlineData("20m")]
                [InlineData("1.0m")]
                public void WhenContentsIsPositiveDecimaltThenShouldReturnDecimalAndEOFToken(string input)
                {
                    var result = CreateDefaultTokenizer().Tokenize(input).ToList();

                    result.Count.Should().Be(2);
                    result.First().TokenType.Should().Be(TokenType.RealLiteral);
                    result.First().Value.Should().Be(input);
                    result.Last().TokenType.Should().Be(TokenType.EOF);
                }
                [Theory]
                [InlineData("-0m")]
                [InlineData("-10m")]
                [InlineData("-20m")]
                [InlineData("-1.0m")]
                public void WhenContentsIsNegativeDecimalThenShouldReturnMinusDecimalAndEOFToken(string input)
                {
                    var result = CreateDefaultTokenizer().Tokenize(input).ToList();

                    result.Count.Should().Be(3);
                    result.First().TokenType.Should().Be(TokenType.Minus);
                    result[1].TokenType.Should().Be(TokenType.RealLiteral);
                    result[1].Value.Should().Be(input.TrimStart('-'));
                    result.Last().TokenType.Should().Be(TokenType.EOF);
                }
                [Theory]
                [InlineData("0d")]
                [InlineData("10d")]
                [InlineData("20d")]
                [InlineData("1.0d")]
                public void WhenContentsIsPositiveDoubleThenShouldReturnDoubleAndEOFToken(string input)
                {
                    var result = CreateDefaultTokenizer().Tokenize(input.ToString()).ToList();

                    result.Count.Should().Be(2);
                    result.First().TokenType.Should().Be(TokenType.RealLiteral);
                    result.First().Value.Should().Be(input);
                    result.Last().TokenType.Should().Be(TokenType.EOF);
                }
                [Theory]
                [InlineData("-1d")]
                [InlineData("-10d")]
                [InlineData("-20d")]
                public void WhenContentsIsNegativeDoubleThenShouldReturnMinusDoubleAndEOFToken(string input)
                {
                    var result = CreateDefaultTokenizer().Tokenize(input).ToList();

                    result.Count.Should().Be(3);
                    result.First().TokenType.Should().Be(TokenType.Minus);
                    result[1].TokenType.Should().Be(TokenType.RealLiteral);
                    result[1].Value.Should().Be(input.ToString().TrimStart('-'));
                    result.Last().TokenType.Should().Be(TokenType.EOF);
                }
                [Theory]
                [InlineData("1e10f")]
                [InlineData("2e10f")]
                [InlineData("3e10f")]
                public void WhenContentIsValidPositiveExponentThenShouldReturnRealAndEOFToken(string input)
                {
                    var result = CreateDefaultTokenizer().Tokenize(input).ToList();

                    result.Count.Should().Be(2);
                    result.First().TokenType.Should().Be(TokenType.RealLiteral);
                    result.First().Value.Should().Be(input);
                    result.Last().TokenType.Should().Be(TokenType.EOF);
                }
                [Theory]
                [InlineData("10e10f")]
                [InlineData("20e10f")]
                [InlineData("30e10f")]
                public void WhenContentIsInvalidPositiveExponentThenShouldReturnErrorAndEOFToken(string input)
                {
                    var result = CreateDefaultTokenizer().Tokenize(input).ToList();

                    result.Count.Should().Be(2);
                    result.First().TokenType.Should().Be(TokenType.Error);
                    result.Last().TokenType.Should().Be(TokenType.EOF);
                }

            }
            public class Comment
            {
                [Fact]
                public void WhenContentIsLineCommentThenShouldReturnLineCommentAndEOFToken()
                {
                    var input = "// This is a line comment";

                    var result = CreateDefaultTokenizer().Tokenize(input).ToList();

                    result.Count.Should().Be(2);
                    result.First().TokenType.Should().Be(TokenType.LineComment);
                    result.First().Value.Should().Be(input);
                    result.Last().TokenType.Should().Be(TokenType.EOF);
                }
            }
            public class BlockComment
            {
                // TODO(Dan): Write failure cases
                [Fact]
                public void WhenContentIsBlockCommentThenShouldReturnBlockCommentAndEOFToken()
                {
                    var input = "/* This is a block comment */";

                    var result = CreateDefaultTokenizer().Tokenize(input).ToList();

                    result.Count.Should().Be(2);
                    result.First().TokenType.Should().Be(TokenType.BlockComment);
                    result.First().Value.Should().Be(input);
                    result.Last().TokenType.Should().Be(TokenType.EOF);
                }
                [Fact]
                public void WhenContentIsNestedBlockCommentsThenShouldReturnBlockCommentAndEOFToken()
                {
                    var input = "/* This is a block /* Within another comment /* Within another comment again */ */ comment */";

                    var result = CreateDefaultTokenizer().Tokenize(input).ToList();

                    result.Count.Should().Be(2);
                    result.First().TokenType.Should().Be(TokenType.BlockComment);
                    result.First().Value.Should().Be(input);
                    result.Last().TokenType.Should().Be(TokenType.EOF);
                }
                [Fact]
                public void WhenContentIsUnterminatedBlockCommentThenShouldReturnErrorAndEOFToken()
                {
                    var input = "/* This is an unterminated block comment";

                    var tokenizer = CreateDefaultTokenizer();
                    var result = tokenizer.Tokenize(input).ToList();

                    result.Count.Should().Be(2);
                    tokenizer.ErrorSink.HasErrors.Should().Be(true);
                    tokenizer.ErrorSink.Errors.First().Message.Should().Be("Unterminated block comment");
                    result.First().TokenType.Should().Be(TokenType.Error);
                    result.First().Value.Should().Be(input);
                    result.Last().TokenType.Should().Be(TokenType.EOF);
                }
            }
            public class Identifier
            {
                // TODO(Dan): Write failure cases
                [Fact]
                public void WhenContentContainsAnIdentifierThenShouldReturnIdentifierAndEOFToken()
                {
                    var input = "identifier";

                    var result = CreateDefaultTokenizer().Tokenize(input).ToList();

                    result.Count.Should().Be(2);
                    result.First().TokenType.Should().Be(TokenType.Identifier);
                    result.First().Value.Should().Be(input);
                    result.Last().TokenType.Should().Be(TokenType.EOF);
                }
            }
            public class Keyword
            {
                // TODO(Dan): Write failure cases
                [Fact]
                public void WhenContentContainsKeywordThenShouldReturnKeywordAndEOFToken()
                {
                    var input = TokenizerGrammar.Default.Keywords.First().Value;

                    var result = CreateDefaultTokenizer().Tokenize(input).ToList();

                    result.Count.Should().Be(2);
                    result.First().TokenType.Should().Be(TokenType.Keyword);
                    result.First().Value.Should().Be(input);
                    result.Last().TokenType.Should().Be(TokenType.EOF);
                }
            }
            public class CharLiteral
            {
                // TODO(Dan): Write failure cases
                [Fact]
                public void WhenContentContainsCharLiteralThenShouldReturnCharLiteralAndEOFToken()
                {
                    var input = "'a'";

                    var result = CreateDefaultTokenizer().Tokenize(input).ToList();

                    result.Count.Should().Be(2);
                    result.First().TokenType.Should().Be(TokenType.CharLiteral);
                    result.First().Value.Should().Be(input.Trim('\''));
                    result.Last().TokenType.Should().Be(TokenType.EOF);
                }
            }
            public class StringLiteral
            {
                // TODO(Dan): Write failure cases
                [Fact]
                public void WhenContentContainsStringLiteralThenShouldReturnStringLiteralAndEOFToken()
                {
                    var input = "\"string literal\"";

                    var result = CreateDefaultTokenizer().Tokenize(input).ToList();

                    result.Count.Should().Be(2);
                    result.First().TokenType.Should().Be(TokenType.StringLiteral);
                    result.First().Value.Should().Be(input.Trim('"'));
                    result.Last().TokenType.Should().Be(TokenType.EOF);
                }
            }
            public class Punctuation
            {
                [Theory]
                [InlineData("<")]
                [InlineData(">")]
                [InlineData("{")]
                [InlineData("}")]
                [InlineData("(")]
                [InlineData(")")]
                [InlineData("[")]
                [InlineData("]")]
                [InlineData("!")]
                [InlineData("%")]
                [InlineData("^")]
                [InlineData("&")]
                [InlineData("*")]
                [InlineData("+")]
                [InlineData("-")]
                [InlineData("=")]
                [InlineData("/")]
                [InlineData(".")]
                [InlineData(",")]
                [InlineData("?")]
                [InlineData(";")]
                [InlineData(":")]
                [InlineData("|")]
                public void WhenContentIsValidPuncuationThenShouldReturnMatchingTokenTypeAndEOF(string input)
                {
                    var result = CreateDefaultTokenizer().Tokenize(input).ToList();

                    result.Count.Should().Be(2);
                    result.First().TokenType.Should().Be(GetMatchingTokenType(input));
                    result.First().Value.Should().Be(input);
                    result.Last().TokenType.Should().Be(TokenType.EOF);
                }
                [Theory]
                [InlineData("`")]
                public void WhenContentIsInvalidPuncuationThenShouldReturnErrorAndEOF(string input)
                {
                    var result = CreateDefaultTokenizer().Tokenize(input).ToList();

                    result.Count.Should().Be(2);
                    result.First().TokenType.Should().Be(TokenType.Error);
                    result.Last().TokenType.Should().Be(TokenType.EOF);
                }

                private TokenType GetMatchingTokenType(string input)
                {
                    switch (input)
                    {
                        case "<":
                            return TokenType.LessThan;
                        case ">":
                            return TokenType.GreaterThan;
                        case "{":
                            return TokenType.LeftBracket;
                        case "}":
                            return TokenType.RightBracket;
                        case "(":
                            return TokenType.LeftParanthesis;
                        case ")":
                            return TokenType.RightParenthesis;
                        case "[":
                            return TokenType.LeftBrace;
                        case "]":
                            return TokenType.RightBrace;
                        case "!":
                            return TokenType.Not;
                        case "%":
                            return TokenType.Mod;
                        case "^":
                            return TokenType.BitwiseXor;
                        case "&":
                            return TokenType.BitwiseAnd;
                        case "*":
                            return TokenType.Mul;
                        case "+":
                            return TokenType.Plus;
                        case "-":
                            return TokenType.Minus;
                        case "=":
                            return TokenType.Assignment;
                        case "/":
                            return TokenType.Div;
                        case ".":
                            return TokenType.Dot;
                        case ",":
                            return TokenType.Comma;
                        case "?":
                            return TokenType.Question;
                        case ";":
                            return TokenType.Semicolon;
                        case ":":
                            return TokenType.Colon;
                        case "|":
                            return TokenType.BitwiseOr;
                        default:
                            throw new Exception("Matching punctuation token type not found");
                    }
                }
            }
        }
    }
}
