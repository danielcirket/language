using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.Lexing;
using Compiler.Parsing.Syntax;
using Compiler.Parsing.Syntax.Declarations;
using Compiler.Parsing.Syntax.Expressions;
using Compiler.Parsing.Syntax.Statements;

namespace Compiler.Parsing
{
    internal class SyntaxParser
    {
        private readonly Tokenizer _tokenizer;
        private readonly ErrorSink _errorSink;
        private SourceFile _currentSourceFile;
        private IEnumerable<Token> _tokens;
        private int _index;
        private bool _error;

        public ErrorSink ErrorSink => _errorSink;
        private Token Current => _tokens.ElementAtOrDefault(_index) ?? _tokens.Last();
        private Token Next => Peek(1);
        private Token Last => Peek(-1);

        public CompilationUnit Parse(SourceFile sourceFile)
        {
            if (sourceFile == null)
                throw new ArgumentNullException(nameof(sourceFile));

            _currentSourceFile = sourceFile;
            _tokens = _tokenizer.Tokenize(sourceFile).Where(token => !token.IsTrivia()).ToList();

            // NOTE(Dan): If we have errors already just bail? Perhaps we want to actually try parsing instead?
            if (_errorSink.HasErrors)
                return null;

            try
            {
                return new CompilationUnit(new[] { ParseInternal() });
            }
            catch (SyntaxException)
            {
                return null;
            }
        }
        private SyntaxNode ParseInternal()
        {
            return ParseDocument();
        }
        private SourceDocument ParseDocument()
        {
            var imports = new List<ImportStatement>();
            var modules = new List<ModuleDeclaration>();

            var start = Current.Start;

            while (Current == "import")
                imports.Add(ParseImportStatement());

            while (Current == "module")
                modules.Add(ParseModuleDeclaration());

            if (Current != TokenType.EOF)
                AddError("Top-level statements are not permitted. Statements must be part of a module with the exception of import statements which are at the start of the file", CreatePart(Current.Start, _tokens.Last().End), Severity.Error);

            return new SourceDocument(CreatePart(start), imports, modules);
        }
        private IEnumerable<AttributeSyntax> ParseAttributes()
        {
            var attributes = new List<AttributeSyntax>();

            Take(TokenType.LeftBrace);

            attributes.Add(ParseAttribute());

            while(Current != TokenType.RightBrace)
            {
                if (Current == TokenType.Comma)
                {
                    Take(TokenType.Comma);
                    attributes.Add(ParseAttribute());
                }
            }

            Take(TokenType.RightBrace);

            return attributes;
        }
        private AttributeSyntax ParseAttribute()
        {
            // [Attribute]
            // [Attribute(/* constructor params */)]
            var start = Current.Start;
            var type = Take(TokenType.Identifier);
            var args = new List<Expression>();

            if (Current == TokenType.LeftParenthesis)
            {
                MakeBlock(() =>
                {
                    if (Current != TokenType.RightParenthesis)
                    {
                        args.Add(ParseExpression());

                        while (Current == TokenType.Comma)
                        {
                            Take(TokenType.Comma);
                            args.Add(ParseExpression());
                        }
                    }
                }, 
                TokenType.LeftParenthesis,
                TokenType.RightParenthesis);
            }

            return new AttributeSyntax(CreatePart(type.Start), type.Value, args);
        }

