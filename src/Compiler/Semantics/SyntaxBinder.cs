using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.Parsing;
using Compiler.Parsing.Syntax;
using Compiler.Parsing.Syntax.Declarations;
using Compiler.Parsing.Syntax.Expressions;
using Compiler.Parsing.Syntax.Expressions.Types;
using Compiler.Parsing.Syntax.Statements;
using Compiler.Semantics.BoundSyntax;
using Compiler.Semantics.BoundSyntax.Declarations;
using Compiler.Semantics.BoundSyntax.Expressions;
using Compiler.Semantics.BoundSyntax.Expressions.Types;
using Compiler.Semantics.BoundSyntax.Statements;
using Compiler.Semantics.Symbols;
using Compiler.Semantics.Types;

namespace Compiler.Semantics
{
    internal class SyntaxBinder : SyntaxVisitor<SyntaxNode, BoundSyntaxNode>
    {
        private readonly ErrorSink _errorSink;
        // NOTE(Dan): This isn't readonly because we will want to parse interfaces as declaration only, even in full binding mode.
        private SyntaxBindingMode _mode; 
        private Stack<Scope> _scopes;
        private ReferenceDeclarationLocator _referenceDeclarationLocator;
        private ReferenceTypeLocator _referenceTypeLocator;
        public Dictionary<string, BoundTypeExpression> _predefinedTypeMap;

        private Scope CurrentScope => _scopes.Peek();

        public BoundCompilationUnit BindCompilationUnit(CompilationUnit unit, Dictionary<string, BoundTypeExpression> predefinedTypeMap, Scope scope)
        {
            if (unit == null)
                return null;

            if (_scopes.Count > 0)
                throw new Exception("Scopes stack is not empty...");

            _scopes.Push(scope);

            var modules = new List<BoundModuleDeclaration>();
            var imports = new List<BoundImportStatement>();

            // Compilation Unit Scope
            _scopes.Push(new Scope(scope));
            _predefinedTypeMap = predefinedTypeMap;

            if (_mode == SyntaxBindingMode.Full)
            {
                foreach (var import in unit.Imports)
                {
                    var boundImport = (BoundImportStatement)import.Accept(this);
                    imports.Add(boundImport);
                }

                foreach (var import in imports)
                    import.Scope.CopyTo(_errorSink, CurrentScope);
            }

            foreach (var module in unit.Modules)
                modules.Add((BoundModuleDeclaration)module.Accept(this));

            var compilationUnit = new BoundCompilationUnit(unit, imports, modules, CurrentScope);

            // Compilation Unit Scope
            _scopes.Pop();
            // Root Scope
            _scopes.Pop();

            return compilationUnit;
        }

