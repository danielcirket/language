using System;
using System.Collections.Generic;
using Compiler.Parsing;
using Compiler.Semantics.Passes.Declaration;
using Compiler.Semantics.Passes.Types.Inference;

namespace Compiler.Semantics
{
    internal class SematicAnalyzer
    {
        private readonly ErrorSink _errorSink;
        private readonly IEnumerable<ISemanticPass> _passes;

        public ErrorSink ErrorSink => _errorSink;

        public void Analyze(CompilationRoot root)
        {
            // TODO(Dan): Cleanup this, we should be able to merge the passes together now the actual logic is contained in the
            //            `SyntaxBinder`
            var forwardDeclarationPass = new ForwardDeclarationPass(_errorSink);
            var result = forwardDeclarationPass.Run(root);

            foreach(var pass in _passes)
            {
                if (!pass.ShouldContinue)
                    return;

                pass.Run(ref result);
            }
            //return result;
        }

        public SematicAnalyzer(ErrorSink errorSink)
        {
            if (errorSink == null)
                throw new ArgumentNullException(nameof(errorSink));

            _errorSink = errorSink;

            _passes = new List<ISemanticPass>
            {
                new DeclarationPass(_errorSink),
                new TypeInferencePass(_errorSink),
                //new TypeResolutionPass(),
                //new TypeCheckPass()
                // Generate properties etc
                // lowering?
            };
        }
    }
}
