using System;
using System.Collections.Generic;
using System.Text;
using Compiler.Parsing.Syntax;
using Compiler.Parsing.Syntax.Expressions;
using Compiler.Semantics.BoundSyntax;
using Compiler.Semantics.BoundSyntax.Declarations;
using Compiler.Semantics.BoundSyntax.Expressions;
using Compiler.Semantics.BoundSyntax.Expressions.Types;
using Compiler.Semantics.BoundSyntax.Statements;

namespace Compiler.Semantics
{
    internal abstract class BoundSyntaxVisitor
    {
        public void Visit(BoundSyntaxNode node)
        {
            switch (node)
            {
                case BoundExpression expression:
                    VisitExpression(expression);
                    break;

                case BoundStatement statement:
                    VisitStatement(statement);
                    break;

                case BoundDeclaration declaration:
                    VisitDeclaration(declaration);
                    break;
            }
        }

        protected void VisitExpression(BoundExpression expression)
        {
            switch (expression)
            {
                case BoundArrayAccessExpression BoundArrayAccessExpression:
                    VisitArrayAccess(BoundArrayAccessExpression);
                    break;

                case BoundBinaryExpression BoundBinaryExpression:
                    VisitBinary(BoundBinaryExpression);
                    break;

                case BoundConstantExpression constantExpression:
                    VisitConstant(constantExpression);
                    break;

                case BoundIdentifierExpression identifierExpression:
                    VisitIdentifier(identifierExpression);
                    break;

                case BoundLambdaExpression lambdaExpression:
                    VisitLambda(lambdaExpression);
                    break;

                case BoundMethodCallExpression methodCallExpression:
                    VisitMethodCall(methodCallExpression);
                    break;

                case BoundNewExpression newExpression:
                    VisitNew(newExpression);
                    break;

                case BoundReferenceExpression referenceExpression:
                    VisitReference(referenceExpression);
                    break;

                case BoundUnaryExpression unaryExpression:
                    VisitUnary(unaryExpression);
                    break;

                case BoundTypeExpression typeExpression:
                    VisitType(typeExpression);
                    break;
            }
        }
        protected void VisitBinary(BoundBinaryExpression expression)
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
        protected void VisitStatement(BoundStatement statement)
        {
            switch (statement)
            {
                case BoundImportStatement importStatement:
                    VisitImport(importStatement);
                    break;

                case BoundBlockStatement blockStatement:
                    VisitBlock(blockStatement);
                    break;

                case BoundBreakStatement breakStatement:
                    VisitBreak(breakStatement);
                    break;

                case BoundCaseStatement caseStatement:
                    VisitCase(caseStatement);
                    break;

                case BoundContinueStatement continueStatement:
                    VisitContinue(continueStatement);
                    break;

                case BoundElseStatement elseStatement:
                    VisitElse(elseStatement);
                    break;

                case BoundEmptyStatement emptyStatement:
                    VisitEmpty(emptyStatement);
                    break;

                case BoundForStatement forStatement:
                    VisitFor(forStatement);
                    break;

                case BoundIfStatement ifStatement:
                    VisitIf(ifStatement);
                    break;

                case BoundSwitchStatement switchStatement:
                    VisitSwitch(switchStatement);
                    break;

                case BoundWhileStatement whileStatement:
                    VisitWhile(whileStatement);
                    break;

                case BoundReturnStatement returnStatement:
                    VisitReturn(returnStatement);
                    break;
            }
        }
        protected void VisitDeclaration(BoundDeclaration node)
        {
            switch (node)
            {
                case BoundModuleDeclaration moduleDeclaration:
                    VisitModuleDeclaration(moduleDeclaration);
                    break;

                case BoundClassDeclaration classDeclaration:
                    VisitClass(classDeclaration);
                    break;

                case BoundInterfaceDeclaration interfaceDeclaration:
                    VisitInterface(interfaceDeclaration);
                    break;

                case BoundFieldDeclaration fieldDeclaration:
                    VisitField(fieldDeclaration);
                    break;

                case BoundConstructorDeclaration constructorDeclaration:
                    VisitConstructor(constructorDeclaration);
                    break;

                case BoundPropertyDeclaration propertyDeclaration:
                    VisitProperty(propertyDeclaration);
                    break;

                case BoundParameterDeclaration parameterDeclaration:
                    VisitParameter(parameterDeclaration);
                    break;

                case BoundMethodDeclaration methodDeclaration:
                    VisitMethod(methodDeclaration);
                    break;

                case BoundVariableDeclaration variableDeclaration:
                    VisitVariable(variableDeclaration);
                    break;

                case BoundEnumDeclaration enumDeclaration:
                    VisitEnum(enumDeclaration);
                    break;

                case BoundEnumMemberDeclaration enumMemberDeclaration:
                    VisitEnumMember(enumMemberDeclaration);
                    break;
            }
        }

