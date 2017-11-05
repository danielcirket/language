using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.Parsing.Syntax.Expressions;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class EnumMemberDeclaration : Declaration
    {
        public Expression Value { get; }
        public override SyntaxKind Kind => SyntaxKind.EnumMemberDeclaration;

        public EnumMemberDeclaration(SourceFilePart filePart, string name, Expression value) 
            : this(filePart, name, value, Enumerable.Empty<AttributeSyntax>())
        {
        }
        public EnumMemberDeclaration(SourceFilePart filePart, string name, Expression value, IEnumerable<AttributeSyntax> attributes) 
            : base(filePart, name, attributes)
        {
            // NOTE(Dan): Value can be null to allow enum X { One, Two, Three }
            Value = value;
        }
    }
}
