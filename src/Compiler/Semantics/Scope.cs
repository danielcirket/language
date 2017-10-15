using Compiler.Parsing.Syntax.Declarations;

namespace Compiler.Semantics
{
    internal class Scope
    {
        private readonly Scope _parent;
        private readonly SymbolTable _symbols;

        public void AddModule(string name, ModuleDeclaration declaration) => _symbols.AddModule(name, declaration);
        public bool ContainsModule(string name) => _symbols.ContainsModule(name);
        public bool TryGetValue(string name, out ModuleDeclaration declaration) => _symbols.TryGetValue(name, out declaration);

        public void AddClass(string name, ClassDeclaration declaration) => _symbols.AddClass(name, declaration);
        public bool ContainsClass(string name) => _symbols.ContainsClass(name);
        public bool TryGetValue(string name, out ClassDeclaration declaration) => _symbols.TryGetValue(name, out declaration);

        public void AddMethod(string name, MethodDeclaration declaration) => _symbols.AddMethod(name, declaration);
        public bool ContainsMethod(string name) => _symbols.ContainsMethod(name);
        public bool TryGetValue(string name, out MethodDeclaration declaration) => _symbols.TryGetValue(name, out declaration);

        public void AddField(string name, FieldDeclaration declaration) => _symbols.AddField(name, declaration);
        public bool ContainsField(string name) => _symbols.ContainsField(name);
        public bool TryGetValue(string name, out FieldDeclaration declaration) => _symbols.TryGetValue(name, out declaration);

        public void AddProperty(string name, PropertyDeclaration declaration) => _symbols.AddProperty(name, declaration);
        public bool ContainsProperty(string name) => _symbols.ContainsProperty(name);
        public bool TryGetValue(string name, out PropertyDeclaration declaration) => _symbols.TryGetValue(name, out declaration);

        public void AddConstructor(string name, ConstructorDeclaration declaration) => _symbols.AddConstructor(name, declaration);
        public bool ContainsConstructor(string name) => _symbols.ContainsConstructor(name);
        public bool TryGetValue(string name, out ConstructorDeclaration declaration) => _symbols.TryGetValue(name, out declaration);

        public void AddVariable(string name, Declaration declaration) => _symbols.AddVariable(name, declaration);
        public bool ContainsVariable(string name) => _symbols.ContainsVariable(name);
        public bool TryGetValue(string name, out Declaration declaration) => _symbols.TryGetValue(name, out declaration);

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