        protected abstract void VisitInterface(BoundInterfaceDeclaration interfaceDeclaration);
        protected abstract void VisitEnum(BoundEnumDeclaration enumDeclaration);
        protected abstract void VisitEnumMember(BoundEnumMemberDeclaration enumMemberDeclaration);
        protected abstract void VisitArithmetic(BoundBinaryExpression expression);
        protected abstract void VisitArrayAccess(BoundArrayAccessExpression expression);
        protected abstract void VisitAssignment(BoundBinaryExpression expression);
        protected abstract void VisitBitwise(BoundBinaryExpression expression);
        protected abstract void VisitBlock(BoundBlockStatement statement);
        protected abstract void VisitImport(BoundImportStatement statement);
        protected abstract void VisitBreak(BoundBreakStatement statement);
        protected abstract void VisitCase(BoundCaseStatement statement);
        protected abstract void VisitClass(BoundClassDeclaration classDeclaration);
        protected abstract void VisitConstant(BoundConstantExpression expression);
        protected abstract void VisitConstructor(BoundConstructorDeclaration constructorDeclaration);
        protected abstract void VisitContinue(BoundContinueStatement statement);
        protected abstract void VisitElse(BoundElseStatement statement);
        protected abstract void VisitEmpty(BoundEmptyStatement statement);
        protected abstract void VisitField(BoundFieldDeclaration fieldDeclaration);
        protected abstract void VisitFor(BoundForStatement statement);
        protected abstract void VisitIdentifier(BoundIdentifierExpression expression);
        protected abstract void VisitIf(BoundIfStatement statement);
        protected abstract void VisitLambda(BoundLambdaExpression expression);
        protected abstract void VisitLogical(BoundBinaryExpression expression);
        protected abstract void VisitMethod(BoundMethodDeclaration methodDeclaration);
        protected abstract void VisitType(BoundTypeExpression typeExpression);
        protected abstract void VisitMethodCall(BoundMethodCallExpression expression);
        protected abstract void VisitNew(BoundNewExpression expression);
        protected abstract void VisitModuleDeclaration(BoundModuleDeclaration moduleDeclaration);
        protected abstract void VisitParameter(BoundParameterDeclaration parameterDeclaration);
        protected abstract void VisitProperty(BoundPropertyDeclaration propertyDeclaration);
        protected abstract void VisitReference(BoundReferenceExpression expression);
        protected abstract void VisitSwitch(BoundSwitchStatement statement);
        protected abstract void VisitUnary(BoundUnaryExpression expression);
        protected abstract void VisitVariable(BoundVariableDeclaration variableDeclaration);
        protected abstract void VisitWhile(BoundWhileStatement statement);
        protected abstract void VisitReturn(BoundReturnStatement statement);
    }