        // Statements
        private ImportStatement ParseImportStatement()
        {
            var token = Take("import");

            var moduleNameParts = new List<IdentifierExpression>();

            while (Current == TokenType.Identifier && Current != TokenType.Semicolon)
            {
                var part = CreatePart(Current.Start);
                var value = ParseIdentifierName();

                moduleNameParts.Add(new IdentifierExpression(part, value));

                if (Current != TokenType.Dot && Current != TokenType.Semicolon)
                    throw UnexpectedToken("'.' or ';'");

                if (Current == TokenType.Dot)
                    Advance();
            }

            Take(TokenType.Semicolon);

            return new ImportStatement(CreatePart(token.Start), moduleNameParts);
        }
        private BlockStatement ParseScope()
        {
            var contents = new List<SyntaxNode>();
            var token = Current;

            MakeBlock(() =>
            {
                contents.Add(ParseStatement());
            });

            return new BlockStatement(CreatePart(token.Start), contents);
        }
        private BlockStatement ParseStatementOrScope()
        {
            if (Current == TokenType.LeftBracket)
            {
                return ParseScope();
            }
            else
            {
                var statement = ParseStatement();

                return new BlockStatement(statement.FilePart, new[] { statement });
            }
        }
        private SyntaxNode ParseStatement()
        {
            SyntaxNode value = null;

            if (Current == TokenType.Keyword)
            {
                switch (Current.Value)
                {
                    case "true":
                    case "false":
                        value = ParseExpression();
                        break;

                    case "if":
                        value = ParseIfStatement();
                        break;

                    case "do":
                        value = ParseDoWhileStatement();
                        break;

                    case "while":
                        value = ParseWhileStatement();
                        break;

                    case "for":
                        value = ParseForStatement();
                        break;

                    case "switch":
                        value = ParseSwitchStatement();
                        break;

                    case "return":
                        value = ParseReturnStatement();
                        break;

                    case "var":
                        value = ParseVariableDeclaration();
                        break;

                    default:
                        // TODO(Dan): Fix this error message!
                        throw UnexpectedToken("if, do, while, for or switch");
                }
            }
            else if (Current == TokenType.Semicolon)
            {
                var token = TakeSemicolon();

                AddError("Possibly mistaken empty statement", CreatePart(token.Start), Severity.Warning);

                return new EmptyStatement(CreatePart(token.Start));
            }
            else
            {
                MakeStatement(() =>
                {
                    value = ParseExpression();
                });

                return value;
            }

            if (Last != TokenType.RightBracket)
            {
                TakeSemicolon();
            }

            return value;
        }
        private IfStatement ParseIfStatement()
        {
            var keyword = TakeKeyword("if");
            var predicate = ParsePredicate();
            var body = ParseStatementOrScope();

            ElseStatement elseStatement = null;

            if (Current == "else")
            {
                elseStatement = ParseElseStatement();
            }

            return new IfStatement(CreatePart(keyword.Start), predicate, body, elseStatement);
        }
        private ElseStatement ParseElseStatement()
        {
            var keyword = TakeKeyword("else");
            var body = ParseStatementOrScope();

            return new ElseStatement(CreatePart(keyword.Start), body);
        }
        private WhileStatement ParseWhileStatement()
        {
            var keyword = TakeKeyword("while");
            var predicate = ParsePredicate();
            var body = ParseStatementOrScope();

            return new WhileStatement(CreatePart(keyword.Start), predicate, body);
        }
        private WhileStatement ParseDoWhileStatement()
        {
            var keyword = TakeKeyword("do");
            var body = ParseStatementOrScope();
            var @while = TakeKeyword("while");
            var predicate = ParsePredicate();

            return new WhileStatement(CreatePart(keyword.Start), predicate, body, WhileStatementType.DoWhile);
        }
        private ForStatement ParseForStatement()
        {
            var keyword = TakeKeyword("for");
            SyntaxNode init = null;
            Expression condition = null;
            Expression increment = null;

            MakeBlock(() =>
            {
                if (Current == "var")
                {
                    // var i = 0;
                    init = ParseVariableDeclaration();
                }
                else if (Current == TokenType.Semicolon)
                {
                    // for (; i < 1; i++)
                    init = new EmptyStatement(CreatePart(Current.Start));
                }
                else
                {
                    init = ParseExpression();
                }

                TakeSemicolon();

                if (Current == TokenType.Semicolon)
                {
                    // for (;;), don't think I need a node in the AST for this
                }
                else
                {
                    condition = ParseLogicalExpression();
                }

                TakeSemicolon();

                if (Current != TokenType.RightParenthesis)
                {
                    increment = ParseExpression();
                }

            }, TokenType.LeftParenthesis, TokenType.RightParenthesis);

            var body = ParseStatementOrScope();

            return new ForStatement(CreatePart(keyword.Start), init, condition, increment);
        }
        private ReturnStatement ParseReturnStatement()
        {
            var keyword = TakeKeyword("return");
            var value = ParseExpression();

            return new ReturnStatement(CreatePart(keyword.Start), value);
        }
        private SwitchStatement ParseSwitchStatement()
        {
            var cases = new List<CaseStatement>();
            var keyword = TakeKeyword("switch");
            Expression condition = null;

            MakeBlock(() => condition = ParseExpression(), TokenType.LeftParenthesis, TokenType.RightParenthesis);
            MakeBlock(() =>
            {
                while (Current == "case" || Current == "default")
                {
                    cases.Add(ParseCaseStatement());
                }
            });

            return new SwitchStatement(CreatePart(keyword.Start), condition, cases);
        }
        private CaseStatement ParseCaseStatement()
        {
            var conditions = new List<Expression>();
            BlockStatement body = null;
            var start = Current;

            while (Current == "case" || Current == "default")
            {
                if (Current == "default")
                {
                    var keyword = TakeKeyword("default");
                    Take(TokenType.Semicolon);

                    // TODO(Dan): Do this better - it's horrible!
                    var part = CreatePart(keyword.Start);
                    var condition = new BinaryExpression(part,
                        new ConstantExpression(part, "true", ConstantType.Boolean),
                        new ConstantExpression(part, "true", ConstantType.Boolean),
                        BinaryOperator.Equal);

                    conditions.Add(condition);
                }
                else
                {
                    Take();
                    var condition = ParseExpression();
                    Take(TokenType.Colon);
                    conditions.Add(condition);
                }
            }

            // TODO(Dan): We aren't verifying that a break / return is used here...
            body = ParseStatementOrScope();

            return new CaseStatement(CreatePart(start.Start), conditions, body);
        }

