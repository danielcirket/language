using System;
using System.Collections.Generic;
using System.Text;
using Compiler.Parsing.Syntax;
using Compiler.Parsing.Syntax.Declarations;
using Compiler.Parsing.Syntax.Expressions;
using Compiler.Parsing.Syntax.Statements;

namespace Compiler.Parsing
{
    internal abstract class SyntaxVisitor
    {
        public void Visit(SyntaxNode node)
        {
            switch (node)
            {
                case SourceDocument document:
                    VisitDocument(document);
                    break;

                case Expression expression:
                    VisitExpression(expression);
                    break;

                case Statement statement:
                    VisitStatement(statement);
                    break;

                case Declaration declaration:
                    VisitDeclaration(declaration);
                    break;
            }
        }

        protected void VisitExpression(Expression expression)
        {
            switch (expression)
            {
                case ArrayAccessExpression arrayAccessExpression:
                    VisitArrayAccess(arrayAccessExpression);
                    break;

                case BinaryExpression binaryExpression:
                    VisitBinary(binaryExpression);
                    break;

                case ConstantExpression constantExpression:
                    VisitConstant(constantExpression);
                    break;

                case IdentifierExpression identifierExpression:
                    VisitIdentifier(identifierExpression);
                    break;

                case LambdaExpression lambdaExpression:
                    VisitLambda(lambdaExpression);
                    break;

                case MethodCallExpression methodCallExpression:
                    VisitMethodCall(methodCallExpression);
                    break;

                case NewExpression newExpression:
                    VisitNew(newExpression);
                    break;

                case ReferenceExpression referenceExpression:
                    VisitReference(referenceExpression);
                    break;

                case UnaryExpression unaryExpression:
                    VisitUnary(unaryExpression);
                    break;
            }
        }
        protected void VisitBinary(BinaryExpression expression)
        {
            switch (expression.Operator)
            {
                case BinaryOperator.Add:
                case BinaryOperator.Div:
                case BinaryOperator.Mod:
                case BinaryOperator.Sub:
                case BinaryOperator.Mul:
                    VisitArithmetic(expression);
                    break;

                case BinaryOperator.Assign:
                case BinaryOperator.AddAssign:
                case BinaryOperator.AndAssign:
                case BinaryOperator.DivAssign:
                case BinaryOperator.ModAssign:
                case BinaryOperator.MulAssign:
                case BinaryOperator.OrAssign:
                case BinaryOperator.SubAssign:
                case BinaryOperator.XorAssign:
                    VisitAssignment(expression);
                    break;

                case BinaryOperator.Equal:
                case BinaryOperator.GreaterThan:
                case BinaryOperator.GreaterThanOrEqual:
                case BinaryOperator.LessThan:
                case BinaryOperator.LessThanOrEqual:
                case BinaryOperator.LogicalAnd:
                case BinaryOperator.LogicalOr:
                case BinaryOperator.NotEqual:
                    VisitLogical(expression);
                    break;

                case BinaryOperator.BitwiseAnd:
                case BinaryOperator.BitwiseOr:
                case BinaryOperator.BitwiseXor:
                    VisitBitwise(expression);
                    break;
            }
        }
        protected void VisitStatement(Statement statement)
        {
            switch (statement)
            {
                case ImportStatement importStatement:
                    VisitImport(importStatement);
                    break;

                case BlockStatement blockStatement:
                    VisitBlock(blockStatement);
                    break;

                case BreakStatement breakStatement:
                    VisitBreak(breakStatement);
                    break;

                case CaseStatement caseStatement:
                    VisitCase(caseStatement);
                    break;

                case ContinueStatement continueStatement:
                    VisitContinue(continueStatement);
                    break;

                case ElseStatement elseStatement:
                    VisitElse(elseStatement);
                    break;

                case EmptyStatement emptyStatement:
                    VisitEmpty(emptyStatement);
                    break;

                case ForStatement forStatement:
                    VisitFor(forStatement);
                    break;

                case IfStatement ifStatement:
                    VisitIf(ifStatement);
                    break;

                case SwitchStatement switchStatement:
                    VisitSwitch(switchStatement);
                    break;

                case WhileStatement whileStatement:
                    VisitWhile(whileStatement);
                    break;

                case ReturnStatement returnStatement:
                    VisitReturn(returnStatement);
                    break;
            }
        }
        protected void VisitDeclaration(Declaration node)
        {
            switch (node)
            {
                case ModuleDeclaration moduleDeclaration:
                    VisitModuleDeclaration(moduleDeclaration);
                    break;

                case ClassDeclaration classDeclaration:
                    VisitClass(classDeclaration);
                    break;

                case FieldDeclaration fieldDeclaration:
                    VisitField(fieldDeclaration);
                    break;

                case ConstructorDeclaration constructorDeclaration:
                    VisitConstructor(constructorDeclaration);
                    break;

                case PropertyDeclaration propertyDeclaration:
                    VisitProperty(propertyDeclaration);
                    break;

                case ParameterDeclaration parameterDeclaration:
                    VisitParameter(parameterDeclaration);
                    break;

                case MethodDeclaration methodDeclaration:
                    VisitMethod(methodDeclaration);
                    break;

                case VariableDeclaration variableDeclaration:
                    VisitVariable(variableDeclaration);
                    break;

                case TypeDeclaration typeDeclaration:
                    VisitType(typeDeclaration);
                    break;
            }
        }

