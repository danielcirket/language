using System;
using System.Collections.Generic;
using Compiler.Semantics.BoundSyntax.Expressions;

namespace Compiler.Semantics.Passes.Declaration
{
    /// <summary>
    /// This goes through the compilation unit's ASTs, then proceeds to go through each node and checking that the
    /// declaration exists and binding the usage of the declaration to the callee
    /// 
    /// We want to keep this immutable rather than tracking state everywhere!
    /// </summary>
    internal class DeclarationPass : ISemanticPass
    {
        private ErrorSink _errorSink;
        private BoundCompilationRoot _compilationRoot;
        private Scope _rootScope;
        private Stack<Scope> _scopes;
        private SyntaxBinder _syntaxBinder;
        public Dictionary<string, BoundTypeExpression> _predefinedTypeMap;

        public bool ShouldContinue => !_errorSink.HasErrors;

        public void Run(ref BoundCompilationRoot compilationRoot)
        {
            if (compilationRoot == null)
                throw new ArgumentNullException(nameof(compilationRoot));

            _compilationRoot = compilationRoot;
            _predefinedTypeMap = compilationRoot.PredefinedTypeMap;
            _rootScope = compilationRoot.Scope;
            _scopes.Push(_rootScope);

            var compilationUnits = new List<BoundCompilationUnit>();

            foreach (var unit in compilationRoot.CompilationUnits)
                compilationUnits.Add(_syntaxBinder.BindCompilationUnit(unit.CompilationSyntax(), _predefinedTypeMap, _rootScope));

            compilationRoot = new BoundCompilationRoot(compilationUnits, _predefinedTypeMap, _rootScope);
        }

        private void AddError(string message, SourceFilePart part)
        {
            _errorSink.AddError(message, part, Severity.Error);
        }
        private void AddWarning(string message, SourceFilePart part)
        {
            _errorSink.AddError(message, part, Severity.Warning);
        }
        private void AddInfo(string message, SourceFilePart part)
        {
            _errorSink.AddError(message, part, Severity.Message);
        }

        public DeclarationPass(ErrorSink errorSink)
        {
            _errorSink = errorSink;
            _syntaxBinder = new SyntaxBinder(SyntaxBindingMode.Full, errorSink);
            _scopes = new Stack<Scope>();
        }
    }
}