        // Declarations
        private ModuleDeclaration ParseModuleDeclaration()
        {
            var keyword = TakeKeyword("module");

            var moduleNameParts = new List<IdentifierExpression>();
            var classes = new List<ClassDeclaration>();
            var enums = new List<EnumDeclaration>();
            var methods = new List<MethodDeclaration>();

            while (Current == TokenType.Identifier)
            {
                moduleNameParts.Add(new IdentifierExpression(CreatePart(Current.Start), ParseIdentifierName()));

                if (Current == TokenType.Dot)
                    Advance();
            }

            MakeBlock(() =>
            {
                while (Current == "class" || Current == "enum" || Current == TokenType.LeftBrace || Current == TokenType.Identifier || Current == TokenType.Keyword)
                {
                    if (Current == "class" || Current == TokenType.LeftBrace)
                    {
                        classes.Add(ParseClassDeclaration());
                    }
                    else if (Current == "enum")
                    {
                        enums.Add(ParseEnumDeclaration());
                    }
                    else
                    {
                        var returnType = ParseTypeDeclaration();
                        var name = ParseIdentifierName();
                        methods.Add(ParseMethodDeclaration(name, returnType));
                    }
                }
            });

            return new ModuleDeclaration(CreatePart(keyword.Start), string.Join(".", moduleNameParts.Select(identifier => identifier.Name)), classes, methods, enums);
        }
        private ClassDeclaration ParseClassDeclaration()
        {
            var attributes = new List<AttributeSyntax>();
            var constructors = new List<ConstructorDeclaration>();
            var fields = new List<FieldDeclaration>();
            var methods = new List<MethodDeclaration>();
            var properties = new List<PropertyDeclaration>();

            while(Current == TokenType.LeftBrace)
                attributes.AddRange(ParseAttributes());

            var keyword = TakeKeyword("class");
            var name = ParseIdentifierName();

            MakeBlock(() =>
            {
                var member = ParseClassMember();

                switch (member)
                {
                    case ConstructorDeclaration constructor:
                        constructors.Add(constructor);
                        break;
                    case FieldDeclaration field:
                        fields.Add(field);
                        break;
                    case MethodDeclaration method:
                        methods.Add(method);
                        break;
                    case PropertyDeclaration property:
                        properties.Add(property);
                        break;
                }
            });

            return new ClassDeclaration(CreatePart(keyword.Start), name, fields, properties, methods, constructors);
        }
        private SyntaxNode ParseClassMember()
        {
            if (Current == "constructor")
                return ParseConstructorDeclaration();

            var returnType = ParseTypeDeclaration();
            var name = ParseIdentifierName();

            switch (Current.Value)
            {
                case "{":
                case "=>":
                    return ParsePropertyDeclaration(name, returnType);

                case "(":
                case "<":
                    return ParseMethodDeclaration(name, returnType);

                case ";":
                case "=":
                    return ParseFieldDeclaration(name, returnType);

                default:
                    throw UnexpectedToken("Field, Property or Method Declaration: '{', '=>', '(', '<', ';', '='");
            }
        }
        private ConstructorDeclaration ParseConstructorDeclaration()
        {
            var keyword = TakeKeyword("constructor");
            var parameters = ParseParameters();
            var body = ParseScope();

            return new ConstructorDeclaration(CreatePart(keyword.Start), keyword.Value, parameters, body);
        }
        private MethodDeclaration ParseMethodDeclaration(string name, TypeDeclaration returnType)
        {
            var token = Current;
            var parameters = ParseParameters();

            if (Current == TokenType.FatArrow)
            {
                var method = ParseExpressionBodiedMember(name, returnType, parameters);

                TakeSemicolon();

                return method;
            }

            var body = ParseScope();

            return new MethodDeclaration(CreatePart(token.Start), name, returnType, parameters, body);
        }
        private FieldDeclaration ParseFieldDeclaration(string name, TypeDeclaration returnType)
        {
            var token = Current;

            Expression defaultValue = null;

            if (Current == TokenType.Assignment)
            {
                Take();
                defaultValue = ParseExpression();
            }

            TakeSemicolon();

            return new FieldDeclaration(CreatePart(token.Start), name, returnType, defaultValue);
        }
        private PropertyDeclaration ParsePropertyDeclaration(string name, TypeDeclaration returnType)
        {
            var token = Current;

            MethodDeclaration getMethod = null;
            MethodDeclaration setMethod = null;

            if (Current == TokenType.FatArrow)
            {
                getMethod = ParseExpressionBodiedMember($"get_{name}", returnType, Enumerable.Empty<ParameterDeclaration>());
                TakeSemicolon();
            }
            else
            {

                MakeBlock(() =>
                {
                    switch (Current.Value)
                    {
                        case "get":
                            {
                                var get = Take();

                                switch (Current.TokenType)
                                {
                                    // TODO(Dan): Allow { get; set; }
                                    case TokenType.Semicolon:
                                        {
                                            //var part = CreatePart(get.Start);
                                            //// TODO(Dan): Need to add this compiler generated field to the AST
                                            //var backingField = new FieldDeclaration(part, $"_{name.FirstToLower()}", returnType, null);
                                            //var body = new BlockStatement(part, new[] { new ReturnStatement(CreatePart(get.Start), new IdentifierExpression(part, $"_{name.ToLower()}")) });
                                            //getMethod = new MethodDeclaration(part, $"get_{name}", returnType, Enumerable.Empty<ParameterDeclaration>(), body, new[] { new AttributeSyntax("CompilerGenerated", new SourceFilePart()) });
                                            TakeSemicolon();
                                            break;
                                        }
                                        
                                    case TokenType.FatArrow:
                                        {
                                            getMethod = ParseExpressionBodiedMember($"get_{name}", returnType, Enumerable.Empty<ParameterDeclaration>());
                                            TakeSemicolon();
                                            break;
                                        }

                                    default:
                                        {
                                            var body = ParseScope();

                                            if (getMethod != null)
                                                AddError($"Multiple getters for property: {name}", CreatePart(get.Start), Severity.Error);
                                            else
                                                getMethod = new MethodDeclaration(CreatePart(get.Start), $"get_{name}", returnType, Enumerable.Empty<ParameterDeclaration>(), body);

                                        }
                                        break;
                                    }

                                break;
                            }
                        case "set":
                            {
                                var set = Take();

                                switch (Current.TokenType)
                                {
                                    case TokenType.Semicolon:
                                        {
                                            //var part = CreatePart(set.Start);
                                            //var backingField = new FieldDeclaration(part, $"_{name.FirstToLower()}", returnType, null);
                                            //var body = new BlockStatement(part, new[] { new BinaryExpression(part, new IdentifierExpression(part, backingField.Name), new IdentifierExpression(part, "value"), BinaryOperator.Assign) });
                                            //var methodDeclaration = new MethodDeclaration(CreatePart(set.Start), $"set_{name}", new TypeDeclaration(new SourceFilePart(null, null, null, null), "void"), new[] { new ParameterDeclaration(part, "value", returnType) }, body);
                                            TakeSemicolon();
                                            break;
                                        }

                                    // TODO(Dan): Allow { get => _value; set => _value = value; }
                                    case TokenType.FatArrow:
                                        {
                                            setMethod = ParseExpressionBodiedMember($"set_{name}", returnType, Enumerable.Empty<ParameterDeclaration>());
                                            TakeSemicolon();
                                            break;
                                        }

                                    default:
                                        {
                                            var body = ParseScope();

                                            if (getMethod != null)
                                                AddError($"Multiple getters for property: {name}", CreatePart(set.Start), Severity.Error);
                                            else
                                                setMethod = new MethodDeclaration(CreatePart(set.Start), $"set_{name}", new TypeDeclaration(new SourceFilePart(null, null, null, null), "void"), new[] { new ParameterDeclaration(CreatePart(set.Start), "value", returnType) }, body);

                                        }
                                        break;
                                }

                                break;
                            }
                        default:
                            throw UnexpectedToken("get or set");
                    }
                });
            }

            if (Current == TokenType.Semicolon)
            {
                var semicolon = TakeSemicolon();
                AddError("Possibly mistaken empty statement", CreatePart(semicolon.Start, semicolon.End), Severity.Warning);
            }

            //if (getMethod == null)
            //    AddError($"Property '{name}' does not have a getter", CreatePart(token.Start), Severity.Error);

            return new PropertyDeclaration(CreatePart(token.Start), name, returnType, getMethod, setMethod);
        }
        private TypeDeclaration ParseTypeDeclaration()
        {
            var token = Current;
            var name = ParseIdentifierName();

            while (Current == TokenType.Dot)
            {
                Advance();

                name += ".";
                name += ParseIdentifierName();
            }

            // TODO(Dan): Allow things like: int[]

            return new TypeDeclaration(CreatePart(token.Start), name);
        }
        private IEnumerable<ParameterDeclaration> ParseParameters()
        {
            var parameters = new List<ParameterDeclaration>();

            MakeBlock(() =>
            {
                if (Current == TokenType.RightParenthesis)
                    return;

                parameters.Add(ParseParameterDeclaration());

                while (Current == TokenType.Comma)
                {
                    Take(TokenType.Comma);
                    parameters.Add(ParseParameterDeclaration());
                }
            }, TokenType.LeftParenthesis, TokenType.RightParenthesis);

            return parameters;
        }
        private ParameterDeclaration ParseParameterDeclaration()
        {
            var type = ParseTypeDeclaration();
            var name = Take(TokenType.Identifier);

            return new ParameterDeclaration(CreatePart(name.Start), name.Value, type);
        }
        private VariableDeclaration ParseVariableDeclaration()
        {
            var start = TakeKeyword("var");
            var isMutable = Peek(1) == "mut";

            if (isMutable)
                TakeKeyword("mut");
            
            var name = ParseIdentifierName();
            var type = TypeDeclaration.Empty;// This will be inferred later
            Expression value = null;

            if (Current == TokenType.Assignment)
            {
                Take();

                value = ParseExpression();
            }

            var mutabilityType = isMutable
                ? VariableMutabilityType.Mutable
                : VariableMutabilityType.Immutable;

            return new VariableDeclaration(CreatePart(start.Start), name, type, value, mutabilityType);
        }
        private MethodDeclaration ParseExpressionBodiedMember(string methodName, TypeDeclaration returnType, IEnumerable<ParameterDeclaration> parameters)
        {
            Take(TokenType.FatArrow);

            var expression = ParseExpression();
            var span = CreatePart(expression.FilePart.Start, expression.FilePart.End);

            ReturnStatement returnStatement = null;

            if (returnType.Name != "void")
                returnStatement = new ReturnStatement(span, expression);

            return new MethodDeclaration(span, methodName, returnType, parameters, new BlockStatement(span, new[] { (SyntaxNode)returnStatement ?? expression }));
        }
        private EnumDeclaration ParseEnumDeclaration()
        {
            var attributes = new List<AttributeSyntax>();

            while(Current == TokenType.LeftBrace)
                attributes.AddRange(ParseAttributes());

            var start = TakeKeyword("enum");
            var name = ParseIdentifierName();
            var members = new List<EnumMemberDeclaration>();

            MakeBlock(() =>
            {
                members.Add(ParseEnumMemberDeclaration());

                if (Current == TokenType.Comma && Next != TokenType.RightBracket)
                    Take(TokenType.Comma);
            });

            return new EnumDeclaration(CreatePart(start.Start), name, members, attributes);
        }
        private EnumMemberDeclaration ParseEnumMemberDeclaration()
        {
            var attributes = new List<AttributeSyntax>();

            while (Current == TokenType.LeftBrace)
                attributes.AddRange(ParseAttributes());

            var name = Take(TokenType.Identifier);
            Expression value = null;

            if (Current == TokenType.Assignment)
            {
                Take(TokenType.Assignment);
                value = ParseExpression();
            }

            return new EnumMemberDeclaration(CreatePart(name.Start), name.Value, value, attributes);
        }

