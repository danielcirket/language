using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Compiler.Semantics.BoundSyntax.Declarations;
using Compiler.Semantics.Symbols;

namespace Compiler.Semantics
{
    internal class SymbolTable : IEnumerable<Symbol>
    {
        private readonly SymbolTable _parent;
        private readonly Dictionary<string, Symbol> _symbols;

        //public void Add(Symbol symbol)
        //{
        //
        //}
        //public void Update(Symbol symbol)
        //{
        //
        //}
        
        public void AddOrUpdate(Symbol symbol)
        {
            if (!_symbols.ContainsKey(symbol.Name))
            {
                _symbols.Add(symbol.Name, symbol);
                return;
            }

            _symbols[symbol.Name].Declaration = symbol.Declaration;
        }
        public bool TryGetValue(string name, out Symbol symbol)
        {
            if (_symbols.TryGetValue(name, out symbol))
                return true;
        
            if (_parent != null)
                return _parent.TryGetValue(name, out symbol);
        
            return false;
        }
        public IEnumerator<Symbol> GetEnumerator() => _symbols.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public SymbolTable()
        {
            _symbols = new Dictionary<string, Symbol>();
            //_modules = new Dictionary<string, BoundModuleDeclaration>();
            //_classes = new Dictionary<string, BoundClassDeclaration>();
            //_methods = new Dictionary<string, BoundMethodDeclaration>();
            //_fields = new Dictionary<string, BoundFieldDeclaration>();
            //_properties = new Dictionary<string, BoundPropertyDeclaration>();
            //_constructors = new Dictionary<string, BoundConstructorDeclaration>();
            //_variables = new Dictionary<string, BoundDeclaration>();
        }
        public SymbolTable(SymbolTable parent) : this()
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            _parent = parent;
        }
    }
}
