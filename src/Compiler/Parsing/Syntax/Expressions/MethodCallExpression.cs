using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax.Expressions.Types;

namespace Compiler.Parsing.Syntax.Expressions
{
    internal class MethodCallExpression : Expression
    {
        public override SyntaxKind Kind => SyntaxKind.MethodCallExpression;
        public Expression Reference { get; }
        public IEnumerable<Expression> Arguments { get; }
        public IEnumerable<TypeExpression> GenericTypes { get; }

        public MethodCallExpression(SourceFilePart filePart, Expression reference, IEnumerable<Expression> arguments, IEnumerable<TypeExpression> genericTypes) 
            : base(filePart)
        {
            if (reference == null)
                throw new ArgumentNullException(nameof(reference));
            if (genericTypes == null)
                throw new ArgumentNullException(nameof(arguments));
            if (arguments == null)
                throw new ArgumentNullException(nameof(genericTypes));

            Reference = reference;
            Arguments = arguments;
            GenericTypes = genericTypes;
        }
    }
}
