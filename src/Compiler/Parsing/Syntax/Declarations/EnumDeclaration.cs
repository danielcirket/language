using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.Parsing.Syntax.Declarations
{
    internal class EnumDeclaration : Declaration
    {
        public IEnumerable<EnumMemberDeclaration> Members { get; }
        public override SyntaxKind Kind => SyntaxKind.EnumDeclaration;

        public EnumDeclaration(SourceFilePart filePart, string name, IEnumerable<EnumMemberDeclaration> members) 
            : this(filePart, name, members, Enumerable.Empty<AttributeSyntax>())
        {
        }
        public EnumDeclaration(SourceFilePart filePart, string name, IEnumerable<EnumMemberDeclaration> members, IEnumerable<AttributeSyntax> attributes) 
            : base(filePart, name, attributes)
        {
            if (members == null)
                throw new ArgumentNullException(nameof(members));

            Members = members;
        }
    }
}
