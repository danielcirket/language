using System;
using System.Collections.Generic;
using Compiler.Parsing.Syntax.Declarations;
using Compiler.Parsing.Syntax.Statements;
//using Compiler.Semantics;

namespace Compiler.Parsing.Syntax
{
    internal class SourceDocument : SyntaxNode
    {
        public override SyntaxKind Kind => SyntaxKind.SourceDocument;
        public override SyntaxCategory Category => SyntaxCategory.SourceDocument;
        public IEnumerable<ImportStatement> Imports { get; }
        public IEnumerable<ModuleDeclaration> Modules { get; }
        //public Scope Scope { get; }

        public SourceDocument(SourceFilePart filePart, IEnumerable<ImportStatement> imports, IEnumerable<ModuleDeclaration> modules)
            : base(filePart)
        {
            if (imports == null)
                throw new ArgumentNullException(nameof(imports));

            if (modules == null)
                throw new ArgumentNullException(nameof(modules));

            Imports = imports;
            Modules = modules;
        }
        //public SourceDocument(SourceDocument sourceDocument, IEnumerable<ModuleDeclaration> modules, Scope scope)
        //    : this(sourceDocument.FilePart, sourceDocument.Imports, modules)
        //{
        //    Scope = scope;
        //}
    }
}
