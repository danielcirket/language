using System;
using System.Collections.Generic;
using Compiler.Parsing;
using Compiler.Parsing.Syntax.Statements;
using Compiler.Semantics.BoundSyntax.Expressions;
using Compiler.Semantics.BoundSyntax.Statements;

namespace Compiler.Semantics.Passes.Declaration
{
    /// <summary>
    /// This goes through the compilation unit's ASTs, then proceeds to go through each module declaration
    /// looking only at the top level method declarations and class declarations.
    /// 
    /// For class declarations we also get the information for the fields, properties, methods and constructors,
    /// however we do not visit past the delcaration itself.
    /// 
    /// This allows us to know the structure of the module and classes before using their symbols in the child scopes
    /// so things don't need to be forward declared.
    /// 
    /// Obviously this is only a temporary pass as it would be better to fill out this info while we process the syntax
    /// and might be bound at that time, but for simplicity we will do this for now.
    /// 
    /// We want to keep this immutable rather than tracking state everywhere!
    /// </summary>
    internal class ForwardDeclarationPass
    {
        private ErrorSink _errorSink;
        private CompilationRoot _compilationRoot;
        private SyntaxBinder _syntaxBinder;
        public Dictionary<string, BoundTypeExpression> _predefinedTypeMap;

        public bool ShouldContinue => true;

        public BoundCompilationRoot Run(CompilationRoot compilationRoot)
        {
            if (compilationRoot == null)
                throw new ArgumentNullException(nameof(compilationRoot));

            _compilationRoot = compilationRoot;

            var rootScope = new Scope();

            var temp = new List<BoundCompilationUnit>();
            var compilationUnits = new List<BoundCompilationUnit>();

            foreach (var unit in compilationRoot.CompilationUnits)
                compilationUnits.Add(_syntaxBinder.BindCompilationUnit(unit, _predefinedTypeMap, rootScope));

            //foreach(var unit in temp)
            //{
            //    var imports = new List<BoundImportStatement>();
            //
            //    foreach (var import in unit.Imports)
            //        imports.Add((BoundImportStatement)_syntaxBinder.BindStatement(import.SyntaxNode<ImportStatement>(), rootScope));
            //
            //    compilationUnits.Add(new BoundCompilationUnit(unit.CompilationSyntax(), imports, unit.Modules));
            //}

            return new BoundCompilationRoot(compilationUnits, _predefinedTypeMap, rootScope);
        }

        public ForwardDeclarationPass(ErrorSink errorSink)
        {
            _errorSink = errorSink;
            _syntaxBinder = new SyntaxBinder(SyntaxBindingMode.DeclarationOnly, _errorSink);
            _predefinedTypeMap = new Dictionary<string, BoundTypeExpression>();
        }
    }
}
