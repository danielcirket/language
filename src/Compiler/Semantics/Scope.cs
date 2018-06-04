using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax;
using Compiler.Semantics.BoundSyntax.Declarations;
using Compiler.Semantics.Symbols;

namespace Compiler.Semantics
{
    internal class Scope
    {
        private readonly Scope _parent;
        private readonly SymbolTable _symbols;

        public Scope Parent => _parent;
        public SymbolTable Symbols => _symbols;

        //public bool Contains(string name)
        //{
        //
        //}
        //public IEnumerable<Symbol<T>> Find<T>(string name) where T : BoundDeclaration
        //{
        //
        //}
        public void AddOrUpdate(Symbol symbol) => _symbols.AddOrUpdate(symbol);
        public bool TryGetValue(string name, out Symbol symbol) => _symbols.TryGetValue(name, out symbol);

        public void CopyTo(ErrorSink errorSink, Scope scope)
        {
            foreach(var item in _symbols)
            {
                if (scope.TryGetValue(item.Name, out Symbol symbol))
                {
                    if (item.Declaration != null)
                    {
                        var filePart = item.Declaration?.SyntaxNode<SyntaxNode>().FilePart;
                        var existingDeclarationFilePart = symbol.Declaration?.SyntaxNode<SyntaxNode>().FilePart;
                        errorSink.AddError($"'{item.Name}' already declared and imported via an 'import' statement. Original declaration in '{existingDeclarationFilePart.FilePath}' ({existingDeclarationFilePart.Start.LineNumber}, {existingDeclarationFilePart.Start.Column}) and will shadow the parent declaration in '{filePart.FilePath}'", filePart, Severity.Warning);
                    }
                }

                scope.AddOrUpdate(item);
            }
        }

        public Scope()
        {
            _symbols = new SymbolTable();
        }
        public Scope(Scope parent)
        {
            _parent = parent;
            _symbols = new SymbolTable(parent._symbols);
        }
    }
}