        // TODO(Dan): Enums

        // Expressions - Urgh make sure I get the precedence right here...
        private bool IsAdditiveOperator()
        {
            switch (Current.TokenType)
            {
                case TokenType.Plus:
                case TokenType.Minus:
                    return true;

                default:
                    return false;
            }
        }
        private bool IsAssignmentOperator()
        {
            switch (Current.TokenType)
            {
                case TokenType.Assignment:
                case TokenType.PlusEqual:
                case TokenType.MinusEqual:
                case TokenType.MulEqual:
                case TokenType.DivEqual:
                case TokenType.ModEqual:
                case TokenType.BitwiseAndEqual:
                case TokenType.BitwiseOrEqual:
                case TokenType.BitwiseXorEqual:
                    return true;

                default:
                    return false;
            }
        }
        private bool IsBitwiseOperator()
        {
            switch(Current.TokenType)
            {
                case TokenType.BitwiseOr:
                case TokenType.BitwiseAnd:
                case TokenType.BitwiseXor:
                    return true;

                default:
                    return false;
            }
        }
        private bool IsEqualityOperator()
        {
            switch(Current.TokenType)
            {
                case TokenType.Equal:
                case TokenType.NotEqual:
                    return true;

                default:
                    return false;
            }
        }
        private bool IsLogicalOperator()
        {
            switch (Current.TokenType)
            {
                case TokenType.BooleanAnd:
                case TokenType.BooleanOr:
                    return true;

                default:
                    return false;
            }
        }
        private bool IsMultiplicativeOperator()
        {
            switch (Current.TokenType)
            {
                case TokenType.Mul:
                case TokenType.Div:
                case TokenType.Mod:
                    return true;

                default:
                    return false;
            }
        }
        private bool IsPrefixUnaryOperator()
        {
            switch (Current.TokenType)
            {
                case TokenType.PlusPlus:
                case TokenType.MinusMinus:
                case TokenType.Not:
                case TokenType.Minus:
                    return true;

                default:
                    return false;
            }
        }
        private bool IsSuffixUnaryOperator()
        {
            switch (Current.TokenType)
            {
                case TokenType.PlusPlus:
                case TokenType.MinusMinus:
                    return true;

                default:
                    return false;
            }
        }
        private bool IsShiftOperator()
        {
            switch (Current.TokenType)
            {
                case TokenType.BitShiftLeft:
                case TokenType.BitShiftRight:
                    return true;

                default:
                    return false;
            }
        }
        private bool IsRelationalOperator()
        {
            switch (Current.TokenType)
            {
                case TokenType.GreaterThan:
                case TokenType.GreaterThanOrEqual:
                case TokenType.LessThan:
                case TokenType.LessThanOrEqual:
                    return true;

                default:
                    return false;
            }
        }

