using System;
using Compiler.Semantics.BoundSyntax.Declarations;
using Compiler.Semantics.BoundSyntax.Expressions;

namespace Compiler.Semantics
{
    internal class ReferenceTypeLocator
    {
        public BoundTypeExpression TypeFor(BoundExpression expression)
        {
            switch (expression)
            {
                case BoundIdentifierExpression identifier:
                    return TypeFor(identifier);
                case BoundMethodCallExpression method:
                    return TypeFor(method);
                case BoundNewExpression @new:
                    return TypeFor(@new.Reference);

                default:
                    throw new NotImplementedException($"DeclarationFor({expression.GetType().Name}) is not implemented");
            }
        }
        public BoundTypeExpression TypeFor(BoundDeclaration declaration)
        {
            if (declaration == null)
                return null;

            switch (declaration)
            {
                case BoundVariableDeclaration variable:
                    return variable.Type;
                case BoundMethodDeclaration method:
                    return method.ReturnType;
                case BoundPropertyDeclaration property:
                    return property.Type;
                case BoundFieldDeclaration field:
                    return field.Type;

                default:
                    throw new NotImplementedException($"TypeFor({declaration.GetType().Name}) is not implemented");
            }
        }

        private BoundTypeExpression TypeFor(BoundIdentifierExpression expression)
        {
            return TypeFor(expression.Declaration.Declaration);
        }
        private BoundTypeExpression TypeFor(BoundMethodCallExpression expression)
        {
            return TypeFor(expression.Reference);
        }

        private BoundTypeExpression TypeFor(BoundTypeExpression expression)
        {
            return expression;
        }
    }
}