        protected override BoundSyntaxNode VisitArithmetic(BinaryExpression expression)
        {
            var left = (BoundExpression)expression.Left.Accept(this);
            var right = (BoundExpression)expression.Right.Accept(this);

            return new BoundBinaryExpression(expression, left, right, CurrentScope);
        }
        protected override BoundSyntaxNode VisitArrayAccess(ArrayAccessExpression expression)
        {
            var arguments = new List<BoundExpression>();

            foreach(var argument in expression.Arguments)
                arguments.Add((BoundExpression)argument.Accept(this));
            
            var reference = (BoundExpression)expression.Reference.Accept(this);

            return new BoundArrayAccessExpression(expression, reference, arguments, CurrentScope);
        }
        protected override BoundSyntaxNode VisitAssignment(BinaryExpression expression)
        {
            var boundLeft = (BoundExpression)expression.Left.Accept(this);
            var boundRight = (BoundExpression)expression.Right.Accept(this);

            return new BoundBinaryExpression(expression, boundLeft, boundRight, CurrentScope);
        }
        protected override BoundSyntaxNode VisitBitwise(BinaryExpression expression)
        {
            var left = (BoundExpression)expression.Left.Accept(this);
            var right = (BoundExpression)expression.Right.Accept(this);

            return new BoundBinaryExpression(expression, left, right, CurrentScope);
        }
        protected override BoundSyntaxNode VisitConstant(ConstantExpression expression)
        {
            switch (expression.ConstantType)
            {
                case ConstantType.Integer:
                    return new BoundConstantExpression(expression, _predefinedTypeMap["Int32"], CurrentScope);
                case ConstantType.Real:
                    return new BoundConstantExpression(expression, _predefinedTypeMap["Float"], CurrentScope);
                case ConstantType.String:
                    return new BoundConstantExpression(expression, _predefinedTypeMap["String"], CurrentScope);
                case ConstantType.Boolean:
                    return new BoundConstantExpression(expression, _predefinedTypeMap["Bool"], CurrentScope);
            }

            return new BoundConstantExpression(expression, CurrentScope);
        }
        protected override BoundSyntaxNode VisitIdentifier(IdentifierExpression expression)
        {
            if (!CurrentScope.TryGetValue(expression.Name, out Symbol symbol))
                AddError($"Use of undeclared identifier '{expression.Name}'", expression.FilePart);

            return new BoundIdentifierExpression(expression, symbol, CurrentScope);
        }
        protected override BoundSyntaxNode VisitLambda(LambdaExpression expression)
        {
            _scopes.Push(new Scope(CurrentScope));

            var parameters = new List<BoundParameterDeclaration>();

            foreach(var parameter in expression.Parameters)
                parameters.Add((BoundParameterDeclaration)parameter.Accept(this));

            var body = (BoundBlockStatement)expression.Body.Accept(this);

            _scopes.Pop();

            return new BoundLambdaExpression(expression, parameters, body, CurrentScope);
        }
        protected override BoundSyntaxNode VisitLogical(BinaryExpression expression)
        {
            var left = (BoundExpression)expression.Left.Accept(this);
            var right = (BoundExpression)expression.Right.Accept(this);

            // TODO(Dan): This needs to be moved into the typecheck pass
            //if (!(left.Type is BoundInferredTypeExpression) && !(right.Type is BoundInferredTypeExpression))
            //    throw new NotImplementedException("Fix this! Logical expression types are not matching!");

            return new BoundBinaryExpression(expression, left, right, right.Type, CurrentScope);
        }
        protected override BoundSyntaxNode VisitMethodCall(MethodCallExpression expression)
        { 
            var reference = (BoundExpression)expression.Reference.Accept(this);
            var arguments = new List<BoundExpression>();

            foreach (var argument in expression.Arguments)
                arguments.Add((BoundExpression)argument.Accept(this));

            if (reference.Type != null)
                return new BoundMethodCallExpression(expression, reference, arguments, reference.Type, CurrentScope);

            return new BoundMethodCallExpression(expression, reference, arguments, CurrentScope);
        }
        protected override BoundSyntaxNode VisitNew(NewExpression expression)
        {
            var reference = (BoundTypeExpression)expression.Reference.Accept(this);
            var arguments = new List<BoundExpression>();

            foreach (var argument in expression.Arguments)
                arguments.Add((BoundExpression)argument.Accept(this));

            return new BoundNewExpression(expression, reference, arguments, CurrentScope);
        }
        protected override BoundSyntaxNode VisitReference(ReferenceExpression expression)
        {
            var references = new List<BoundExpression>();
            var index = 0;
            
            foreach(var reference in expression.References)
            {
                if (index > 0)
                {
                    // TODO(Dan): We need to re-bind the declaration here... to update it now we should be in a full decl pass.
                    var declaration = _referenceDeclarationLocator.DeclarationFor(references[index - 1]);
            
                    var scope = declaration?.Scope;
            
                    if (scope == null)
                        break;
            
                    _scopes.Push(scope);
                }
            
                var boundReference = (BoundExpression)reference.Accept(this);
            
                references.Add(boundReference);
            
                if (index > 0)
                   _scopes.Pop();
            
                index++;
            }
            // NOTE(Dan): We only want to do the first reference here.
            //            This is so that we can check the initial variable is declared
            //            Then after type inference and during type checking we can chase through the types
            //            for the references checking that the members are available and of the correct type.
            //var boundFirstReference = (BoundExpression)expression.References.First().Accept(this);

            //references.Add(boundFirstReference);
            var type = _referenceTypeLocator.TypeFor(references.First());

            return new BoundReferenceExpression(expression, references, type, CurrentScope);
        }
        protected override BoundSyntaxNode VisitUserDefinedType(UserDefinedTypeExpression userdefinedTypeExpression)
        {
            // TODO(Dan): If we are in Stack.lang and have imported System, we cannot currently find Array<T> because the CurrentScope doesn't
            //            have granular enough information, we only have "System" or "System.Collections" at that level.
            //            
            //            We may need to think about making this a fast lookup in future, but for now just do the simple naive thing.
            if (!CurrentScope.TryGetValue(userdefinedTypeExpression.Name, out Symbol symbol))
            {
                if (_mode == SyntaxBindingMode.Full)
                    AddError($"Could not find type '{userdefinedTypeExpression.Name}', are you missing an import statement?", userdefinedTypeExpression.FilePart);
            }
            
            var typeExpression = new BoundUserDefinedTypeExpression(userdefinedTypeExpression, symbol, CurrentScope);

            CurrentScope.AddOrUpdate(new Symbol(userdefinedTypeExpression.Name, null));

            return typeExpression;
        }
        protected override BoundSyntaxNode VisitGenericConstraintType(GenericConstraintTypeExpression genericConstraintTypeExpression)
        {
            //AddError("Not implemented yet!", genericConstraintTypeExpression.FilePart);

            if (!CurrentScope.TryGetValue(genericConstraintTypeExpression.Name, out Symbol symbol))
            {
                CurrentScope.AddOrUpdate(new Symbol(genericConstraintTypeExpression.Name, null));

                if (_mode == SyntaxBindingMode.Full)
                    AddError($"Could not find type '{genericConstraintTypeExpression.Name}', are you missing an import statement?", genericConstraintTypeExpression.FilePart);
            }

            // TODO(Dan): Need to add this type to the current scope so it can be looked up and inferred later in the pipeline!
            var typeExpression = new BoundGenericConstraintTypeExpression(genericConstraintTypeExpression, symbol, CurrentScope);

            return typeExpression;
        }
        protected override BoundSyntaxNode VisitPredefinedType(PredefinedTypeExpression predfinedTypeExpression)
        {
            // TODO(Dan): This is just flat out broken, we never actually fill in the delcaration for any symbol!
            //            What was I thinking when I created this?!
            var typeName = BuiltInTypeNameAlias.From(predfinedTypeExpression);

            if (!CurrentScope.TryGetValue(typeName, out Symbol symbol))
            {
                if (_mode == SyntaxBindingMode.Full)
                    AddError($"Could not find type '{predfinedTypeExpression.Name}', are you missing an import statement?", predfinedTypeExpression.FilePart);
            }

            if (_predefinedTypeMap.ContainsKey(typeName))
            {
                var cachedType = _predefinedTypeMap[typeName];

                if (cachedType == null || cachedType?.Declaration == null)
                    _predefinedTypeMap[typeName] = new BoundPredefinedTypeExpression(predfinedTypeExpression, symbol, CurrentScope);

                return _predefinedTypeMap[typeName];
            }
                
            var type = new BoundPredefinedTypeExpression(predfinedTypeExpression, symbol, CurrentScope);

            _predefinedTypeMap[typeName] = type;

            return type;
        }
        protected override BoundSyntaxNode VisitInferredType(InferredTypeExpression inferredTypeExpression)
        {
            // TODO(Dan): Add to current scope!
            CurrentScope.AddOrUpdate(new Symbol(inferredTypeExpression.Name, null));

            return new BoundInferredTypeExpression(inferredTypeExpression, CurrentScope);
        }
        protected override BoundSyntaxNode VisitUnary(UnaryExpression expression)
        {
            var argument = (BoundExpression)expression.Argument.Accept(this);

            return new BoundUnaryExpression(expression, argument, CurrentScope);
        }

