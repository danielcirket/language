using Compiler.Parsing.Syntax.Expressions;

namespace Compiler.Parsing.Syntax.Statements
{
    internal class ReturnStatement : Statement
    {
        public override SyntaxKind Kind => SyntaxKind.ReturnStatement;
        public Expression Value { get; }

        public ReturnStatement(SourceFilePart filePart, Expression value) 
            : base(filePart)
        {
            Value = value;
        }
    }
}
