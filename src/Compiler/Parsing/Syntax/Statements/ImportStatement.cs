using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.Parsing.Syntax.Expressions;

namespace Compiler.Parsing.Syntax.Statements
{
    internal class ImportStatement : Statement
    {
        public override SyntaxKind Kind => throw new NotImplementedException();
        public IEnumerable<IdentifierExpression> Names { get; }
        public string Name => string.Join(".", Names.Select(n => n.Name));

        public ImportStatement(SourceFilePart filePart, IEnumerable<IdentifierExpression> names) 
            : base(filePart)
        {
            if (names == null)
                throw new ArgumentNullException(nameof(names));

            Names = names;
        }
    }
}
