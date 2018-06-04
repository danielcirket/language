using Compiler.Parsing.Syntax.Expressions.Types;
using Compiler.Semantics.BoundSyntax.Expressions;

namespace Compiler.Semantics.Types
{
    internal class BuiltInTypeNameAlias
    {
        public static string From(TypeExpression typeExpression)
        {
            return From(typeExpression.Name);
        }
        public static string From(BoundTypeExpression typeExpression)
        {
            return From(typeExpression.Name);
        }
        private static string From(string name)
        {
            switch (name.ToLower())
            {
                case "bool":
                    return "Bool";
                case "byte":
                    return "Byte";
                case "sbyte":
                    return "SByte";
                case "char":
                    return "Char";
                case "decimal":
                    return "Decimal";
                case "double":
                    return "Double";
                case "exception":
                    return "Exception";
                case "float":
                    return "Float";
                case "int":
                    return "Int32";
                case "uint":
                    return "UInt32";
                case "long":
                    return "Int64";
                case "ulong":
                    return "UInt64";
                case "short":
                    return "Int16";
                case "ushort":
                    return "UInt16";
                case "string":
                    return "String";
                case "void":
                    return "Void";
            }

            if (name.StartsWith("Optional<"))
                return "Optional";

            return name;
        }
    }
}
