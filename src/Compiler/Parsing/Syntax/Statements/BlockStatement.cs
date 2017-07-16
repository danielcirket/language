using System;
using System.Collections.Generic;

namespace Compiler.Parsing.Syntax.Statements
{
    internal class BlockStatement : Statement
    {
        public override SyntaxKind Kind => SyntaxKind.BlockStatement;
        public IEnumerable<SyntaxNode> Contents { get; }

        public BlockStatement(SourceFilePart filePart, IEnumerable<SyntaxNode> contents) 
            : base(filePart)
        {
            if (contents == null)
                throw new ArgumentNullException(nameof(contents));

            Contents = contents;
        }
    }
}