        protected override BoundSyntaxNode VisitClass(ClassDeclaration classDeclaration)
        {
            CurrentScope.TryGetValue(classDeclaration.Name, out Symbol symbol);

            var existingModule = symbol?.Declaration as BoundClassDeclaration;

            var boundFields = new List<BoundFieldDeclaration>();
            var boundProperties = new List<BoundPropertyDeclaration>();
            var boundMethods = new List<BoundMethodDeclaration>();
            var boundConstructors = new List<BoundConstructorDeclaration>();
            var boundGenericParameters = new List<BoundTypeExpression>();
            var boundInheritors = new List<BoundIdentifierExpression>();

            var scope = new Scope(CurrentScope);

            if (existingModule?.Scope != null)
                existingModule.Scope.CopyTo(_errorSink, scope);

            _scopes.Push(scope);

            foreach (var parameter in classDeclaration.GenericParameters)
                boundGenericParameters.Add((BoundTypeExpression)parameter.Accept(this));

            foreach (var field in classDeclaration.Fields)
                boundFields.Add((BoundFieldDeclaration)field.Accept(this));

            foreach (var property in classDeclaration.Properties)
                boundProperties.Add((BoundPropertyDeclaration)property.Accept(this));

            foreach (var method in classDeclaration.Methods)
                boundMethods.Add((BoundMethodDeclaration)method.Accept(this));

            foreach (var constructor in classDeclaration.Constructors)
                boundConstructors.Add((BoundConstructorDeclaration)constructor.Accept(this));

            var boundClass = new BoundClassDeclaration(classDeclaration, boundGenericParameters, boundFields, boundProperties, boundMethods, boundConstructors, CurrentScope);

            _scopes.Pop();

            CurrentScope.AddOrUpdate(new Symbol(boundClass.Name, boundClass));

            return boundClass;
        }
        protected override BoundSyntaxNode VisitConstructor(ConstructorDeclaration constructorDeclaration)
        {
            // TODO(Dan): I want to allow overloading, so will need to figure out a reasonable way to prevent name collisions
            if (CurrentScope.TryGetValue(constructorDeclaration.Name, out Symbol symbol))
            {
                if (_mode == SyntaxBindingMode.DeclarationOnly)
                {
                    AddError($"Cannot redeclare identifier '{constructorDeclaration.Name}'", constructorDeclaration.FilePart);
                    return new BoundConstructorDeclaration(constructorDeclaration, Enumerable.Empty<BoundParameterDeclaration>(), CurrentScope);
                }
            }

            var currentScope = CurrentScope;
            _scopes.Push(new Scope(CurrentScope));

            var parameters = new List<BoundParameterDeclaration>();

            foreach (var item in constructorDeclaration.Parameters)
                parameters.Add(item.Accept(this) as BoundParameterDeclaration);

            if (_mode == SyntaxBindingMode.DeclarationOnly)
            {
                _scopes.Pop();

                var declaration = new BoundConstructorDeclaration(constructorDeclaration, parameters, currentScope);
                CurrentScope.AddOrUpdate(new Symbol(constructorDeclaration.Name, declaration));
                return declaration;
            }

            var boundBody = (BoundBlockStatement)constructorDeclaration.Body.Accept(this);
            var boundDelcaration = new BoundConstructorDeclaration(constructorDeclaration, parameters, boundBody, currentScope);

            _scopes.Pop();

            CurrentScope.AddOrUpdate(new Symbol(constructorDeclaration.Name, boundDelcaration));

            return boundDelcaration;
        }
        protected override BoundSyntaxNode VisitEnum(EnumDeclaration enumDeclaration)
        {
            // TODO(Dan): Need to consider when to auto calculate the enum values is they are not defined!
            if (CurrentScope.TryGetValue(enumDeclaration.Name, out Symbol symbol))
            {
                if (_mode == SyntaxBindingMode.DeclarationOnly)
                { 
                    AddError($"A member with this name already exists in the current context '{enumDeclaration.Name}'", enumDeclaration.FilePart);
                    return new BoundEnumDeclaration(enumDeclaration, Enumerable.Empty<BoundEnumMemberDeclaration>(), CurrentScope);
                }
            }

            var boundMembers = new List<BoundEnumMemberDeclaration>();

            if (_mode == SyntaxBindingMode.DeclarationOnly)
            {
                var @enum = new BoundEnumDeclaration(enumDeclaration, Enumerable.Empty<BoundEnumMemberDeclaration>(), CurrentScope);
                CurrentScope.AddOrUpdate(new Symbol(@enum.Name, @enum));
                return @enum;
            }
            
            foreach (var members in enumDeclaration.Members)
                boundMembers.Add((BoundEnumMemberDeclaration)members.Accept(this));

            var boundEnum = new BoundEnumDeclaration(enumDeclaration, boundMembers, CurrentScope);

            CurrentScope.AddOrUpdate(new Symbol(boundEnum.Name, boundEnum));

            return boundEnum;
        }
        protected override BoundSyntaxNode VisitEnumMember(EnumMemberDeclaration enumMemberDeclaration)
        {
            if (CurrentScope.TryGetValue(enumMemberDeclaration.Name, out Symbol symbol))
            {
                AddError($"Enum already defines a member called '{enumMemberDeclaration.Name}'", enumMemberDeclaration.FilePart);
            }

            var boundValue = (BoundExpression)enumMemberDeclaration.Value?.Accept(this);

            return new BoundEnumMemberDeclaration(enumMemberDeclaration, boundValue, CurrentScope);
        }
        protected override BoundSyntaxNode VisitField(FieldDeclaration fieldDeclaration)
        {
            if (CurrentScope.TryGetValue(fieldDeclaration.Name, out Symbol symbol))
            {
                if (_mode == SyntaxBindingMode.DeclarationOnly)
                { 
                    AddError($"Cannot redeclare identifier '{fieldDeclaration.Name}'", fieldDeclaration.FilePart);
                    return new BoundFieldDeclaration(fieldDeclaration, CurrentScope);
                }
            }

            var boundType = (BoundTypeExpression)fieldDeclaration.Type.Accept(this);

            if (_mode == SyntaxBindingMode.DeclarationOnly)
            {
                var field = new BoundFieldDeclaration(fieldDeclaration, boundType, CurrentScope);
                CurrentScope.AddOrUpdate(new Symbol(field.Name, field));
                return field;
            }

            var boundValue = (BoundExpression)fieldDeclaration.DefaultValue?.Accept(this);
            var boundField = new BoundFieldDeclaration(fieldDeclaration, boundType, boundValue, CurrentScope);

            CurrentScope.AddOrUpdate(new Symbol(boundField.Name, boundField));

            return boundField;
        }
        protected override BoundSyntaxNode VisitInterface(InterfaceDeclaration interfaceDeclaration)
        {
            if (CurrentScope.TryGetValue(interfaceDeclaration.Name, out Symbol symbol))
            {
                if (_mode == SyntaxBindingMode.DeclarationOnly)
                {
                    AddError($"Cannot redeclare identifier '{interfaceDeclaration.Name}'", interfaceDeclaration.FilePart);
                    return new BoundInterfaceDeclaration(interfaceDeclaration, null, null, CurrentScope);
                }
            }

            _scopes.Push(new Scope(CurrentScope));

            var existingMode = _mode;
            _mode = SyntaxBindingMode.DeclarationOnly;

            var boundProperties = new List<BoundPropertyDeclaration>();
            var boundMethods = new List<BoundMethodDeclaration>();

            foreach (var property in interfaceDeclaration.Properties)
                boundProperties.Add((BoundPropertyDeclaration)property.Accept(this));

            foreach (var method in interfaceDeclaration.Methods)
                boundMethods.Add((BoundMethodDeclaration)method.Accept(this));

            _mode = existingMode;

            var boundInterface = new BoundInterfaceDeclaration(interfaceDeclaration, boundProperties, boundMethods, CurrentScope);

            _scopes.Pop();

            CurrentScope.AddOrUpdate(new Symbol(boundInterface.Name, boundInterface));

            return boundInterface;
        }
        protected override BoundSyntaxNode VisitMethod(MethodDeclaration methodDeclaration)
        {
            // TODO(Dan): I want to allow overloading, so will need to figure out a reasonable way to prevent name collisions
            if (CurrentScope.TryGetValue(methodDeclaration.Name, out Symbol symbol))
            {
                if (_mode == SyntaxBindingMode.DeclarationOnly)
                {
                    AddError($"Cannot redeclare identifier '{methodDeclaration.Name}'", methodDeclaration.FilePart);
                    return new BoundMethodDeclaration(methodDeclaration, Enumerable.Empty<BoundParameterDeclaration>(), CurrentScope);
                }
            }

            var currentScope = CurrentScope;
            _scopes.Push(new Scope(CurrentScope));

            var boundType = (BoundTypeExpression)methodDeclaration.ReturnType.Accept(this);
            var boundGenericConstrains = new List<BoundTypeExpression>();
            var parameters = new List<BoundParameterDeclaration>();

            foreach (var item in methodDeclaration.GenericTypeConstraints)
            {
                var boundItem = item.Accept(this) as BoundTypeExpression;

                if (CurrentScope.TryGetValue(boundItem.Name, out Symbol match))
                    if (_mode == SyntaxBindingMode.Full)
                        AddWarning($"Type parameter '{boundItem.Name}' has the same name as the type parameter from it's enclosing type.", item.FilePart);

                boundGenericConstrains.Add(boundItem);
            }

            foreach (var item in methodDeclaration.Parameters)
                parameters.Add(item.Accept(this) as BoundParameterDeclaration);



            if (_mode == SyntaxBindingMode.DeclarationOnly)
            {
                var declaration = new BoundMethodDeclaration(methodDeclaration, parameters, boundType, currentScope);

                _scopes.Pop();

                CurrentScope.AddOrUpdate(new Symbol(declaration.Name, declaration));

                return declaration;
            }

            // TODO(Dan): Figure out if we should try and bind the return type (i.e. it potentially could be generic so can't do that until it's been inferred!)
            //var boundType = (BoundTypeExpression)methodDeclaration.ReturnType.Accept(this);
            var boundBody = (BoundBlockStatement)methodDeclaration.Body.Accept(this);
            var boundDelcaration = new BoundMethodDeclaration(methodDeclaration, parameters, boundType, boundBody, currentScope);

            _scopes.Pop();

            CurrentScope.AddOrUpdate(new Symbol(boundDelcaration.Name, boundDelcaration));

            return boundDelcaration;
        }
        protected override BoundSyntaxNode VisitModuleDeclaration(ModuleDeclaration moduleDeclaration)
        {
            CurrentScope.TryGetValue(moduleDeclaration.Name, out Symbol symbol);

            var existingModule = symbol?.Declaration as BoundModuleDeclaration;

            var classes = existingModule?.Classes ?? new Dictionary<string, BoundClassDeclaration>();
            var interfaces = existingModule?.Interfaces ?? new Dictionary<string, BoundInterfaceDeclaration>();
            var methods = existingModule?.Methods ?? new Dictionary<string, BoundMethodDeclaration>();
            var enums = existingModule?.Enums ?? new Dictionary<string, BoundEnumDeclaration>();

            var scope = new Scope(CurrentScope);

            if (existingModule?.Scope != null)
                existingModule?.Scope.CopyTo(_errorSink, scope);

            _scopes.Push(scope);

            foreach (var @class in moduleDeclaration.Classes)
            {
                var boundClass = (BoundClassDeclaration)@class.Accept(this);

                classes[@class.Name] = boundClass;
            }

            foreach (var @interface in moduleDeclaration.Interfaces)
            {
                var boundInterface = (BoundInterfaceDeclaration)@interface.Accept(this);

                interfaces[@interface.Name] = boundInterface;
            }

            foreach (var method in moduleDeclaration.Methods)
            {
                var boundMethod = (BoundMethodDeclaration)method.Accept(this);

                methods[method.Name] = boundMethod;
            }

            foreach (var @enum in moduleDeclaration.Enums)
            {
                var boundEnum = (BoundEnumDeclaration)@enum.Accept(this);

                enums[@enum.Name] = boundEnum;
            }

            var module = new BoundModuleDeclaration(moduleDeclaration, ref classes, ref interfaces, ref methods, ref enums, scope);

            _scopes.Pop();

            CurrentScope.Parent.AddOrUpdate(new Symbol(module.Name, module));

            return module;
        }
        protected override BoundSyntaxNode VisitParameter(ParameterDeclaration parameterDeclaration)
        {
            if (CurrentScope.TryGetValue(parameterDeclaration.Name, out Symbol symbol))
            {
                if (symbol.Declaration is BoundParameterDeclaration && (_mode == SyntaxBindingMode.DeclarationOnly))
                {
                    AddError($"Cannot redeclare identifier '{parameterDeclaration.Name}'", parameterDeclaration.FilePart);
                    return new BoundParameterDeclaration(parameterDeclaration, CurrentScope);
                }
            }

            var type = (BoundTypeExpression)parameterDeclaration.Type.Accept(this);
            var boundParameter = new BoundParameterDeclaration(parameterDeclaration, type, CurrentScope);

            CurrentScope.AddOrUpdate(new Symbol(boundParameter.Name, boundParameter));

            return boundParameter;
        }
        protected override BoundSyntaxNode VisitProperty(PropertyDeclaration propertyDeclaration)
        {
            if (CurrentScope.TryGetValue(propertyDeclaration.Name, out Symbol symbol))
            {
                if (_mode == SyntaxBindingMode.DeclarationOnly)
                {
                    AddError($"Cannot redeclare identifier '{propertyDeclaration.Name}'", propertyDeclaration.FilePart);
                    return new BoundPropertyDeclaration(propertyDeclaration, null, null, CurrentScope);
                }
            }

            var boundType = (BoundTypeExpression)propertyDeclaration.Type.Accept(this);

            if (_mode == SyntaxBindingMode.DeclarationOnly)
            {
                var property = new BoundPropertyDeclaration(propertyDeclaration, boundType, null, null, CurrentScope);
                CurrentScope.AddOrUpdate(new Symbol(property.Name, property));
                return property;
            }

            // TODO(Dan): Need to figure out when is best to generate the auto property methods...
            
            var boundGetter = (BoundMethodDeclaration)propertyDeclaration.Getter?.Accept(this);
            var boundSetter = (BoundMethodDeclaration)propertyDeclaration.Setter?.Accept(this);

            Symbol getterSymbol = null;
            Symbol setterSymbol = null;

            if (boundGetter != null)
                CurrentScope.TryGetValue(boundGetter.Name, out getterSymbol);

            if (boundSetter != null)
                CurrentScope.TryGetValue(boundSetter.Name, out setterSymbol);

            var boundProperty = new BoundPropertyDeclaration(propertyDeclaration, boundType, getterSymbol, setterSymbol, CurrentScope);

            CurrentScope.AddOrUpdate(new Symbol(boundProperty.Name, boundProperty));

            return boundProperty;
        }
        protected override BoundSyntaxNode VisitVariable(VariableDeclaration variableDeclaration)
        {
            if (CurrentScope.TryGetValue(variableDeclaration.Name, out Symbol symbol))
            {
                if (symbol.Declaration is BoundParameterDeclaration || symbol.Declaration is BoundVariableDeclaration)
                    AddError($"Cannot redeclare identifier '{variableDeclaration.Name}'", variableDeclaration.FilePart);
            }

            var boundType = (BoundTypeExpression)variableDeclaration.Type.Accept(this);
            var boundValue = (BoundExpression)variableDeclaration.Value.Accept(this);
            var boundVariableDeclaration = new BoundVariableDeclaration(variableDeclaration, boundType, boundValue, CurrentScope);

            CurrentScope.AddOrUpdate(new Symbol(variableDeclaration.Name, boundVariableDeclaration));

            return boundVariableDeclaration;
        }

