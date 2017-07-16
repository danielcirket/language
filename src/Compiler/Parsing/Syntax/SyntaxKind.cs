namespace Compiler.Parsing.Syntax
{
    internal enum SyntaxKind
    {
        Invalid,

        BinaryExpression,
        UnaryExpression,
        IdentifierExpression,
        ConstantExpression,
        ReferenceExpression,
        MethodCallExpression,
        LambdaExpression,
        NewExpression,
        ArrayAccessExpression,

        ParameterDeclaration,
        VariableDeclaration,
        ClassDeclaration,
        FieldDeclaration,
        PropertyDeclaration,
        MethodDeclaration,
        ConstructorDeclaration,
        TypeDeclaration,
        ModuleDeclaration,

        BlockStatement,
        WhileStatement,
        IfStatement,
        ElseStatement,
        SwitchStatement,
        CaseStatement,
        ImportStatement,
        EmptyStatement,
        BreakStatement,
        ContinueStatement,
        ForStatement,
        ReturnStatement,
        SourceDocument
    }
}
