using System;

namespace Compiler.Parsing.Syntax.Expressions
{
    internal class ConstantExpression : Expression
    {
        public override SyntaxKind Kind => SyntaxKind.ConstantExpression;
        public string Value { get; }
        public ConstantType ConstantType { get; }

        public ConstantExpression(SourceFilePart filePart, string value, ConstantType type) 
            : base(filePart)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Value = value;
            ConstantType = type;
        }
    }
}
