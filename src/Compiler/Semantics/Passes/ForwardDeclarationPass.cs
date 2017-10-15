using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.Parsing;
using Compiler.Parsing.Syntax;
using Compiler.Parsing.Syntax.Declarations;
using Compiler.Parsing.Syntax.Expressions;
using Compiler.Parsing.Syntax.Statements;

namespace Compiler.Semantics.Passes
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
    internal class ForwardDeclarationPass : SyntaxVisitor<SyntaxNode>, ISemanticPass
    {
        private ErrorSink _errorSink;
        private CompilationUnit _compilationUnit;
        private Scope _rootScope;
        private Stack<Scope> _scopes;
        private ModuleDeclaration _currentModule;

        public bool ShouldContinue => throw new NotImplementedException();

        public void Run(ErrorSink errorSink, ref CompilationUnit compilationUnit)
        {
            if (errorSink == null)
                throw new ArgumentNullException(nameof(errorSink));
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            _errorSink = errorSink;
            _compilationUnit = compilationUnit;

            _rootScope = new Scope();
            _scopes.Push(_rootScope);

            var children = new List<SyntaxNode>();

            foreach (var item in compilationUnit.Children)
                children.Add(item.Accept(this));

            compilationUnit = new CompilationUnit(compilationUnit, children, _rootScope);
        }

        protected override SyntaxNode VisitArithmetic(BinaryExpression expression)
        {
            throw new NotImplementedException();
        }
        protected override SyntaxNode VisitArrayAccess(ArrayAccessExpression expression)
        {
            throw new NotImplementedException();
        }
        protected override SyntaxNode VisitAssignment(BinaryExpression expression)
        {
            var right = expression.Right.Accept(this);
            var left = expression.Left.Accept(this);

            throw new NotImplementedException();
        }
        protected override SyntaxNode VisitBitwise(BinaryExpression expression)
        {
            throw new NotImplementedException();
        }
        protected override SyntaxNode VisitConstant(ConstantExpression expression)
        {
            throw new NotImplementedException();
        }
        protected override SyntaxNode VisitIdentifier(IdentifierExpression expression)
        {
            throw new NotImplementedException();
        }
        protected override SyntaxNode VisitLambda(LambdaExpression expression)
        {
            throw new NotImplementedException();
        }
        protected override SyntaxNode VisitLogical(BinaryExpression expression)
        {
            throw new NotImplementedException();
        }
        protected override SyntaxNode VisitMethodCall(MethodCallExpression expression)
        {
            throw new NotImplementedException();
        }
        protected override SyntaxNode VisitNew(NewExpression expression)
        {
            throw new NotImplementedException();
        }
        protected override SyntaxNode VisitReference(ReferenceExpression expression)
        {
            throw new NotImplementedException();
        }
        protected override SyntaxNode VisitUnary(UnaryExpression expression)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitBlock(BlockStatement statement)
        {
            throw new NotImplementedException();
        }
        protected override SyntaxNode VisitBreak(BreakStatement statement)
        {
            throw new NotImplementedException();
        }
        protected override SyntaxNode VisitCase(CaseStatement statement)
        {
            throw new NotImplementedException();
        }
        protected override SyntaxNode VisitContinue(ContinueStatement statement)
        {
            throw new NotImplementedException();
        }
        protected override SyntaxNode VisitElse(ElseStatement statement)
        {
            throw new NotImplementedException();
        }
        protected override SyntaxNode VisitEmpty(EmptyStatement statement)
        {
            throw new NotImplementedException();
        }
        protected override SyntaxNode VisitFor(ForStatement statement)
        {
            throw new NotImplementedException();
        }
        protected override SyntaxNode VisitIf(IfStatement statement)
        {
            throw new NotImplementedException();
        }
        protected override SyntaxNode VisitImport(ImportStatement statement)
        {
            throw new NotImplementedException();
        }
        protected override SyntaxNode VisitReturn(ReturnStatement statement)
        {
            throw new NotImplementedException();
        }
        protected override SyntaxNode VisitSwitch(SwitchStatement statement)
        {
            throw new NotImplementedException();
        }
        protected override SyntaxNode VisitWhile(WhileStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitModuleDeclaration(ModuleDeclaration moduleDeclaration)
        {
            _currentModule = moduleDeclaration;

            var currentScope = _scopes.Peek();

            Scope scope = null;

            if (_rootScope.TryGetValue(moduleDeclaration.Name, out ModuleDeclaration module))
            {
                scope = module.Scope;
            }
            else
            {
                //scope = new Scope(_rootScope);
                scope = new Scope(currentScope);
            }

            _scopes.Push(scope);

            var classes = new List<ClassDeclaration>();
            var methods = new List<MethodDeclaration>();

            foreach (var item in moduleDeclaration.Classes)
                classes.Add(item.Accept(this) as ClassDeclaration);

            foreach (var item in moduleDeclaration.Methods)
                methods.Add(item.Accept(this) as MethodDeclaration);

            _scopes.Pop();

            return new ModuleDeclaration(moduleDeclaration, classes, methods, scope);
        }
        protected override SyntaxNode VisitClass(ClassDeclaration classDeclaration)
        {
            var currentScope = _scopes.Peek();

            var fields = new List<FieldDeclaration>();
            var properties = new List<PropertyDeclaration>();
            var methods = new List<MethodDeclaration>();
            var constructors = new List<ConstructorDeclaration>();

            var scope = new Scope(currentScope);

            _scopes.Push(scope);

            if (currentScope.ContainsClass(classDeclaration.Name))
            {
                AddError($"Cannot redeclare class '{classDeclaration.Name}'", classDeclaration.FilePart);
            }

            foreach (var item in classDeclaration.Fields)
                fields.Add(item.Accept(this) as FieldDeclaration);

            foreach (var item in classDeclaration.Properties)
                properties.Add(item.Accept(this) as PropertyDeclaration);

            foreach (var item in classDeclaration.Methods)
                methods.Add(item.Accept(this) as MethodDeclaration);

            foreach (var item in classDeclaration.Constructors)
                constructors.Add(item.Accept(this) as ConstructorDeclaration);

            // TODO(Dan): Consider navigating the class even if an additional declaration!?

            var @class = new ClassDeclaration(classDeclaration, fields, properties, methods, constructors, scope);

            currentScope.AddClass(classDeclaration.Name, @class);

            _scopes.Pop();

            return @class;
        }
        protected override SyntaxNode VisitConstructor(ConstructorDeclaration constructorDeclaration)
        {
            var currentScope = _scopes.Peek();

            if (currentScope.ContainsConstructor(constructorDeclaration.Name))
            {
                var parameterDescription = string.Join(" ", constructorDeclaration.Parameters.Select(param => param.Type.Name)).Trim();

                var description = string.IsNullOrWhiteSpace(parameterDescription)
                    ? ""
                    : $"'{parameterDescription}'";

                AddError($"Cannot redeclare constructor {description}", constructorDeclaration.FilePart);
            }

            var scope = new Scope(currentScope);

            _scopes.Push(scope);

            var parameters = new List<ParameterDeclaration>();

            foreach (var item in constructorDeclaration.Parameters)
                parameters.Add(item.Accept(this) as ParameterDeclaration);

            var constructor = new ConstructorDeclaration(constructorDeclaration, parameters, constructorDeclaration.Body, scope);

            currentScope.AddConstructor(constructorDeclaration.Name, constructor);

            _scopes.Pop();

            return constructor;
        }
        protected override SyntaxNode VisitField(FieldDeclaration fieldDeclaration)
        {
            var currentScope = _scopes.Peek();

            if (currentScope.ContainsField(fieldDeclaration.Name))
            {
                AddError($"Cannot redeclare field '{fieldDeclaration.Name}'", fieldDeclaration.FilePart);
            }

            var field = new FieldDeclaration(fieldDeclaration, currentScope);

            currentScope.AddField(fieldDeclaration.Name, field);

            return field;
        }
        protected override SyntaxNode VisitMethod(MethodDeclaration methodDeclaration)
        {
            var currentScope = _scopes.Peek();
            var scope = new Scope(currentScope);

            _scopes.Push(scope);

            var parameters = new List<ParameterDeclaration>();

            foreach (var item in methodDeclaration.Parameters)
                parameters.Add(item.Accept(this) as ParameterDeclaration);

            var declaration = new MethodDeclaration(methodDeclaration, parameters, scope);

            if (currentScope.ContainsMethod(methodDeclaration.Name))
            {
                AddError($"Cannot redeclare method '{methodDeclaration.Name}'", methodDeclaration.FilePart);
            }
            else
            {
                currentScope.AddMethod(methodDeclaration.Name, declaration);
            }

            _scopes.Pop();

            return declaration;
        }
        protected override SyntaxNode VisitParameter(ParameterDeclaration parameterDeclaration)
        {
            var currentScope = _scopes.Peek();

            var parameter = new ParameterDeclaration(parameterDeclaration, currentScope);

            if (currentScope.ContainsVariable(parameterDeclaration.Name))
            {
                AddError($"Cannot redeclare identifier '{parameterDeclaration.Name}'", parameterDeclaration.FilePart);
            }
            else
            {
                currentScope.AddVariable(parameterDeclaration.Name, parameter);
            }

            return parameter;
        }
        protected override SyntaxNode VisitProperty(PropertyDeclaration propertyDeclaration)
        {
            var currentScope = _scopes.Peek();

            if (currentScope.ContainsProperty(propertyDeclaration.Name))
            {
                AddError($"Cannot redeclare property '{propertyDeclaration.Name}'", propertyDeclaration.FilePart);
            }

            var getMethod = propertyDeclaration.Getter.Accept(this) as MethodDeclaration;
            var setMethod = propertyDeclaration.Setter?.Accept(this) as MethodDeclaration;

            var property = new PropertyDeclaration(propertyDeclaration, getMethod, setMethod, currentScope);

            currentScope.AddProperty(propertyDeclaration.Name, propertyDeclaration);

            return property;
        }
        protected override SyntaxNode VisitType(TypeDeclaration typeDeclaration)
        {
            throw new NotImplementedException();
        }
        protected override SyntaxNode VisitVariable(VariableDeclaration variableDeclaration)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode VisitDocument(SourceDocument sourceDocument)
        {
            var currentScope = _scopes.Peek();

            var scope = new Scope(currentScope);

            _scopes.Push(scope);

            var modules = new List<ModuleDeclaration>();

            foreach (var item in sourceDocument.Modules)
            {
                var module = item.Accept(this) as ModuleDeclaration;
                modules.Add(module);

                if (!scope.ContainsModule(module.Name))
                    scope.AddModule(module.Name, module);
            }

            var document = new SourceDocument(sourceDocument, modules, scope);

            _scopes.Pop();

            return document;
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

        public ForwardDeclarationPass()
        {
            _scopes = new Stack<Scope>();
        }
    }
}