        private BinaryOperator ParseBinaryOperator()
        {
            var token = Take();

            switch(token.TokenType)
            {
                case TokenType.Plus: return BinaryOperator.Add;
                case TokenType.Minus: return BinaryOperator.Sub;
                case TokenType.Assignment: return BinaryOperator.Assign;
                case TokenType.PlusEqual: return BinaryOperator.AddAssign;
                case TokenType.MinusEqual: return BinaryOperator.SubAssign;
                case TokenType.MulEqual: return BinaryOperator.MulAssign;
                case TokenType.DivEqual: return BinaryOperator.DivAssign;
                case TokenType.ModEqual: return BinaryOperator.ModAssign;
                case TokenType.BitwiseAndEqual: return BinaryOperator.AndAssign;
                case TokenType.BitwiseOrEqual: return BinaryOperator.OrAssign;
                case TokenType.BitwiseXorEqual: return BinaryOperator.XorAssign;
                case TokenType.BitwiseAnd: return BinaryOperator.BitwiseAnd;
                case TokenType.BitwiseOr: return BinaryOperator.BitwiseOr;
                case TokenType.BitwiseXor: return BinaryOperator.BitwiseXor;
                case TokenType.Equal: return BinaryOperator.Equal;
                case TokenType.NotEqual: return BinaryOperator.NotEqual;
                case TokenType.BooleanAnd: return BinaryOperator.LogicalAnd;
                case TokenType.BooleanOr: return BinaryOperator.LogicalOr;
                case TokenType.Mul: return BinaryOperator.Mul;
                case TokenType.Div: return BinaryOperator.Div;
                case TokenType.Mod: return BinaryOperator.Mod;
                case TokenType.GreaterThan: return BinaryOperator.GreaterThan;
                case TokenType.LessThan: return BinaryOperator.LessThan;
                case TokenType.GreaterThanOrEqual: return BinaryOperator.GreaterThanOrEqual;
                case TokenType.LessThanOrEqual: return BinaryOperator.LessThanOrEqual;
                case TokenType.BitShiftLeft: return BinaryOperator.LeftShift;
                case TokenType.BitShiftRight: return BinaryOperator.RightShift;
            }

            _index--;

            throw UnexpectedToken("Binary operator");
        }
        private UnaryOperator ParsePrefixUnaryOperator()
        {
            var token = Current;
            Advance();

            switch(token.TokenType)
            {
                case TokenType.PlusPlus: return UnaryOperator.PreIncrement;
                case TokenType.MinusMinus: return UnaryOperator.PreDecrement;
                case TokenType.Not: return UnaryOperator.Not;
                case TokenType.Minus: return UnaryOperator.Negation;
            }

            _index--;
            throw UnexpectedToken("Unary operator");
        }
        private UnaryOperator ParseSuffixUnaryOperator()
        {
            var token = Current;
            Advance();

            switch (token.TokenType)
            {
                case TokenType.PlusPlus: return UnaryOperator.PostIncrement;
                case TokenType.MinusMinus: return UnaryOperator.PostDecrement;
            }

            _index--;
            throw UnexpectedToken("Unary operator");
        }

