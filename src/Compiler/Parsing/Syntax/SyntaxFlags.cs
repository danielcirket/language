using System;

namespace Compiler.Parsing.Syntax
{
    [Flags]
    internal enum SyntaxFlags
    {
        None = 1 << 0,
        AutoImplementedPropertyGetter = 1 << 1,
        AutoImplementedPropertySetter = 1 << 2,
    }
}
