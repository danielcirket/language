using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.Lexing;
using Compiler.Parsing.Syntax;
using Compiler.Parsing.Syntax.Declarations;
using Compiler.Parsing.Syntax.Expressions;
using Compiler.Parsing.Syntax.Statements;

namespace Compiler.Parsing
{
    internal class SyntaxParser
    {
        private readonly Tokenizer _tokenizer;
        private readonly ErrorSink _errorSink;
        private SourceFile _currentSourceFile;
        private IEnumerable<Token> _tokens;
        private int _index;
        private bool _error;

        public ErrorSink ErrorSink => _errorSink;
        private Token Current => _tokens.ElementAtOrDefault(_index) ?? _tokens.Last();
        private Token Next => Peek(1);
        private Token Last => Peek(-1);

        public CompilationUnit Parse(SourceFile sourceFile)
        {
            if (sourceFile == null)
                throw new ArgumentNullException(nameof(sourceFile));

            _currentSourceFile = sourceFile;
            _tokens = _tokenizer.Tokenize(sourceFile).Where(token => !token.IsTrivia()).ToList();

            // TODO(Dan): If we have errors already just bail? Perhaps we want to actually try parsing instead?
            if (_errorSink.HasErrors)
                return null;

            try
            {
                return new CompilationUnit(new[] { ParseInternal() });
            }
            catch (SyntaxException)
            {
                return null;
            }
        }
        private SyntaxNode ParseInternal()
        {
            return ParseDocument();
        }
        private SourceDocument ParseDocument()
        {
            var imports = new List<ImportStatement>();
            var modules = new List<ModuleDeclaration>();

            var start = Current.Start;

            while (Current == "import")
                imports.Add(ParseImportStatement());

            while (Current == "module")
                modules.Add(ParseModuleDeclaration());

            if (Current != TokenType.EOF)
                AddError("Top-level statements are not permitted. Statements must be part of a module with the exception of import statements which are at the start of the file", CreatePart(Current.Start, _tokens.Last().End), Severity.Error);

            return new SourceDocument(CreatePart(start), imports, modules);
        }

        // Statements
        private ImportStatement ParseImportStatement()
        {
            var token = Take("import");

            var moduleNameParts = new List<IdentifierExpression>();

            while (Current == TokenType.Identifier && Current != TokenType.Semicolon)
            {
                var part = CreatePart(Current.Start);
                var value = ParseIdentifierName();

                moduleNameParts.Add(new IdentifierExpression(part, value));

                if (Current != TokenType.Dot && Current != TokenType.Semicolon)
                    throw UnexpectedToken("'.' or ';'");

                if (Current == TokenType.Dot)
                    Advance();
            }

            Take(TokenType.Semicolon);

            return new ImportStatement(CreatePart(token.Start), moduleNameParts);
        }

        // Declarations
        private ModuleDeclaration ParseModuleDeclaration()
        {
            var keyword = TakeKeyword("module");

            var moduleNameParts = new List<IdentifierExpression>();
            var classes = new List<ClassDeclaration>();
            var methods = new List<MethodDeclaration>();

            while (Current == TokenType.Identifier)
            {
                moduleNameParts.Add(new IdentifierExpression(CreatePart(Current.Start), ParseIdentifierName()));

                if (Current == TokenType.Dot)
                    Advance();
            }

            MakeBlock(() =>
            {
                while (Current == "class" || Current == TokenType.Identifier)
                {
                    if (Current == "class")
                    {
                        throw new NotImplementedException();

                        //classes.Add(ParseClassDeclaration());
                    }
                    else
                    {
                        // TODO(Dan): Module level method
                        throw new NotImplementedException();
                    }
                }
            });

            return new ModuleDeclaration(CreatePart(keyword.Start), string.Join(".", moduleNameParts.Select(identifier => identifier.Name)), classes, methods);
        }