        protected override BoundSyntaxNode VisitBlock(BlockStatement statement)
        {
            var currentScope = CurrentScope;

            var contents = new List<BoundSyntaxNode>();

            foreach (var item in statement.Contents)
                contents.Add(item.Accept(this));

            return new BoundBlockStatement(statement, contents, currentScope);
        }
        protected override BoundSyntaxNode VisitBreak(BreakStatement statement)
        {
            return new BoundBreakStatement(statement, CurrentScope);
        }
        protected override BoundSyntaxNode VisitCase(CaseStatement statement)
        {
            var boundCases = new List<BoundExpression>();

            foreach (var @case in statement.Cases)
                boundCases.Add((BoundExpression)@case.Accept(this));

            _scopes.Push(new Scope(CurrentScope));

            var boundBody = (BoundBlockStatement)statement.Body.Accept(this);

            _scopes.Pop();

            return new BoundCaseStatement(statement, boundCases, boundBody, CurrentScope);
        }
        protected override BoundSyntaxNode VisitContinue(ContinueStatement statement)
        {
            return new BoundContinueStatement(statement, CurrentScope);
        }
        protected override BoundSyntaxNode VisitElse(ElseStatement statement)
        {
            _scopes.Push(new Scope(CurrentScope));

            var boundBody = (BoundBlockStatement)statement.Body.Accept(this);

            _scopes.Pop();

            return new BoundElseStatement(statement, boundBody, CurrentScope);
        }
        protected override BoundSyntaxNode VisitEmpty(EmptyStatement statement)
        {
            return new BoundEmptyStatement(statement, CurrentScope);
        }
        protected override BoundSyntaxNode VisitFor(ForStatement statement)
        {
            var boundInitializer = statement.Initialiser?.Accept(this);
            var boundCondition = (BoundExpression)statement.Condition?.Accept(this);
            var boundIncrement = (BoundExpression)statement.Increment?.Accept(this);

            _scopes.Push(new Scope(CurrentScope));

            var boundBody = (BoundBlockStatement)statement.Body.Accept(this);

            _scopes.Pop();

            return new BoundForStatement(statement, boundInitializer, boundCondition, boundIncrement, boundBody, CurrentScope);
        }
        protected override BoundSyntaxNode VisitIf(IfStatement statement)
        {
            var boundPredicate = (BoundExpression)statement.Predicate.Accept(this);

            _scopes.Push(new Scope(CurrentScope));

            var boundBody = (BoundBlockStatement)statement.Body.Accept(this);

            _scopes.Pop();

            _scopes.Push(new Scope(CurrentScope));

            var boundElseStatement = (BoundElseStatement)statement.Else?.Accept(this);

            _scopes.Pop();

            return new BoundIfStatement(statement, boundPredicate, boundBody, boundElseStatement, CurrentScope);
        }
        protected override BoundSyntaxNode VisitImport(ImportStatement statement)
        {
            //if (_mode == SyntaxBindingMode.DeclarationOnly)
            //    return new BoundImportStatement(statement, null);

            if (!CurrentScope.TryGetValue(statement.Name, out Symbol symbol))
            {
                AddError($"Cannot find module '{statement.Name}'", statement.Names.First().FilePart);
                return new BoundImportStatement(statement, null);
            }
            
            return new BoundImportStatement(statement, (BoundModuleDeclaration)symbol.Declaration);
        }
        protected override BoundSyntaxNode VisitReturn(ReturnStatement statement)
        {
            var boundValue = (BoundExpression)statement.Value?.Accept(this);

            return new BoundReturnStatement(statement, boundValue, CurrentScope);
        }
        protected override BoundSyntaxNode VisitSwitch(SwitchStatement statement)
        {
            var boundCondition = (BoundExpression)statement.Condition.Accept(this);

            var boundCases = new List<BoundCaseStatement>();

            _scopes.Push(new Scope(CurrentScope));

            foreach(var @case in statement.Cases)
            { 
                var boundCase = (BoundCaseStatement)@case.Accept(this);
                boundCases.Add(boundCase);
            }

            _scopes.Pop();

            return new BoundSwitchStatement(statement, boundCondition, boundCases, CurrentScope);
        }
        protected override BoundSyntaxNode VisitWhile(WhileStatement statement)
        {
            var boundPredicate = (BoundExpression)statement.Predicate.Accept(this);

            _scopes.Push(new Scope(CurrentScope));

            var boundBody = (BoundBlockStatement)statement.Body.Accept(this);

            _scopes.Pop();

            return new BoundWhileStatement(statement, boundPredicate, boundBody, CurrentScope);
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

        public SyntaxBinder(SyntaxBindingMode mode, ErrorSink errorSink)
        {
            _mode = mode;
            _scopes = new Stack<Scope>();
            _referenceDeclarationLocator = new ReferenceDeclarationLocator();
            _referenceTypeLocator = new ReferenceTypeLocator();
            _errorSink = errorSink;
        }
    }
}
