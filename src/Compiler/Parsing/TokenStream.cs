using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Compiler.Lexing;

namespace Compiler.Parsing
{
    internal class TokenStream
    {
        private readonly Tokenizer _tokenizer;
        private readonly SourceFile _sourceFile;
        private int _index;
        private List<Token> _tokens;

        public int Position => _index;
        public int Length => _tokens.Count - 1;
        public Token Current => _tokens.ElementAtOrDefault(_index) ?? _tokens.Last();
        public Token Next => Peek(1);
        public Token Last => Peek(-1);

        private Token Take()
        {
            var token = Current;

            Advance();

            return token;
        }
        private Token Take(TokenType type)
        {
            if (Current != type)
                return null;

            return Take();
        }
        public Token Peek(int ahead = 1)
        {
            return _tokens.ElementAtOrDefault(_index + ahead) ?? _tokens.Last();
        }
        public bool IsMakingProgress(int lastTokenPosition)
        {
            if (_index > lastTokenPosition)
                return true;

            return false;
        }
        public Token Dequeue()
        {
            var token = Current;

            _tokens.RemoveAt(_index);

            return token;
        }
        public void Enqueue(Token token)
        {
            _tokens.Insert(_index, token);
        }
        public void Advance(int number = 1)
        {
            if (number < 1)
                throw new ArgumentException("You cannot advance the stream by less than 1. If you need to rewind the stream use 'Rewind'");

            _index = _index + number;
        }
        public void Rewind(int number = 1)
        {
            if (number < 1)
                throw new ArgumentException("You cannot rewind the stream by less than 1. If you need to advance the stream use 'Advance'");

            _index = _index - number;
        }

        public TokenStream(SourceFile sourceFile)
            : this(sourceFile, new Tokenizer(TokenizerGrammar.Default))
        {

        }
        public TokenStream(SourceFile sourceFile, Tokenizer tokenizer)
        {
            if (tokenizer == null)
                throw new ArgumentNullException(nameof(tokenizer));
            if (sourceFile == null)
                throw new ArgumentNullException(nameof(sourceFile));

            _tokenizer = tokenizer;
            _sourceFile = sourceFile;
            _index = 0;
            _tokens = tokenizer.Tokenize(sourceFile).Where(t => !t.IsTrivia()).ToList();
        }
    }
}