        protected virtual void VisitDocument(SourceDocument sourceDocument)
        {
            foreach (var node in sourceDocument.Imports)
                node.Accept(this);

            foreach (var node in sourceDocument.Modules)
                node.Accept(this);
        }

        protected abstract void VisitArithmetic(BinaryExpression expression);
        protected abstract void VisitArrayAccess(ArrayAccessExpression expression);
        protected abstract void VisitAssignment(BinaryExpression expression);
        protected abstract void VisitBitwise(BinaryExpression expression);
        protected abstract void VisitBlock(BlockStatement statement);
        protected abstract void VisitImport(ImportStatement statement);
        protected abstract void VisitBreak(BreakStatement statement);
        protected abstract void VisitCase(CaseStatement statement);
        protected abstract void VisitClass(ClassDeclaration classDeclaration);
        protected abstract void VisitConstant(ConstantExpression expression);
        protected abstract void VisitConstructor(ConstructorDeclaration constructorDeclaration);
        protected abstract void VisitContinue(ContinueStatement statement);
        protected abstract void VisitElse(ElseStatement statement);
        protected abstract void VisitEmpty(EmptyStatement statement);
        protected abstract void VisitField(FieldDeclaration fieldDeclaration);
        protected abstract void VisitFor(ForStatement statement);
        protected abstract void VisitIdentifier(IdentifierExpression expression);
        protected abstract void VisitIf(IfStatement statement);
        protected abstract void VisitLambda(LambdaExpression expression);
        protected abstract void VisitLogical(BinaryExpression expression);
        protected abstract void VisitMethod(MethodDeclaration methodDeclaration);
        protected abstract void VisitType(TypeDeclaration typeDeclaration);
        protected abstract void VisitMethodCall(MethodCallExpression expression);
        protected abstract void VisitNew(NewExpression expression);
        protected abstract void VisitModuleDeclaration(ModuleDeclaration moduleDeclaration);
        protected abstract void VisitParameter(ParameterDeclaration parameterDeclaration);
        protected abstract void VisitProperty(PropertyDeclaration propertyDeclaration);
        protected abstract void VisitReference(ReferenceExpression expression);
        protected abstract void VisitSwitch(SwitchStatement statement);
        protected abstract void VisitUnary(UnaryExpression expression);
        protected abstract void VisitVariable(VariableDeclaration variableDeclaration);
        protected abstract void VisitWhile(WhileStatement statement);
        protected abstract void VisitReturn(ReturnStatement statement);
    }

    internal abstract class SyntaxVisitor<T> where T : SyntaxNode
    {
        public T Visit(SyntaxNode node)
        {
            switch (node)
            {
                case SourceDocument document:
                    return VisitDocument(node as SourceDocument);

                case Expression expression:
                    return VisitExpression(expression);

                case Statement statement:
                    return VisitStatement(statement);

                case Declaration declaration:
                    return VisitDeclaration(declaration);
            }

            // We shouldn't ever get here in reality
            return default(T);
        }