    internal abstract class BoundSyntaxVisitor<T> where T : BoundSyntaxNode
    {
        public T Visit(BoundSyntaxNode node)
        {
            switch (node)
            {
                case BoundExpression expression:
                    return VisitExpression(expression);

                case BoundStatement statement:
                    return VisitStatement(statement);

                case BoundDeclaration declaration:
                    return VisitDeclaration(declaration);
            }

            // We shouldn't ever get here in reality
            return default;
        }

        protected T VisitExpression(BoundExpression expression)
        {
            switch (expression)
            {
                case BoundArrayAccessExpression arrayAccessExpression:
                    return VisitArrayAccess(arrayAccessExpression);

                case BoundBinaryExpression binaryExpression:
                    return VisitBinary(binaryExpression);

                case BoundConstantExpression constantExpression:
                    return VisitConstant(constantExpression);

                case BoundIdentifierExpression identifierExpression:
                    return VisitIdentifier(identifierExpression);

                case BoundLambdaExpression lambdaExpression:
                    return VisitLambda(lambdaExpression);

                case BoundMethodCallExpression methodCallExpression:
                    return VisitMethodCall(methodCallExpression);

                case BoundNewExpression newExpression:
                    return VisitNew(newExpression);

                case BoundReferenceExpression referenceExpression:
                    return VisitReference(referenceExpression);

                case BoundUnaryExpression unaryExpression:
                    return VisitUnary(unaryExpression);

                case BoundTypeExpression typeExpression:
                    return VisitType(typeExpression);
            }

            return default;
        }
        protected T VisitBinary(BoundBinaryExpression expression)
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

            return default;
        }
        protected T VisitStatement(BoundStatement statement)
        {
            switch (statement)
            {
                case BoundImportStatement importStatement:
                    return VisitImport(importStatement);

                case BoundBlockStatement blockStatement:
                    return VisitBlock(blockStatement);

                case BoundBreakStatement breakStatement:
                    return VisitBreak(breakStatement);

                case BoundCaseStatement caseStatement:
                    return VisitCase(caseStatement);

                case BoundContinueStatement continueStatement:
                    return VisitContinue(continueStatement);

                case BoundElseStatement elseStatement:
                    return VisitElse(elseStatement);

                case BoundEmptyStatement emptyStatement:
                    return VisitEmpty(emptyStatement);

                case BoundForStatement forStatement:
                    return VisitFor(forStatement);

                case BoundIfStatement ifStatement:
                    return VisitIf(ifStatement);

                case BoundSwitchStatement switchStatement:
                    return VisitSwitch(switchStatement);

                case BoundWhileStatement whileStatement:
                    return VisitWhile(whileStatement);

                case BoundReturnStatement returnStatement:
                    return VisitReturn(returnStatement);
            }

            return default;
        }
        protected T VisitDeclaration(BoundDeclaration node)
        {
            switch (node)
            {
                case BoundModuleDeclaration moduleDeclaration:
                    return VisitModuleDeclaration(moduleDeclaration);

                case BoundClassDeclaration classDeclaration:
                    return VisitClass(classDeclaration);

                case BoundInterfaceDeclaration interfaceDeclaration:
                    return VisitInterface(interfaceDeclaration);

                case BoundFieldDeclaration fieldDeclaration:
                    return VisitField(fieldDeclaration);

                case BoundConstructorDeclaration constructorDeclaration:
                    return VisitConstructor(constructorDeclaration);

                case BoundPropertyDeclaration propertyDeclaration:
                    return VisitProperty(propertyDeclaration);

                case BoundParameterDeclaration parameterDeclaration:
                    return VisitParameter(parameterDeclaration);

                case BoundMethodDeclaration methodDeclaration:
                    return VisitMethod(methodDeclaration);

                case BoundVariableDeclaration variableDeclaration:
                    return VisitVariable(variableDeclaration);

                case BoundEnumDeclaration enumDeclaration:
                    return VisitEnum(enumDeclaration);

                case BoundEnumMemberDeclaration enumMemberDeclaration:
                    return VisitEnumMember(enumMemberDeclaration);
            }

            return default;
        }
        protected T VisitType(BoundTypeExpression typeExpression)
        {
            switch (typeExpression)
            {
                case BoundInferredTypeExpression inferredTypeExpression:
                    return VisitInferredType(inferredTypeExpression);

                case BoundPredefinedTypeExpression predfinedTypeExpression:
                    return VisitPredefinedType(predfinedTypeExpression);

                case BoundUserDefinedTypeExpression userdefinedTypeExpression:
                    return VisitUserDefinedType(userdefinedTypeExpression);

                case BoundGenericConstraintTypeExpression genericConstraintTypeExpression:
                    return VisitBoundGenericConstraintType(genericConstraintTypeExpression);

                default:
                    throw new NotImplementedException($"{typeExpression.GetType().Name} has not been implemented");
            }
        }

