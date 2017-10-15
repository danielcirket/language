using System;
using System.Collections.Generic;
using System.Text;
using Compiler.Parsing.Syntax.Declarations;

namespace Compiler.Semantics
{
    internal class SymbolTable
    {
        private readonly SymbolTable _parent;
        private readonly Dictionary<string, ModuleDeclaration> _modules;
        private readonly Dictionary<string, ClassDeclaration> _classes;
        private readonly Dictionary<string, MethodDeclaration> _methods;
        private readonly Dictionary<string, FieldDeclaration> _fields;
        private readonly Dictionary<string, PropertyDeclaration> _properties;
        private readonly Dictionary<string, ConstructorDeclaration> _constructors;
        private readonly Dictionary<string, Declaration> _variables;

        public void AddModule(string name, ModuleDeclaration declaration)
        {
            _modules.Add(name, declaration);
        }
        public bool ContainsModule(string name)
        {
            if (_modules.ContainsKey(name))
                return true;

            if (_parent != null)
                return _parent.ContainsModule(name);

            return false;
        }
        public bool TryGetValue(string name, out ModuleDeclaration declaration)
        {
            declaration = null;

            if (_modules.ContainsKey(name))
            {
                declaration = _modules[name];
                return true;
            }

            if (_parent != null)
            {
                return _parent.TryGetValue(name, out declaration);
            }

            return false;
        }

        public void AddClass(string name, ClassDeclaration declaration)
        {
            _classes.Add(name, declaration);
        }
        public bool ContainsClass(string name)
        {
            if (_classes.ContainsKey(name))
                return true;

            if (_parent != null)
                return _parent.ContainsClass(name);

            return false;
        }
        public bool TryGetValue(string name, out ClassDeclaration declaration)
        {
            declaration = null;

            if (_classes.ContainsKey(name))
            {
                declaration = _classes[name];
                return true;
            }

            if (_parent != null)
            {
                return _parent.TryGetValue(name, out declaration);
            }

            return false;
        }

        public void AddMethod(string name, MethodDeclaration declaration)
        {
            _methods.Add(name, declaration);
        }
        public bool ContainsMethod(string name)
        {
            if (_methods.ContainsKey(name))
                return true;

            if (_parent != null)
                return _parent.ContainsMethod(name);

            return false;
        }
        public bool TryGetValue(string name, out MethodDeclaration declaration)
        {
            declaration = null;

            if (_methods.ContainsKey(name))
            {
                declaration = _methods[name];
                return true;
            }

            if (_parent != null)
            {
                return _parent.TryGetValue(name, out declaration);
            }

            return false;
        }

        public void AddField(string name, FieldDeclaration declaration)
        {
            _fields.Add(name, declaration);
        }
        public bool ContainsField(string name)
        {
            if (_fields.ContainsKey(name))
                return true;

            if (_parent != null)
                return _parent.ContainsField(name);

            return false;
        }
        public bool TryGetValue(string name, out FieldDeclaration declaration)
        {
            declaration = null;

            if (_fields.ContainsKey(name))
            {
                declaration = _fields[name];
                return true;
            }

            if (_parent != null)
            {
                return _parent.TryGetValue(name, out declaration);
            }

            return false;
        }

        public void AddProperty(string name, PropertyDeclaration declaration)
        {
            _properties.Add(name, declaration);
        }
        public bool ContainsProperty(string name)
        {
            if (_properties.ContainsKey(name))
                return true;

            if (_parent != null)
                return _parent.ContainsProperty(name);

            return false;
        }
        public bool TryGetValue(string name, out PropertyDeclaration declaration)
        {
            declaration = null;

            if (_properties.ContainsKey(name))
            {
                declaration = _properties[name];
                return true;
            }

            if (_parent != null)
            {
                return _parent.TryGetValue(name, out declaration);
            }

            return false;
        }

        public void AddConstructor(string name, ConstructorDeclaration declaration)
        {
            _constructors.Add(name, declaration);
        }
        public bool ContainsConstructor(string name)
        {
            if (_constructors.ContainsKey(name))
                return true;

            if (_parent != null)
                return _parent.ContainsConstructor(name);

            return false;
        }
        public bool TryGetValue(string name, out ConstructorDeclaration declaration)
        {
            declaration = null;

            if (_constructors.ContainsKey(name))
            {
                declaration = _constructors[name];
                return true;
            }

            if (_parent != null)
            {
                return _parent.TryGetValue(name, out declaration);
            }

            return false;
        }

        public void AddVariable(string name, Declaration declaration)
        {
            _variables.Add(name, declaration);
        }
        public bool ContainsVariable(string name)
        {
            if (_variables.ContainsKey(name))
                return true;

            if (_parent != null)
                return _parent.ContainsVariable(name);

            return false;
        }
        public bool TryGetValue(string name, out Declaration declaration)
        {
            declaration = null;

            if (_variables.ContainsKey(name))
            {
                declaration = _variables[name];
                return true;
            }

            if (_parent != null)
            {
                return _parent.TryGetValue(name, out declaration);
            }

            return false;
        }

        public SymbolTable()
        {
            _modules = new Dictionary<string, ModuleDeclaration>();
            _classes = new Dictionary<string, ClassDeclaration>();
            _methods = new Dictionary<string, MethodDeclaration>();
            _fields = new Dictionary<string, FieldDeclaration>();
            _properties = new Dictionary<string, PropertyDeclaration>();
            _constructors = new Dictionary<string, ConstructorDeclaration>();
            _variables = new Dictionary<string, Declaration>();
        }
        public SymbolTable(SymbolTable parent) : this()
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            _parent = parent;
        }
    }
}
