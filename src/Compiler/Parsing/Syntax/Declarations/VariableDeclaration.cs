using System;
using Compiler.Parsing.Syntax.Expressions;
using Compiler.Parsing.Syntax.Expressions.Types;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class VariableDeclaration : Declaration
    {
        public override SyntaxKind Kind => SyntaxKind.VariableDeclaration;
        public TypeExpression Type { get; }
        public Expression Value { get; }
        public VariableMutabilityType MutabilityType { get; }

        public VariableDeclaration(SourceFilePart filePart, string name, TypeExpression type, Expression value, VariableMutabilityType variableMutabilityType)
            : base(filePart, name)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Type = type;
            Value = value;
            MutabilityType = variableMutabilityType;
        }
    }
}
