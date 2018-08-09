using System;
using Compiler.Semantics.BoundSyntax.Declarations;
using Compiler.Semantics.BoundSyntax.Expressions;

namespace Compiler.Semantics
{
    internal class ReferenceDeclarationLocator
    {
        public BoundDeclaration DeclarationFor(BoundExpression expression)
        {
            switch (expression)
            {
                case BoundIdentifierExpression identifier:
                    return DeclarationFor(identifier);
                case BoundMethodCallExpression method:
                    return DeclarationFor(method);
                case BoundNewExpression @new:
                    return DeclarationFor(@new.Reference);

                default:
                    throw new NotImplementedException($"DeclarationFor({expression.GetType().Name}) is not implemented");
            }
        }
        public BoundDeclaration DeclarationFor(BoundDeclaration declaration)
        {
            if (declaration == null)
                return null;

            switch (declaration)
            {
                case BoundVariableDeclaration variable:
                    return DeclarationFor(variable.Value);
                case BoundMethodDeclaration method:
                    return DeclarationFor(method.ReturnType);
                case BoundPropertyDeclaration method:
                    return DeclarationFor(method.Type);
                case BoundClassDeclaration @class:
                    return @class;
                case BoundFieldDeclaration field:
                    return DeclarationFor(field.Type);

                default:
                    throw new NotImplementedException($"DeclarationFor({declaration.GetType().Name}) is not implemented");
            }
        }

        private BoundDeclaration DeclarationFor(BoundIdentifierExpression expression)
        {
            return DeclarationFor(expression.Symbol.Declaration);
        }
        private BoundDeclaration DeclarationFor(BoundMethodCallExpression expression)
        {
            return DeclarationFor(expression.Reference);
        }

        private BoundDeclaration DeclarationFor(BoundTypeExpression expression)
        {
            return expression.Symbol.Declaration;
        }
    }
}
