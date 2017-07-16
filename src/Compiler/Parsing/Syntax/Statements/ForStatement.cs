using Compiler.Parsing.Syntax.Expressions;

namespace Compiler.Parsing.Syntax.Statements
{
    internal class ForStatement : Statement
    {
        public override SyntaxKind Kind => SyntaxKind.ForStatement;
        public SyntaxNode Initialiser { get; }
        public Expression Condition { get; }
        public Expression Increment { get; }

        public ForStatement(SourceFilePart filePart, SyntaxNode initialiser, Expression condition, Expression increment) 
            : base(filePart)
        {
            // NOTE(Dan): These can all be empty, e.g. for (;;) {}
            // TODO(Dan): Do we want to allow this?!
            // TODO(Dan): Consider how lowering other loops will impact this too!
            Initialiser = initialiser;
            Condition = condition;
            Increment = increment;
        }
    }
}
