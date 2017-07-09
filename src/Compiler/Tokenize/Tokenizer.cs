using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler.Tokenize
{
    internal class Tokenizer
    {
        private int _column;
        private int _index;
        private int _line;
        private StringBuilder _builder;
        private SourceFile _sourceFile;
        private SourceFileLocation _sourceFileLocation;
        private readonly TokenizerGrammar _grammar;
        private readonly ErrorSink _errorSink;
        private readonly IEnumerable<string> _keywords;

        // TODO(Dan): Possibly add some bounds checks here
        private char Current => _sourceFile.Contents[_index];
        private char Next => _sourceFile.Contents[_index + 1];
        public ErrorSink ErrorSink => _errorSink;

        public IEnumerable<Token> Tokenize(string content)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            return Tokenize(new SourceFile("n/a", content));
        }
        public IEnumerable<Token> Tokenize(SourceFile sourceFile)
        {
            if (sourceFile == null)
                throw new ArgumentNullException(nameof(sourceFile));

            _sourceFile = sourceFile;

            // Urgh! Why am I bothering?!
            return TokenizeInternal();
        }

        private IEnumerable<Token> TokenizeInternal()
        {
            _builder.Clear();
            _line = 1;
            _index = 0;
            _column = 1;
            _sourceFileLocation = new SourceFileLocation(_column, _index, _line);

            while (!IsEOF())
                yield return ParseNextToken();

            yield return CreateToken(TokenType.EOF);
        }
        private Token ParseNextToken()
        {
            if (IsEOF())
            {
                return CreateToken(TokenType.EOF);
            }
            else if (Current.IsNewLine())
            {
                return NewLine();
            }
            else if (Current.IsWhiteSpace())
            {
                return WhiteSpace();
            }
            else if (Current.IsDigit())
            {
                return Int();
            }
            else if (Current == '/' && (Peek(1) == '/' || Peek(1) == '*'))
            {
                return Comment();
            }
            else if (Current.IsLetter() || Current == '_')
            {
                return Identifier();
            }
            else if (Current == '\'')
            {
                return CharLiteral();
            }
            else if (Current == '"')
            {
                return StringLiteral();
            }
            else if (Current.IsPunctuation())
            {
                return Punctuation();
            }
            else
            {
                return Error();
            }
        }
        private Token CreateToken(TokenType tokenType)
        {
            var content = _builder.ToString();
            var start = _sourceFileLocation;
            var end = new SourceFileLocation(_column, _index, _line);

            _sourceFileLocation = end;

            _builder.Clear();

            return new Token(tokenType, content, start, end);
        }
        private Token Error(Severity severity = Severity.Error, string message = "Unexpected token '{0}'")
        {
            while (!IsEOF() && !Current.IsWhiteSpace() && !Current.IsPunctuation())
                Consume();

            AddError(string.Format(message, severity), severity);

            return CreateToken(TokenType.Error);
        }
        private Token NewLine()
        {
            Consume();

            _line++;
            _column = 1;

            return CreateToken(TokenType.NewLine);
        }
        private Token WhiteSpace()
        {
            while (!IsEOF() && Current.IsWhiteSpace())
                Consume();

            return CreateToken(TokenType.Whitespace);
        }
        private Token Int()
        {
            while (!IsEOF() && Current.IsDigit())
                Consume();

            if (!IsEOF() && (Current == 'f' || Current == 'F' || Current == 'd' || Current == 'D' || Current == 'm' || Current == 'M' || Current == '.' || Current == 'e'))
                return Real();

            if (!IsEOF() && !Current.IsWhiteSpace() && !Current.IsPunctuation())
                return Error();

            return CreateToken(TokenType.IntegerLiteral);
        }
        private Token Real()
        {
            if (Current == 'f' || Current == 'F' || Current == 'd' || Current == 'D' || Current == 'm' || Current == 'M')
            {
                Consume();

                if (!IsEOF() && (!Current.IsWhiteSpace() && !Current.IsPunctuation() || Current == '.'))
                    return Error(message: $"Remove '{Current}' in real number");

                return CreateToken(TokenType.RealLiteral);
            }

            var preDotLength = _index - _sourceFileLocation.Index;

            if (!IsEOF() && Current == '.')
                Consume();

            while (!IsEOF() && Current.IsDigit())
                Consume();

            if (!IsEOF() && Peek(-1) == '.')
                return Error(message: "Must contain digits after '.'");

            if (!IsEOF() && Current == 'e')
            {
                Consume();

                if (preDotLength > 1)
                    return Error(message: "Coefficient must be less than 10.");

                if (Current == '+' || Current == '-')
                    Consume();

                while (Current.IsDigit())
                    Consume();
            }

            if (!IsEOF() && (Current == 'f' || Current == 'F' || Current == 'd' || Current == 'D' || Current == 'm' || Current == 'M'))
                Consume();

            if (!IsEOF() && !Current.IsWhiteSpace() && !Current.IsPunctuation())
            {
                if (Current.IsLetter())
                    return Error(message: "'{0}' is an invalid read value");

                return Error();
            }

            return CreateToken(TokenType.RealLiteral);
        }
        private Token Comment()
        {
            Consume();

            if (Current == '*')
                return BlockComment();

            Consume();

            while (!IsEOF() && !Current.IsNewLine())
                Consume();

            return CreateToken(TokenType.LineComment);
        }
        private Token BlockComment()
        {
            // We're going to allow nested block comments.
            var level = 1;

            Func<bool> IsStartOfComment = () => Current == '/' && Next == '*';
            Func<bool> IsEndOfComment = () => Current == '*' && Next == '/';

            while (level > 0)
            {
                if (IsEOF())
                {
                    AddError("Unterminated block comment", Severity.Error);
                    return CreateToken(TokenType.Error);
                }

                if (IsStartOfComment())
                {
                    level++;

                    Consume();
                    Consume();
                }
                else if (IsEndOfComment())
                {
                    level--;

                    Consume();
                    Consume();
                }
                else
                {
                    Consume();
                }
            }

            return CreateToken(TokenType.BlockComment);
        }
        private Token Identifier()
        {
            while (!IsEOF() && Current.IsIdentifier())
                Consume();

            if (!IsEOF() && !Current.IsWhiteSpace() && !Current.IsPunctuation())
                return Error();

            if (_builder.ToString().IsKeyword(_keywords))
                return CreateToken(TokenType.Keyword);

            return CreateToken(TokenType.Identifier);
        }
        private Token CharLiteral()
        {
            // Allow escaping
            Advance();

            var escaping = false;

            while (Current != '\'' || escaping)
            {
                if (IsEOF())
                {
                    AddError("Unexpected End Of File", Severity.Fatal);
                    return CreateToken(TokenType.Error);
                }

                if (escaping)
                {
                    escaping = false;
                }
                else if (Current == '\\')
                {
                    Advance();
                    escaping = true;
                    continue;
                }

                Consume();
            }

            Advance();

            return CreateToken(TokenType.CharLiteral);
        }
        private Token StringLiteral()
        {
            Advance();

            var escaping = false;

            while (Current != '"' || escaping)
            {
                if (IsEOF())
                {
                    AddError("Unexpected End Of File", Severity.Fatal);
                    return CreateToken(TokenType.Error);
                }

                if (escaping)
                {
                    escaping = false;
                }
                else if (Current == '\\')
                {
                    Advance();
                    escaping = true;
                    continue;
                }

                Consume();
            }

            Advance();

            return CreateToken(TokenType.StringLiteral);
        }
        private Token Punctuation()
        {
            switch (Current)
            {
                case ';':
                    Consume();
                    return CreateToken(TokenType.Semicolon);

                case ':':
                    Consume();
                    return CreateToken(TokenType.Colon);

                case '{':
                    Consume();
                    return CreateToken(TokenType.LeftBracket);

                case '}':
                    Consume();
                    return CreateToken(TokenType.RightBracket);

                case '[':
                    Consume();
                    return CreateToken(TokenType.LeftBrace);

                case ']':
                    Consume();
                    return CreateToken(TokenType.RightBrace);

                case '(':
                    Consume();
                    return CreateToken(TokenType.LeftParanthesis);

                case ')':
                    Consume();
                    return CreateToken(TokenType.RightParenthesis);

                case '.':
                    Consume();
                    return CreateToken(TokenType.Dot);

                case ',':
                    Consume();
                    return CreateToken(TokenType.Comma);

                case '>':
                    Consume();
                    if (!IsEOF() && Current == '=')
                    {
                        Consume();
                        return CreateToken(TokenType.GreaterThanOrEqual);
                    }
                    else if (!IsEOF() && Current == '>')
                    {
                        Consume();
                        CreateToken(TokenType.BitShiftRight);
                    }
                    return CreateToken(TokenType.GreaterThan);

                case '<':
                    Consume();
                    if (!IsEOF() && Current == '=')
                    {
                        Consume();
                        return CreateToken(TokenType.LessThanOrEqual);
                    }
                    else if (!IsEOF() && Current == '<')
                    {
                        Consume();
                        CreateToken(TokenType.BitShiftLeft);
                    }
                    return CreateToken(TokenType.LessThan);

                case '+':
                    Consume();
                    if (!IsEOF() && Current == '=')
                    {
                        Consume();
                        return CreateToken(TokenType.PlusEqual);
                    }
                    else if (!IsEOF() && Current == '+')
                    {
                        Consume();
                        CreateToken(TokenType.PlusPlus);
                    }
                    return CreateToken(TokenType.Plus);

                case '-':
                    Consume();
                    if (!IsEOF() && Current == '=')
                    {
                        Consume();
                        return CreateToken(TokenType.MinusEqual);
                    }
                    else if (!IsEOF() && Current == '-')
                    {
                        Consume();
                        CreateToken(TokenType.MinusMinus);
                    }
                    return CreateToken(TokenType.Minus);

                case '=':
                    Consume();
                    if (!IsEOF() && Current == '=')
                    {
                        Consume();
                        return CreateToken(TokenType.Equal);
                    }
                    else if (!IsEOF() && Current == '>')
                    {
                        Consume();
                        CreateToken(TokenType.FatArrow);
                    }
                    return CreateToken(TokenType.Assignment);

                case '!':
                    Consume();
                    if (!IsEOF() && Current == '=')
                    {
                        Consume();
                        CreateToken(TokenType.NotEqual);
                    }
                    return CreateToken(TokenType.Not);

                case '*':
                    Consume();
                    if (!IsEOF() && Current == '=')
                    {
                        Consume();
                        CreateToken(TokenType.MulEqual);
                    }
                    return CreateToken(TokenType.Mul);

                case '/':
                    Consume();
                    if (!IsEOF() && Current == '=')
                    {
                        Consume();
                        CreateToken(TokenType.DivEqual);
                    }
                    return CreateToken(TokenType.Div);

                case '%':
                    Consume();
                    if (!IsEOF() && Current == '=')
                    {
                        Consume();
                        CreateToken(TokenType.ModEqual);
                    }
                    return CreateToken(TokenType.Mod);

                case '^':
                    Consume();
                    if (!IsEOF() && Current == '=')
                    {
                        Consume();
                        CreateToken(TokenType.BitwiseXorEqual);
                    }
                    return CreateToken(TokenType.BitwiseXor);

                case '?':
                    Consume();
                    if (!IsEOF() && Current == '?')
                    {
                        Consume();
                        CreateToken(TokenType.DoubleQuestion);
                    }
                    return CreateToken(TokenType.Question);

                case '&':
                    Consume();
                    if (!IsEOF() && Current == '=')
                    {
                        Consume();
                        return CreateToken(TokenType.BitwiseAndEqual);
                    }
                    else if (!IsEOF() && Current == '&')
                    {
                        Consume();
                        CreateToken(TokenType.BooleanAnd);
                    }
                    return CreateToken(TokenType.BitwiseAnd);

                case '|':
                    Consume();
                    if (!IsEOF() && Current == '=')
                    {
                        Consume();
                        return CreateToken(TokenType.BitwiseOrEqual);
                    }
                    else if (!IsEOF() && Current == '|')
                    {
                        Consume();
                        CreateToken(TokenType.BooleanOr);
                    }
                    return CreateToken(TokenType.BitwiseOr);

                default:
                    return Error();
            }

            
        }
        private bool IsEOF()
        {
            return _index == _sourceFile.Contents.Length || Current.IsEOF();
        }
        private char Peek(int ahead)
        {
            if (_index + ahead > _sourceFile.Contents.Length - 1)
                return '\0';

            return _sourceFile.Contents[_index + ahead];
        }
        private void Consume()
        {
            _builder.Append(Current);
            Advance();
        }
        private void Advance()
        {
            _index++;
            _column++;
        }
        private void AddError(string message, Severity severity)
        {
            var sourceFilePart = new SourceFilePart(
                _sourceFile.Name,
                _sourceFileLocation,
                new SourceFileLocation(_column, _index, _line),
                _builder.ToString().Split('\n'));

            _errorSink.AddError(message, sourceFilePart, severity);
        }

        public Tokenizer(TokenizerGrammar grammar) : this(grammar, new ErrorSink()) { }
        public Tokenizer(TokenizerGrammar grammar, ErrorSink errorSink)
        {
            if (grammar == null)
                throw new ArgumentNullException(nameof(grammar));

            if (errorSink == null)
                throw new ArgumentNullException(nameof(errorSink));

            _builder = new StringBuilder();
            _grammar = grammar;
            _errorSink = errorSink;
            _keywords = grammar.Keywords.Select(match => match.Value).ToArray();
        }
    }
}
