using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Compiler.Parsing.Syntax;
using Compiler.Parsing.Syntax.Declarations;
using Compiler.Parsing.Syntax.Expressions;
using Compiler.Parsing.Syntax.Statements;
using Compiler.Semantics.BoundSyntax;
using Compiler.Semantics.BoundSyntax.Declarations;
using Compiler.Semantics.BoundSyntax.Expressions;
using Compiler.Semantics.BoundSyntax.Expressions.Types;
using Compiler.Semantics.BoundSyntax.Statements;
using Compiler.Semantics.Symbols;
using Compiler.Semantics.Types;

namespace Compiler.Semantics.Passes.Types.Inference
{
    internal class TypeInferencePass : BoundSyntaxVisitor<BoundSyntaxNode>, ISemanticPass
    {
        private ErrorSink _errorSink;
        private BoundCompilationRoot _compilationRoot;

        public bool ShouldContinue => !_errorSink.HasErrors;

        public void Run(ref BoundCompilationRoot compilationRoot)
        {
            if (compilationRoot == null)
                throw new ArgumentNullException(nameof(compilationRoot));

            _compilationRoot = compilationRoot;

            foreach (var compilationUnit in compilationRoot.CompilationUnits)
                foreach(var module in compilationUnit.Modules)
                    module.Accept(this);
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

        protected override BoundSyntaxNode VisitEnumMember(BoundEnumMemberDeclaration enumMemberDeclaration)
        {
            return enumMemberDeclaration;
        }
        protected override BoundSyntaxNode VisitEnum(BoundEnumDeclaration enumDeclaration)
        {
            return enumDeclaration;
        }
        protected override BoundSyntaxNode VisitInterface(BoundInterfaceDeclaration interfaceDeclaration)
        {
            return interfaceDeclaration;
        }
        protected override BoundSyntaxNode VisitArithmetic(BoundBinaryExpression expression)
        {
            var left = expression.Left.Accept(this);
            var right = expression.Right.Accept(this);

            // TODO(Dan): We need to actually infer the types here...

            return new BoundBinaryExpression(expression.SyntaxNode<BinaryExpression>(), (BoundExpression)left, (BoundExpression)right, expression.Scope);
        }
        protected override BoundSyntaxNode VisitArrayAccess(BoundArrayAccessExpression expression)
        {
            throw new NotImplementedException();
        }
        protected override BoundSyntaxNode VisitAssignment(BoundBinaryExpression expression)
        {
            var left = (BoundExpression)expression.Left.Accept(this);
            var right = (BoundExpression)expression.Right.Accept(this);

            // TODO(Dan): Move to the type checking pass, we only want to be inferring types here...
            //if (left.Type.Name != right.Type.Name)
            //  AddError($"Cannot assign type '{right.Type.Name}' to '{left.Type.Name}'", right.SyntaxNode<SyntaxNode>().FilePart);

            return new BoundBinaryExpression(expression.SyntaxNode<BinaryExpression>(), left, right, right.Type, expression.Scope);
        }
        protected override BoundSyntaxNode VisitBitwise(BoundBinaryExpression expression)
        {
            throw new NotImplementedException();
        }
        protected override BoundSyntaxNode VisitBlock(BoundBlockStatement statement)
        {
            var contents = new List<BoundSyntaxNode>();

            foreach (var item in statement.Contents)
                contents.Add(item.Accept(this));

            return new BoundBlockStatement(statement.SyntaxNode<BlockStatement>(), contents, statement.Scope);
        }
        protected override BoundSyntaxNode VisitImport(BoundImportStatement statement)
        {
            throw new NotImplementedException();
        }
        protected override BoundSyntaxNode VisitBreak(BoundBreakStatement statement)
        {
            throw new NotImplementedException();
        }
        protected override BoundSyntaxNode VisitCase(BoundCaseStatement statement)
        {
            throw new NotImplementedException();
        }
        protected override BoundSyntaxNode VisitClass(BoundClassDeclaration classDeclaration)
        {
            var fields = new List<BoundFieldDeclaration>();
            var constructors = new List<BoundConstructorDeclaration>();
            var methods = new List<BoundMethodDeclaration>();
            var properties = new List<BoundPropertyDeclaration>();
            var genericTypeConstraints = new List<BoundTypeExpression>();

            foreach (var parameter in classDeclaration.GenericParameters)
                genericTypeConstraints.Add((BoundTypeExpression)parameter.Accept(this));

            foreach (var field in classDeclaration.Fields)
                fields.Add((BoundFieldDeclaration)field.Accept(this));

            foreach (var constructor in classDeclaration.Constructors)
                constructors.Add((BoundConstructorDeclaration)constructor.Accept(this));
            
            foreach (var method in classDeclaration.Methods)
                methods.Add((BoundMethodDeclaration)method.Accept(this));
            
            foreach (var property in classDeclaration.Properties)
                properties.Add((BoundPropertyDeclaration)property.Accept(this));
            
            return new BoundClassDeclaration(classDeclaration.SyntaxNode<ClassDeclaration>(), classDeclaration.GenericParameters, fields, properties, methods, constructors, classDeclaration.Scope);
        }
        protected override BoundSyntaxNode VisitConstant(BoundConstantExpression expression)
        {
            switch (expression.ConstantType)
            {
                case ConstantType.Boolean:
                    return new BoundConstantExpression(expression.SyntaxNode<ConstantExpression>(), _compilationRoot.PredefinedTypeMap["Bool"], expression.Scope);
                case ConstantType.Integer:
                    return new BoundConstantExpression(expression.SyntaxNode<ConstantExpression>(), _compilationRoot.PredefinedTypeMap["Int32"], expression.Scope);
                case ConstantType.Real:
                    return new BoundConstantExpression(expression.SyntaxNode<ConstantExpression>(), _compilationRoot.PredefinedTypeMap["Float"], expression.Scope);
                case ConstantType.String:
                    return new BoundConstantExpression(expression.SyntaxNode<ConstantExpression>(), _compilationRoot.PredefinedTypeMap["String"], expression.Scope);
            }

            throw new NotImplementedException();
        }
        protected override BoundSyntaxNode VisitConstructor(BoundConstructorDeclaration constructorDeclaration)
        {
            var parameters = new List<BoundParameterDeclaration>();

            foreach (var parameter in constructorDeclaration.Parameters)
                parameters.Add((BoundParameterDeclaration)parameter.Accept(this));

            var body = constructorDeclaration.Body.Accept(this);

            return new BoundConstructorDeclaration(constructorDeclaration.SyntaxNode<ConstructorDeclaration>(), parameters, constructorDeclaration.Scope);
        }
        protected override BoundSyntaxNode VisitContinue(BoundContinueStatement statement)
        {
            throw new NotImplementedException();
        }
        protected override BoundSyntaxNode VisitElse(BoundElseStatement statement)
        {
            throw new NotImplementedException();
        }
        protected override BoundSyntaxNode VisitEmpty(BoundEmptyStatement statement)
        {
            return _compilationRoot.PredefinedTypeMap["Void"];
        }
        protected override BoundSyntaxNode VisitField(BoundFieldDeclaration fieldDeclaration)
        {
            var defaultValue = (BoundExpression)fieldDeclaration.DefaultValue?.Accept(this);
            var fieldType = (BoundTypeExpression)fieldDeclaration.Type.Accept(this);

            //if (defaultValueType != null && defaultValueType.Declaration.Name != fieldType.Declaration.Name)
            //{
            //    AddError($"Type mismatch '{defaultValueType.Declaration.Name}' is not assignable to '{fieldType.Declaration.Name}'", fieldDeclaration.FilePart);
            //}

            return new BoundFieldDeclaration(fieldDeclaration.SyntaxNode<FieldDeclaration>(), fieldType, defaultValue, fieldDeclaration.Scope);
        }
        protected override BoundSyntaxNode VisitFor(BoundForStatement statement)
        {
            var init = statement.Initialiser?.Accept(this);
            //var condition = statement.Condition?.Accept(this);
            //var increment = statement.Increment?.Accept(this);

            statement.Body.Accept(this);

            return statement;            
        }
        protected override BoundSyntaxNode VisitIdentifier(BoundIdentifierExpression expression)
        {
            var declaration = (BoundDeclaration)expression.Declaration.Declaration.Accept(this);

            return new BoundIdentifierExpression(expression.SyntaxNode<IdentifierExpression>(), expression.Declaration, declaration.Type, expression.Scope);
        }
        protected override BoundSyntaxNode VisitIf(BoundIfStatement statement)
        {
            var predicate = (BoundExpression)statement.Predicate.Accept(this);
            var body = (BoundBlockStatement)statement.Body.Accept(this);
            var @else = (BoundElseStatement)statement.Else?.Accept(this);

            return new BoundIfStatement(statement.SyntaxNode<IfStatement>(), predicate, body, @else, statement.Scope);
        }
        protected override BoundSyntaxNode VisitLambda(BoundLambdaExpression expression)
        {
            throw new NotImplementedException();
        }
        protected override BoundSyntaxNode VisitLogical(BoundBinaryExpression expression)
        {
            var left = (BoundExpression)expression.Left.Accept(this);
            var right = (BoundExpression)expression.Right.Accept(this);
            // TODO(Dan): Need to figure out what operations are allowed for the various types etc etc.
            //if (left.Declaration.Name != right.Declaration.Name)
            //{
            //    AddError($"Type mismatch '{right.Declaration.Name}' is not assignable to '{left.Declaration.Name}'", part);
            //}

            return new BoundBinaryExpression(expression.SyntaxNode<BinaryExpression>(), left, right, _compilationRoot.PredefinedTypeMap["Bool"], expression.Scope);
        }
        protected override BoundSyntaxNode VisitMethod(BoundMethodDeclaration methodDeclaration)
        {
            var parameters = new List<BoundParameterDeclaration>();
            var returnType = (BoundTypeExpression)methodDeclaration.ReturnType.Accept(this);
            // TODO(Dan): Need to potentially visit this.
            var genericTypeParameters = methodDeclaration.GenericTypeParameters;
            
            foreach (var parameter in methodDeclaration.Parameters)
                parameters.Add((BoundParameterDeclaration)parameter.Accept(this));
            
            var body = (BoundBlockStatement)methodDeclaration.Body.Accept(this);

            return new BoundMethodDeclaration(methodDeclaration.SyntaxNode<MethodDeclaration>(), parameters, genericTypeParameters, returnType, body, methodDeclaration.Scope);
        }
        protected override BoundSyntaxNode VisitUserDefinedType(BoundUserDefinedTypeExpression userdefinedTypeExpression)
        {
            return userdefinedTypeExpression;
        }
        protected override BoundSyntaxNode VisitBoundGenericConstraintType(BoundGenericConstraintTypeExpression genericConstraintTypeExpression)
        {
            return genericConstraintTypeExpression;
        }
        protected override BoundSyntaxNode VisitPredefinedType(BoundPredefinedTypeExpression predefinedTypeExpression)
        {
            return _compilationRoot.PredefinedTypeMap[BuiltInTypeNameAlias.From(predefinedTypeExpression)];
        }
        protected override BoundSyntaxNode VisitInferredType(BoundInferredTypeExpression inferredTypeExpression)
        {
            // TODO(Dan): Need to implement this so we can figure it out...
            return inferredTypeExpression;
        }

        protected override BoundSyntaxNode VisitMethodCall(BoundMethodCallExpression expression)
        {
            var arguments = new List<BoundExpression>();
            var genericTypeParameters = new List<BoundTypeExpression>();

            var type = (BoundTypeExpression)expression.Type.Accept(this);
            var reference = (BoundExpression)expression.Reference.Accept(this);
            var declaration = ((BoundIdentifierExpression)reference).Declaration.Declaration;
            var methodDeclaration = (BoundMethodDeclaration)declaration;

            foreach (var argument in expression.Arguments)
                arguments.Add((BoundExpression)argument.Accept(this));

            foreach (var genericParameter in expression.GenericTypeParameters)
                genericTypeParameters.Add((BoundTypeExpression)genericParameter.Accept(this));

            // TODO(Dan): Need to match up the inferred types to their callee site types.
            var parameters = methodDeclaration.Parameters.ToList();


            if (genericTypeParameters.Any())
            {
                // TODO(Dan): From the declaration, get the generic type parameters.
                var generics = methodDeclaration.GenericTypeParameters.ToList();
                var temp = new List<(int, BoundExpression)>();

                // TODO(Dan): From the declaration, get the generic type argument positions.
                var args = methodDeclaration.Parameters.ToList();

                for (var i = 0; i < args.Count; i++)
                {
                    var arg = args[i];

                    if (arg.Type is BoundInferredTypeExpression inferredTypeExpression)
                    {
                        var name = inferredTypeExpression.Name;
                        var genericParamIndex = 0;

                        for (genericParamIndex = 0; genericParamIndex < generics.Count; genericParamIndex++)
                        {
                            var generic = generics[genericParamIndex];

                            if (name == generic.Type.Name)
                                break;
                        }

                        temp.Add((genericParamIndex, arguments[i]));
                    }
                }



                // TODO(Dan): 
            }
            // TODO(Dan): Check we don't OOB.
            for (var i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];
                var argument = arguments[i];

                var matchingGenericTypeParameter = genericTypeParameters.FirstOrDefault(t => t.Name == parameter.Type.Name);

                if (matchingGenericTypeParameter == null)
                {
                    // TODO(Dan): The type for T has been explicitly supplied!?
                    //matchingGenericTypeParameter = 
                }

                if (parameter.Type is BoundInferredTypeExpression boundInferredType)
                {
                    if (!(argument.Type is BoundInferredTypeExpression))
                    {
                        // TODO(Dan): Substitute T for the arguments type if we can.

                    }
                }
            }

            return new BoundMethodCallExpression(expression.SyntaxNode<MethodCallExpression>(), reference, arguments, genericTypeParameters, type, expression.Scope);
        }
        protected override BoundSyntaxNode VisitNew(BoundNewExpression expression)
        {
            var arguments = new List<BoundExpression>();
            var reference = (BoundTypeExpression)expression.Reference.Accept(this);

            foreach (var argument in expression.Arguments)
                arguments.Add((BoundExpression)argument.Accept(this));

            return new BoundNewExpression(expression.SyntaxNode<NewExpression>(), reference, arguments, expression.Scope);
        }
        protected override BoundSyntaxNode VisitModuleDeclaration(BoundModuleDeclaration moduleDeclaration)
        {
            foreach (var @class in moduleDeclaration.Classes.Values)
                @class.Accept(this);
            
            foreach (var @interface in moduleDeclaration.Interfaces.Values)
                @interface.Accept(this);
            
            foreach (var method in moduleDeclaration.Methods.Values)
                method.Accept(this);
            
            foreach (var @enum in moduleDeclaration.Enums.Values)
                @enum.Accept(this);

            return moduleDeclaration;
        }
        protected override BoundSyntaxNode VisitParameter(BoundParameterDeclaration parameterDeclaration)
        {
            var type = (BoundTypeExpression)parameterDeclaration.Type.Accept(this);

            return new BoundParameterDeclaration(parameterDeclaration.SyntaxNode<ParameterDeclaration>(), type, parameterDeclaration.Scope);
        }
        protected override BoundSyntaxNode VisitProperty(BoundPropertyDeclaration propertyDeclaration)
        {
            var getter = (BoundMethodDeclaration)propertyDeclaration.Getter?.Declaration?.Accept(this);
            var setter = (BoundMethodDeclaration)propertyDeclaration.Setter?.Declaration?.Accept(this);
            var returnType = (BoundTypeExpression)propertyDeclaration.Type.Accept(this);

            Symbol getterSymbol = null;
            Symbol setterSymbol = null;

            if (getter != null)
                propertyDeclaration.Scope.TryGetValue(getter.Name, out getterSymbol);

            if (setter != null)
                propertyDeclaration.Scope.TryGetValue(setter.Name, out setterSymbol);

            return new BoundPropertyDeclaration(propertyDeclaration.SyntaxNode<PropertyDeclaration>(), returnType, getterSymbol, setterSymbol, propertyDeclaration.Scope);
        }
        protected override BoundSyntaxNode VisitReference(BoundReferenceExpression expression)
        {
            throw new NotImplementedException();
        }
        protected override BoundSyntaxNode VisitSwitch(BoundSwitchStatement statement)
        {
            throw new NotImplementedException();
        }
        protected override BoundSyntaxNode VisitUnary(BoundUnaryExpression expression)
        {
            var argument = expression.Argument.Accept(this);
            
            return argument;
        }
        protected override BoundSyntaxNode VisitVariable(BoundVariableDeclaration variableDeclaration)
        {
            var value = (BoundExpression)variableDeclaration.Value.Accept(this);
            //var type = variableDeclaration.Type.Accept(this);

            return new BoundVariableDeclaration(variableDeclaration.SyntaxNode<VariableDeclaration>(), value.Type, variableDeclaration.Value, variableDeclaration.Scope);
        }
        protected override BoundSyntaxNode VisitWhile(BoundWhileStatement statement)
        {
            var predicate = (BoundExpression)statement.Predicate.Accept(this);
            var body = (BoundBlockStatement)statement.Body.Accept(this);

            if (predicate.Type.Declaration.Name != "Bool")
            {
                AddError($"Type mismatch, got '{predicate.Type.Declaration.Name}' expecting 'bool'", statement.Predicate.SyntaxNode<Expression>().FilePart);
            }

            return new BoundWhileStatement(statement.SyntaxNode<WhileStatement>(), predicate, body, statement.Scope);
        }
        protected override BoundSyntaxNode VisitReturn(BoundReturnStatement statement)
        {
            var value = (BoundExpression)statement.Value?.Accept(this);

            return new BoundReturnStatement(statement.SyntaxNode<ReturnStatement>(), value, statement.Scope);
        }

        public TypeInferencePass(ErrorSink errorSink)
        {
            _errorSink = errorSink;
        }
    }
}