        // Errors
        private void AddError(string message, SourceFilePart part, Severity severity)
        {
            _errorSink.AddError($"{message} in '{_currentSourceFile.Name}'", part, severity);
        }
        private SyntaxException UnexpectedToken(TokenType expected)
        {
            return UnexpectedToken($"{expected.Value()}");
        }
        private SyntaxException UnexpectedToken(string expected)
        {
            Advance();

            var value = string.IsNullOrEmpty(Last?.Value)
                ? Last?.TokenType.ToString()
                : Last?.Value;

            var message = $"Unexpected '{value}'. Expected {expected}";

            return SyntaxError(message, CreatePart(Last.Start, Current.End), Severity.Error);
        }
        private SyntaxException SyntaxError(string message, SourceFilePart part, Severity severity)
        {
            _error = true;
            AddError(message, part, severity);
            return new SyntaxException(message);
        }

        // Helpers / Utilities
        private Token Peek(int ahead)
        {
            return _tokens.ElementAtOrDefault(_index + ahead) ?? _tokens.Last();
        }
        private void Advance()
        {
            _index++;
        }
        private bool IsMakingProgress(int lastTokenPosition)
        {
            if (_index > lastTokenPosition)
                return true;

            return false;
        }
        private Token Take()
        {
            var token = Current;

            Advance();

            return token;
        }
        private Token Take(TokenType type)
        {
            if (Current != type)
                throw UnexpectedToken(type);

            return Take();
        }
        private Token Take(string contextualKeyword)
        {
            if (Current != TokenType.Identifier && Current != contextualKeyword)
                throw UnexpectedToken(contextualKeyword);

            return Take();
        }
        private Token TakeKeyword(string keyword)
        {
            if (Current != TokenType.Keyword && Current != keyword)
                throw UnexpectedToken(keyword);

            return Take();
        }
        private Token TakeSemicolon()
        {
            return Take(TokenType.Semicolon);
        }
        private SourceFilePart CreatePart(SourceFileLocation start)
        {
            return CreatePart(start, Current.End);
        }
        private SourceFilePart CreatePart(SourceFileLocation start, SourceFileLocation end)
        {
            var content = _currentSourceFile.Lines.Skip(start.LineNumber - 1).Take(end.LineNumber - 1).ToArray();
            return new SourceFilePart(_currentSourceFile.Name, start, end, content);
        }
        private string ParseIdentifierName()
        {
            if (Current == TokenType.Identifier)
                return Take(TokenType.Identifier).Value;

            throw UnexpectedToken("Identifier");
        }
        private void MakeBlock(Action action, TokenType open = TokenType.LeftBracket, TokenType close = TokenType.RightBracket)
        {
            Take(open);

            MakeStatement(action, close);
        }
        private void MakeStatement(Action action, TokenType close = TokenType.Semicolon)
        {
            try
            {
                var startIndex = _index;

                while (Current != close && Current != TokenType.EOF)
                {
                    action();

                    if (!IsMakingProgress(startIndex))
                        throw SyntaxError($"Unexpected '{Current.Value}'", CreatePart(Current.Start), Severity.Error);

                    startIndex = _index;
                }
            }
            catch (SyntaxException)
            {
                while (Current != close && Current != TokenType.EOF)
                    Take();
            }
            finally
            {
                // TODO(Dan): Improve error recovery - this will be awful!
                if (_error)
                {
                    if (Last == close)
                        _index--;

                    if (Current != close || Next == close)
                    {
                        if (Next == close)
                            Take();

                        while (Current != close && Current != TokenType.EOF)
                            Take();
                    }

                    _error = false;
                }

                if (close == TokenType.Semicolon)
                    TakeSemicolon();
                else
                    Take(close);
            }
        }

        public SyntaxParser()
            : this(new Tokenizer(TokenizerGrammar.Default))
        { }
        public SyntaxParser(Tokenizer tokenizer)
            : this(tokenizer, tokenizer.ErrorSink)
        {
        }
        public SyntaxParser(Tokenizer tokenizer, ErrorSink errorSink)
        {
            if (tokenizer == null)
                throw new ArgumentNullException(nameof(tokenizer));

            if (errorSink == null)
                throw new ArgumentNullException(nameof(errorSink));

            _tokenizer = tokenizer;
            _errorSink = errorSink;
        }
    }
}