        protected abstract T VisitBoundGenericConstraintType(BoundGenericConstraintTypeExpression genericConstraintTypeExpression);
        protected abstract T VisitUserDefinedType(BoundUserDefinedTypeExpression userdefinedTypeExpression);
        protected abstract T VisitPredefinedType(BoundPredefinedTypeExpression predfinedTypeExpression);
        protected abstract T VisitInferredType(BoundInferredTypeExpression inferredTypeExpression);
        protected abstract T VisitEnumMember(BoundEnumMemberDeclaration enumMemberDeclaration);
        protected abstract T VisitEnum(BoundEnumDeclaration enumDeclaration);
        protected abstract T VisitInterface(BoundInterfaceDeclaration interfaceDeclaration);
        protected abstract T VisitArithmetic(BoundBinaryExpression expression);
        protected abstract T VisitArrayAccess(BoundArrayAccessExpression expression);
        protected abstract T VisitAssignment(BoundBinaryExpression expression);
        protected abstract T VisitBitwise(BoundBinaryExpression expression);
        protected abstract T VisitBlock(BoundBlockStatement statement);
        protected abstract T VisitImport(BoundImportStatement statement);
        protected abstract T VisitBreak(BoundBreakStatement statement);
        protected abstract T VisitCase(BoundCaseStatement statement);
        protected abstract T VisitClass(BoundClassDeclaration classDeclaration);
        protected abstract T VisitConstant(BoundConstantExpression expression);
        protected abstract T VisitConstructor(BoundConstructorDeclaration constructorDeclaration);
        protected abstract T VisitContinue(BoundContinueStatement statement);
        protected abstract T VisitElse(BoundElseStatement statement);
        protected abstract T VisitEmpty(BoundEmptyStatement statement);
        protected abstract T VisitField(BoundFieldDeclaration fieldDeclaration);
        protected abstract T VisitFor(BoundForStatement statement);
        protected abstract T VisitIdentifier(BoundIdentifierExpression expression);
        protected abstract T VisitIf(BoundIfStatement statement);
        protected abstract T VisitLambda(BoundLambdaExpression expression);
        protected abstract T VisitLogical(BoundBinaryExpression expression);
        protected abstract T VisitMethod(BoundMethodDeclaration methodDeclaration);
        protected abstract T VisitMethodCall(BoundMethodCallExpression expression);
        protected abstract T VisitNew(BoundNewExpression expression);
        protected abstract T VisitModuleDeclaration(BoundModuleDeclaration moduleDeclaration);
        protected abstract T VisitParameter(BoundParameterDeclaration parameterDeclaration);
        protected abstract T VisitProperty(BoundPropertyDeclaration propertyDeclaration);
        protected abstract T VisitReference(BoundReferenceExpression expression);
        protected abstract T VisitSwitch(BoundSwitchStatement statement);
        protected abstract T VisitUnary(BoundUnaryExpression expression);
        protected abstract T VisitVariable(BoundVariableDeclaration variableDeclaration);
        protected abstract T VisitWhile(BoundWhileStatement statement);
        protected abstract T VisitReturn(BoundReturnStatement statement);
    }
}
