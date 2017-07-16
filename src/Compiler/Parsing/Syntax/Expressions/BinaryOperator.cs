namespace Compiler.Parsing.Syntax.Expressions
{
    internal enum BinaryOperator
    {
        Assign,
        AddAssign,
        SubAssign,
        MulAssign,
        DivAssign,
        ModAssign,
        AndAssign,
        XorAssign,
        OrAssign,

        LogicalOr,
        LogicalAnd,

        Equal,
        NotEqual,

        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,

        BitwiseAnd,
        BitwiseOr,
        BitwiseXor,

        LeftShift,
        RightShift,

        Add,
        Sub,
        Mul,
        Div,
        Mod
    }
}