        private Expression ParsePredicate()
        {
            Expression expression = null;

            MakeBlock(() =>
            {
                expression = ParseLogicalExpression();
            }, 
            TokenType.LeftParenthesis, 
            TokenType.RightParenthesis);

            return expression;
        }
        private Expression ParseExpression()
        {
            return ParseAssignmentExpression();
        }
        private Expression ParseAssignmentExpression()
        {
            var left = ParseLogicalExpression();

            if (IsAssignmentOperator())
            {
                // Assignment is right associative!
                var op = ParseBinaryOperator();
                var right = ParseAssignmentExpression();

                return new BinaryExpression(CreatePart(left.FilePart.Start), left, right, op);
            }

            return left;
        }
        private Expression ParseLogicalExpression()
        {
            var left = ParseEqualityExpression();

            while (IsLogicalOperator())
            {
                var op = ParseBinaryOperator();
                var right = ParseEqualityExpression();

                left = new BinaryExpression(CreatePart(left.FilePart.Start), left, right, op);
            }

            return left;
        }
        private Expression ParseEqualityExpression()
        {
            var left = ParseRelationalExpression();

            while (IsEqualityOperator())
            {
                var op = ParseBinaryOperator();
                var right = ParseRelationalExpression();

                left = new BinaryExpression(CreatePart(left.FilePart.Start), left, right, op);
            }

            return left;
        }
        private Expression ParseRelationalExpression()
        {
            var left = ParseBitwiseExpression();

            while (IsRelationalOperator())
            {
                var op = ParseBinaryOperator();
                var right = ParseBitwiseExpression();

                left = new BinaryExpression(CreatePart(left.FilePart.Start), left, right, op);
            }

            return left;
        }
        private Expression ParseBitwiseExpression()
        {
            var left = ParseShiftExpression();

            while (IsBitwiseOperator())
            {
                var op = ParseBinaryOperator();
                var right = ParseShiftExpression();

                left = new BinaryExpression(CreatePart(left.FilePart.Start), left, right, op);
            }

            return left;
        }
        private Expression ParseShiftExpression()
        {
            var left = ParseAdditiveExpression();

            while (IsShiftOperator())
            {
                var op = ParseBinaryOperator();
                var right = ParseAdditiveExpression();

                left = new BinaryExpression(CreatePart(left.FilePart.Start), left, right, op);
            }

            return left;
        }
        private Expression ParseAdditiveExpression()
        {
            var left = ParseMultiplicativeExpression();

            while (IsAdditiveOperator())
            {
                var op = ParseBinaryOperator();
                var right = ParseMultiplicativeExpression();

                left = new BinaryExpression(CreatePart(left.FilePart.Start), left, right, op);
            }

            return left;
        }
        private Expression ParseMultiplicativeExpression()
        {
            var left = ParseUnaryExpression();

            while (IsMultiplicativeOperator())
            {
                var op = ParseBinaryOperator();
                var right = ParseUnaryExpression();

                left = new BinaryExpression(CreatePart(left.FilePart.Start), left, right, op);
            }

            return left;
        }
        private Expression ParseUnaryExpression()
        {
            var op = UnaryOperator.Default;
            var start = Current.Start;

            if (IsPrefixUnaryOperator())
            {
                op = ParsePrefixUnaryOperator();
            }

            var expression = ParsePrimaryExpression();

            if (IsSuffixUnaryOperator())
            {
                op = ParseSuffixUnaryOperator();
            }

            if (op != UnaryOperator.Default)
            {
                return new UnaryExpression(CreatePart(start), expression, op);
            }

            return expression;
        }
        private Expression ParsePrimaryExpression()
        {
            if (Current == TokenType.Identifier)
            {
                if (Next == TokenType.Dot)
                {
                    return ParseReferenceExpression();
                }
                else if (Next == TokenType.LeftParenthesis)
                {
                    return ParseMethodCallExpression();
                }
                else if (Next == TokenType.LeftBrace)
                {
                    return ParseArrayAccessExpression(ParseIdentifierExpression());
                }

                return ParseIdentifierExpression();
            }
            else if (Current.Category == TokenCategory.Constant || Current == "true" || Current == "false")
            {
                return ParseConstantExpression();
            }
            else if (Current == TokenType.LeftParenthesis)
            {
                return ParseOverrideExpression();
            }
            else if (Current == "new")
            {
                return ParseNewExpression();
            }
            else if (Current == TokenType.Semicolon && Last == "return")
            {
                // TODO(Dan): Is this actually OK?
                return null;
            }
            else
            {
                if (Current.Category == TokenCategory.Operator)
                {
                    var token = Current;

                    Advance();

                    throw SyntaxError($"'{token.Value}' is an invalid expression term", CreatePart(token.Start), Severity.Error);
                }

                throw UnexpectedToken("Expression term");
            }
        }
        private Expression ParseReferenceExpression()
        {
            var hint = ParseIdentifierExpression();
            return ParseReferenceExpression(hint);
        }
        private Expression ParseReferenceExpression(Expression hint)
        {
            var references = new List<Expression>();

            references.Add(hint);

            do
            {
                Take(TokenType.Dot);

                if (Current == TokenType.Identifier)
                {
                    var expression = ParseIdentifierExpression();
                    references.Add(expression);
                }

                if (Current == TokenType.LeftParenthesis)
                {
                    var expression = new ReferenceExpression(CreatePart(hint.FilePart.Start), references);
                    return ParseMethodCallExpression(expression);
                }
                else if (Current == TokenType.LeftBrace)
                {
                    var expression = new ReferenceExpression(CreatePart(hint.FilePart.Start), references);
                    return ParseArrayAccessExpression(expression);
                }

            } while (Current == TokenType.Dot);

            return new ReferenceExpression(CreatePart(hint.FilePart.Start), references);
        }
        private Expression ParseIdentifierExpression()
        {
            var token = Take(TokenType.Identifier);

            return new IdentifierExpression(CreatePart(token.Start), token.Value);
        }
        private Expression ParseMethodCallExpression()
        {
            var hint = ParseIdentifierExpression();
            return ParseMethodCallExpression(hint);
        }
        private Expression ParseMethodCallExpression(Expression hint)
        {
            var args = new List<Expression>();

            MakeBlock(() =>
            {
                if (Current != TokenType.RightParenthesis)
                {
                    args.Add(ParseExpression());

                    while(Current == TokenType.Comma)
                    {
                        Take(TokenType.Comma);
                        args.Add(ParseExpression());
                    }
                }
            },
            TokenType.LeftParenthesis,
            TokenType.RightParenthesis);

            var expression = new MethodCallExpression(CreatePart(hint.FilePart.Start), hint, args);

            if (Current == TokenType.Dot)
            {
                return ParseReferenceExpression(expression);
            }

            return expression;
        }
        private Expression ParseArrayAccessExpression(Expression hint)
        {
            var args = new List<Expression>();

            MakeBlock(() =>
            {
                args.Add(ParseExpression());

                while (Current == TokenType.Comma)
                {
                    Take(TokenType.Comma);
                    args.Add(ParseExpression());
                }
            }, 
            TokenType.LeftBrace, 
            TokenType.RightBrace);

            var expression = new ArrayAccessExpression(CreatePart(hint.FilePart.Start), hint, args);

            if (Current == TokenType.Dot)
            {
                return ParseReferenceExpression(expression);
            }

            return expression;
        }
        private Expression ParseConstantExpression()
        {
            var kind = ConstantType.Invalid;

            if (Current == "true" || Current == "false")
            {
                kind = ConstantType.Boolean;
            }
            else if (Current == TokenType.StringLiteral)
            {
                kind = ConstantType.String;
            }
            else if (Current == TokenType.IntegerLiteral)
            {
                kind = ConstantType.Integer;
            }
            else if (Current == TokenType.RealLiteral)
            {
                kind = ConstantType.Real;
            }
            else
            {
                throw UnexpectedToken("Constant");
            }

            var token = Take();
            var expression = new ConstantExpression(CreatePart(token.Start), token.Value, kind);

            if (Current == TokenType.Dot)
            {
                return ParseReferenceExpression(expression);
            }

            return expression;
        }
        private Expression ParseOverrideExpression()
        {
            var start = Take(TokenType.LeftParenthesis).Start;

            if (Current == TokenType.RightParenthesis)
            {
                Take(TokenType.RightParenthesis);
                return ParseLambdaExpression(start, new ParameterDeclaration[0]);
            }

            if (Current == TokenType.Identifier && (Next == TokenType.Comma || (Next == TokenType.RightParenthesis && Peek(2) == TokenType.FatArrow) || Next == TokenType.Colon))
            {
                Rewind();
                var parameters = ParseParameters();

                return ParseLambdaExpression(start, parameters);
            }

            var expression = ParseExpression();

            Take(TokenType.RightParenthesis);

            if (Current == TokenType.LeftParenthesis)
                return ParseMethodCallExpression();

            if (Current == TokenType.Dot)
                return ParseReferenceExpression(expression);

            return expression;
        }
        private Expression ParseNewExpression()
        {
            var start = TakeKeyword("new");
            var references = new List<Expression>();
            var arguments = new List<Expression>();

            references.Add(ParseIdentifierExpression());

            while (Current == TokenType.Dot)
            {
                Take(TokenType.Dot);
                references.Add(ParseIdentifierExpression());
            }

            Expression reference = null;

            if (references.Count == 1)
                reference = references.First();
            else
                reference = new ReferenceExpression(CreatePart(references.First().FilePart.Start), references);

            MakeBlock(() =>
            {
                if (Current != TokenType.RightParenthesis)
                {
                    arguments.Add(ParseExpression());

                    while (Current == TokenType.Comma)
                        arguments.Add(ParseExpression());
                }
            },
            TokenType.LeftParenthesis,
            TokenType.RightParenthesis);

            var expression = new NewExpression(CreatePart(start.Start), reference, arguments);

            if (Current == TokenType.Dot)
                return ParseReferenceExpression(expression);

            return expression;
        }
        private Expression ParseLambdaExpression(SourceFileLocation start, IEnumerable<ParameterDeclaration> arguments)
        {
            Take(TokenType.FatArrow);

            var body = ParseStatementOrScope();

            if (Current != TokenType.Semicolon)
                throw UnexpectedToken(TokenType.Semicolon);

            return new LambdaExpression(CreatePart(start), arguments, body);
        }

