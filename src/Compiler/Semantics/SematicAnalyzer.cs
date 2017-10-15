using System;
using System.Collections.Generic;
using Compiler.Parsing;
using Compiler.Semantics.Passes;

namespace Compiler.Semantics
{
    internal class SematicAnalyzer
    {
        private readonly ErrorSink _errorSink;
        private readonly IEnumerable<ISemanticPass> _passes = new List<ISemanticPass>
        {
            new ForwardDeclarationPass(),
            //new DeclarationPass(),
            //new TypeResolutionPass(),
            //new TypeInferencePass(),
            //new TypeCheckPass()
        };

        public ErrorSink ErrorSink => _errorSink;

        public void Analyze(CompilationUnit unit)
        {
            foreach (var pass in _passes)
            {
                pass.Run(_errorSink, ref unit);

                if (!pass.ShouldContinue)
                {
                    // TODO(Dan): Deal with printing errors etc
                    throw new NotImplementedException();
                }
            }
        }

        public SematicAnalyzer(ErrorSink errorSink)
        {
            if (errorSink == null)
                throw new ArgumentNullException(nameof(errorSink));

            _errorSink = errorSink;
        }
    }
}