        protected T VisitExpression(Expression expression)
        {
            switch (expression)
            {
                case ArrayAccessExpression arrayAccessExpression:
                    return VisitArrayAccess(arrayAccessExpression);

                case BinaryExpression binaryExpression:
                    return VisitBinary(binaryExpression);

                case ConstantExpression constantExpression:
                    return VisitConstant(constantExpression);

                case IdentifierExpression identifierExpression:
                    return VisitIdentifier(identifierExpression);

                case LambdaExpression lambdaExpression:
                    return VisitLambda(lambdaExpression);

                case MethodCallExpression methodCallExpression:
                    return VisitMethodCall(methodCallExpression);

                case NewExpression newExpression:
                    return VisitNew(newExpression);

                case ReferenceExpression referenceExpression:
                    return VisitReference(referenceExpression);

                case UnaryExpression unaryExpression:
                    return VisitUnary(unaryExpression);
            }

            return default(T);
        }
        protected T VisitBinary(BinaryExpression expression)
        {
            switch (expression.Operator)
            {
                case BinaryOperator.Add:
                case BinaryOperator.Div:
                case BinaryOperator.Mod:
                case BinaryOperator.Sub:
                case BinaryOperator.Mul:
                    return VisitArithmetic(expression);

                case BinaryOperator.Assign:
                case BinaryOperator.AddAssign:
                case BinaryOperator.AndAssign:
                case BinaryOperator.DivAssign:
                case BinaryOperator.ModAssign:
                case BinaryOperator.MulAssign:
                case BinaryOperator.OrAssign:
                case BinaryOperator.SubAssign:
                case BinaryOperator.XorAssign:
                    return VisitAssignment(expression);

                case BinaryOperator.Equal:
                case BinaryOperator.GreaterThan:
                case BinaryOperator.GreaterThanOrEqual:
                case BinaryOperator.LessThan:
                case BinaryOperator.LessThanOrEqual:
                case BinaryOperator.LogicalAnd:
                case BinaryOperator.LogicalOr:
                case BinaryOperator.NotEqual:
                    return VisitLogical(expression);

                case BinaryOperator.BitwiseAnd:
                case BinaryOperator.BitwiseOr:
                case BinaryOperator.BitwiseXor:
                case BinaryOperator.LeftShift:
                case BinaryOperator.RightShift:
                    return VisitBitwise(expression);
            }

            return default(T);
        }
        protected T VisitStatement(Statement statement)
        {
            switch (statement)
            {
                case ImportStatement importStatement:
                    return VisitImport(importStatement);

                case BlockStatement blockStatement:
                    return VisitBlock(blockStatement);

                case BreakStatement breakStatement:
                    return VisitBreak(breakStatement);

                case CaseStatement caseStatement:
                    return VisitCase(caseStatement);

                case ContinueStatement continueStatement:
                    return VisitContinue(continueStatement);

                case ElseStatement elseStatement:
                    return VisitElse(elseStatement);

                case EmptyStatement emptyStatement:
                    return VisitEmpty(emptyStatement);

                case ForStatement forStatement:
                    return VisitFor(forStatement);

                case IfStatement ifStatement:
                    return VisitIf(ifStatement);

                case SwitchStatement switchStatement:
                    return VisitSwitch(switchStatement);

                case WhileStatement whileStatement:
                    return VisitWhile(whileStatement);

                case ReturnStatement returnStatement:
                    return VisitReturn(returnStatement);
            }

            return default(T);
        }
        protected T VisitDeclaration(Declaration node)
        {
            switch (node)
            {
                case ModuleDeclaration moduleDeclaration:
                    return VisitModuleDeclaration(moduleDeclaration);

                case ClassDeclaration classDeclaration:
                    return VisitClass(classDeclaration);

                case FieldDeclaration fieldDeclaration:
                    return VisitField(fieldDeclaration);

                case ConstructorDeclaration constructorDeclaration:
                    return VisitConstructor(constructorDeclaration);

                case PropertyDeclaration propertyDeclaration:
                    return VisitProperty(propertyDeclaration);

                case ParameterDeclaration parameterDeclaration:
                    return VisitParameter(parameterDeclaration);

                case MethodDeclaration methodDeclaration:
                    return VisitMethod(methodDeclaration);

                case VariableDeclaration variableDeclaration:
                    return VisitVariable(variableDeclaration);

                case TypeDeclaration typeDeclaration:
                    return VisitType(typeDeclaration);
            }

            return default(T);
        }

        protected abstract T VisitDocument(SourceDocument sourceDocument);
        protected abstract T VisitArithmetic(BinaryExpression expression);
        protected abstract T VisitArrayAccess(ArrayAccessExpression expression);
        protected abstract T VisitAssignment(BinaryExpression expression);
        protected abstract T VisitBitwise(BinaryExpression expression);
        protected abstract T VisitBlock(BlockStatement statement);
        protected abstract T VisitImport(ImportStatement statement);
        protected abstract T VisitBreak(BreakStatement statement);
        protected abstract T VisitCase(CaseStatement statement);
        protected abstract T VisitClass(ClassDeclaration classDeclaration);
        protected abstract T VisitConstant(ConstantExpression expression);
        protected abstract T VisitConstructor(ConstructorDeclaration constructorDeclaration);
        protected abstract T VisitContinue(ContinueStatement statement);
        protected abstract T VisitElse(ElseStatement statement);
        protected abstract T VisitEmpty(EmptyStatement statement);
        protected abstract T VisitField(FieldDeclaration fieldDeclaration);
        protected abstract T VisitFor(ForStatement statement);
        protected abstract T VisitIdentifier(IdentifierExpression expression);
        protected abstract T VisitIf(IfStatement statement);
        protected abstract T VisitLambda(LambdaExpression expression);
        protected abstract T VisitLogical(BinaryExpression expression);
        protected abstract T VisitMethod(MethodDeclaration methodDeclaration);
        protected abstract T VisitType(TypeDeclaration typeDeclaration);
        protected abstract T VisitMethodCall(MethodCallExpression expression);
        protected abstract T VisitNew(NewExpression expression);
        protected abstract T VisitModuleDeclaration(ModuleDeclaration moduleDeclaration);
        protected abstract T VisitParameter(ParameterDeclaration parameterDeclaration);
        protected abstract T VisitProperty(PropertyDeclaration propertyDeclaration);
        protected abstract T VisitReference(ReferenceExpression expression);
        protected abstract T VisitSwitch(SwitchStatement statement);
        protected abstract T VisitUnary(UnaryExpression expression);
        protected abstract T VisitVariable(VariableDeclaration variableDeclaration);
        protected abstract T VisitWhile(WhileStatement statement);
        protected abstract T VisitReturn(ReturnStatement statement);
    }
}