        // Errors
        private void AddError(string message, SourceFilePart part, Severity severity)
        {
            _errorSink.AddError($"{message} in '{_currentSourceFile.Name}'", part, severity);
        }
        private SyntaxException UnexpectedToken(TokenType expected)
        {
            return UnexpectedToken($"{expected.Value()}");
        }
        private SyntaxException UnexpectedToken(string expected)
        {
            Advance();

            var value = string.IsNullOrEmpty(Last?.Value)
                ? Last?.TokenType.ToString()
                : Last?.Value;

            var message = $"Unexpected '{value}'. Expected {expected}";

            return SyntaxError(message, CreatePart(Last.Start, Current.End), Severity.Error);
        }
        private SyntaxException SyntaxError(string message, SourceFilePart part, Severity severity)
        {
            _error = true;
            AddError(message, part, severity);
            return new SyntaxException(message);
        }

        // Helpers / Utilities
        private Token Peek(int ahead)
        {
            return _tokens.ElementAtOrDefault(_index + ahead) ?? _tokens.Last();
        }
        private void Advance()
        {
            _index++;
        }
        private void Rewind()
        {
            _index--;
        }
        private bool IsMakingProgress(int lastTokenPosition)
        {
            if (_index > lastTokenPosition)
                return true;

            return false;
        }
        private Token Take()
        {
            var token = Current;

            Advance();

            return token;
        }
        private Token Take(TokenType type)
        {
            if (Current != type)
                throw UnexpectedToken(type);

            return Take();
        }
        private Token Take(string contextualKeyword)
        {
            if (Current != TokenType.Identifier && Current != contextualKeyword)
                throw UnexpectedToken(contextualKeyword);

            return Take();
        }
        private Token TakeKeyword(string keyword)
        {
            if (Current != TokenType.Keyword && Current != keyword)
                throw UnexpectedToken(keyword);

            return Take();
        }
        private Token TakeSemicolon()
        {
            return Take(TokenType.Semicolon);
        }
        private SourceFilePart CreatePart(SourceFileLocation start)
        {
            return CreatePart(start, Current.End);
        }
        private SourceFilePart CreatePart(SourceFileLocation start, SourceFileLocation end)
        {
            var skip = start.LineNumber - 1;
            var take = (end.LineNumber - start.LineNumber) < 1 ? 1 : (end.LineNumber - start.LineNumber);
            var content = _currentSourceFile.Lines.Skip(start.LineNumber - 1).Take(take).ToArray();
            return new SourceFilePart(_currentSourceFile.Name, start, end, content);
        }
        private string ParseIdentifierName()
        {
            if (Current == TokenType.Identifier)
                return Take(TokenType.Identifier).Value;

            if (Current == TokenType.Keyword && new[] { "int", "string", "void", "float", "double", "decimal", "char" }.Contains(Current.Value))
                return Take(TokenType.Keyword).Value;

            throw UnexpectedToken("Identifier");
        }
        private void MakeBlock(Action action, TokenType open = TokenType.LeftBracket, TokenType close = TokenType.RightBracket)
        {
            Take(open);

            MakeStatement(action, close);
        }
        private void MakeStatement(Action action, TokenType close = TokenType.Semicolon)
        {
            try
            {
                var startIndex = _index;

                while (Current != close && Current != TokenType.EOF)
                {
                    action();

                    if (!IsMakingProgress(startIndex))
                        throw SyntaxError($"Unexpected '{Current.Value}'", CreatePart(Current.Start), Severity.Error);

                    startIndex = _index;
                }
            }
            catch (SyntaxException)
            {
                while (Current != close && Current != TokenType.EOF)
                    Take();
            }
            finally
            {
                // TODO(Dan): Improve error recovery - this will be awful!
                if (_error)
                {
                    if (Last == close)
                        _index--;

                    if (Current != close || Next == close)
                    {
                        if (Next == close)
                            Take();

                        while (Current != close && Current != TokenType.EOF)
                            Take();
                    }

                    _error = false;
                }

                if (close == TokenType.Semicolon)
                    TakeSemicolon();
                else
                    Take(close);
            }
        }

        public SyntaxParser()
            : this(new Tokenizer(TokenizerGrammar.Default))
        { }
        public SyntaxParser(Tokenizer tokenizer)
            : this(tokenizer, tokenizer.ErrorSink)
        {
        }
        public SyntaxParser(Tokenizer tokenizer, ErrorSink errorSink)
        {
            if (tokenizer == null)
                throw new ArgumentNullException(nameof(tokenizer));

            if (errorSink == null)
                throw new ArgumentNullException(nameof(errorSink));

            _tokenizer = tokenizer;
            _errorSink = errorSink;
        }
    }
}
