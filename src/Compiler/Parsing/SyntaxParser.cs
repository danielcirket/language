using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Compiler.Lexing;
using Compiler.Parsing.Syntax;
using Compiler.Parsing.Syntax.Declarations;
using Compiler.Parsing.Syntax.Expressions;
using Compiler.Parsing.Syntax.Expressions.Types;
using Compiler.Parsing.Syntax.Statements;

namespace Compiler.Parsing
{
    internal class SyntaxParser
    {
        private readonly Tokenizer _tokenizer;
        private readonly ErrorSink _errorSink;
        private SourceFile _currentSourceFile;
        private TokenStream _tokenStream;
        private Stack<List<TypeExpression>> _genericTypes;
        private bool _error;

        public ErrorSink ErrorSink => _errorSink;
        private Token Current => _tokenStream.Current;
        private Token Next => _tokenStream.Next;
        private Token Last => _tokenStream.Last;

        public CompilationRoot Parse(SourceFile sourceFile)
        {
            if (sourceFile == null)
                throw new ArgumentNullException(nameof(sourceFile));

            return Parse(new[] { sourceFile });
        }
        public CompilationRoot Parse(IEnumerable<SourceFile> sourceFiles)
        {
            if (sourceFiles == null)
                throw new ArgumentNullException(nameof(sourceFiles));

            var compilationUnits = new List<CompilationUnit>();

            foreach(var sourceFile in sourceFiles)
            {
                try
                {
                    _currentSourceFile = sourceFile;
                    _tokenStream = new TokenStream(sourceFile, _tokenizer);

                    // NOTE(Dan): If we have errors already just bail? Perhaps we want to actually try parsing instead?
                    //if (_errorSink.HasErrors)
                    //    continue;
                    compilationUnits.Add(ParseInternal());
                }
                catch (SyntaxException ex)
                { 
                    throw new InternalCompilerError("Internal compiler error. See inner exception for more details", ex);
                }
            }

            return new CompilationRoot(compilationUnits);
        }
        private CompilationUnit ParseInternal()
        {
            var imports = new List<ImportStatement>();
            var modules = new List<ModuleDeclaration>();

            var start = Current.Start;

            while (Current == TokenType.ImportKeyword)
                imports.Add(ParseImportStatement());

            while (Current == TokenType.ModuleKeyword)
                modules.Add(ParseModuleDeclaration());

            if (Current != TokenType.EOF)
                AddError("Top-level statements are not permitted. Statements must be part of a module with the exception of import statements which are at the start of the file", CreatePart(Current.Start), Severity.Error);

            return new CompilationUnit(imports, modules);
        }
        private IEnumerable<AttributeSyntax> ParseAttributes()
        {
            var attributes = new List<AttributeSyntax>();

            if (Current != TokenType.LeftBrace)
                return attributes;

            while (Current == TokenType.LeftBrace)
            {
                Take(TokenType.LeftBrace);

                attributes.Add(ParseAttribute());

                while (Current != TokenType.RightBrace)
                {
                    if (Current == TokenType.Comma)
                    {
                        Take(TokenType.Comma);
                        attributes.Add(ParseAttribute());
                    }
                }

                Take(TokenType.RightBrace);
            }
            
            return attributes;
        }
        private AttributeSyntax ParseAttribute()
        {
            // [Attribute]
            // [Attribute(/* constructor params */)]
            var start = Current.Start;
            // TODO(Dan): This needs to be parse type
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
        private IEnumerable<IdentifierExpression> ParseBaseInheritors()
        {
            Take(TokenType.Colon);

            var identifiers = new List<IdentifierExpression>();
            var start = ParseIdentifierExpression();

            identifiers.Add((IdentifierExpression)start);

            while (Current == TokenType.Comma)
            {
                var comma = Take(TokenType.Comma);

                if (Current != TokenType.Identifier)
                {
                    AddError($"Type expected", CreatePart(comma.Start), Severity.Error);
                    continue;
                }

                identifiers.Add((IdentifierExpression)ParseIdentifierExpression());
            }

            return identifiers;
        }
        private SyntaxModifier ParseModifier()
        {
            switch (Current.TokenType)
            {
                case TokenType.PublicKeyword:
                    Take(TokenType.PublicKeyword);
                    return SyntaxModifier.Public;

                case TokenType.InternalKeyword:
                    Take(TokenType.InternalKeyword);
                    return SyntaxModifier.Internal;

                case TokenType.PrivateKeyword:
                    Take(TokenType.PrivateKeyword);
                    return SyntaxModifier.Private;

                default:
                    return SyntaxModifier.None;
            }
        }

        // Statements
        private ImportStatement ParseImportStatement()
        {
            var token = Take(TokenType.ImportKeyword);

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

            TakeSemicolon();

            return new ImportStatement(CreatePart(token.Start, token.End), moduleNameParts);
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

            switch (Current.TokenType)
            {
                case TokenType.TrueKeyword:
                case TokenType.FalseKeyword:
                    value = ParseExpression();
                    break;

                case TokenType.IfKeyword:
                    value = ParseIfStatement();
                    break;

                case TokenType.DoKeyword:
                    value = ParseDoWhileStatement();
                    break;

                case TokenType.WhileKeyword:
                    value = ParseWhileStatement();
                    break;

                case TokenType.ForKeyword:
                    value = ParseForStatement();
                    break;

                case TokenType.SwitchKeyword:
                    value = ParseSwitchStatement();
                    break;

                case TokenType.ReturnKeyword:
                    value = ParseReturnStatement();
                    break;

                case TokenType.ConstKeyword:
                case TokenType.LetKeyword:
                    value = ParseVariableDeclaration();
                    break;

                default:
                    {
                        if (Current == TokenType.Semicolon)
                        {
                            AddError("Possibly mistaken empty statement", CreatePart(Current.Start), Severity.Warning);

                            return new EmptyStatement(CreatePart(Current.Start));
                        }
                        else
                        {
                            MakeStatement(() =>
                            {
                                value = ParseExpression();
                            });

                            return value;
                        }
                    }
                    // TODO(Dan): Fix this error message!
                    //throw UnexpectedToken("Statement");
            }

            if (Last != TokenType.RightBracket && Current == TokenType.Semicolon)
                TakeSemicolon();

            return value;
        }
        private IfStatement ParseIfStatement()
        {
            var keyword = Take(TokenType.IfKeyword);
            var predicate = ParsePredicate();
            var body = ParseStatementOrScope();

            ElseStatement elseStatement = null;

            if (Current == TokenType.ElseKeyword)
            {
                elseStatement = ParseElseStatement();
            }

            return new IfStatement(CreatePart(keyword.Start), predicate, body, elseStatement);
        }
        private ElseStatement ParseElseStatement()
        {
            var keyword = Take(TokenType.ElseKeyword);
            var body = ParseStatementOrScope();

            return new ElseStatement(CreatePart(keyword.Start), body);
        }
        private WhileStatement ParseWhileStatement()
        {
            var keyword = Take(TokenType.WhileKeyword);
            var predicate = ParsePredicate();
            var body = ParseStatementOrScope();

            return new WhileStatement(CreatePart(keyword.Start), predicate, body);
        }
        private WhileStatement ParseDoWhileStatement()
        {
            var keyword = Take(TokenType.DoKeyword);
            var body = ParseStatementOrScope();
            var @while = Take(TokenType.WhileKeyword);
            var predicate = ParsePredicate();

            return new WhileStatement(CreatePart(keyword.Start), predicate, body, WhileStatementType.DoWhile);
        }
        private ForStatement ParseForStatement()
        {
            var keyword = Take(TokenType.ForKeyword);
            SyntaxNode init = null;
            Expression condition = null;
            Expression increment = null;

            MakeBlock(() =>
            {
                if (Current == TokenType.LetKeyword)
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

            return new ForStatement(CreatePart(keyword.Start), init, condition, increment, body);
        }
        private ReturnStatement ParseReturnStatement()
        {
            var keyword = Take(TokenType.ReturnKeyword);
            var value = ParseExpression();

            return new ReturnStatement(CreatePart(keyword.Start), value);
        }
        private SwitchStatement ParseSwitchStatement()
        {
            var cases = new List<CaseStatement>();
            var keyword = Take(TokenType.SwitchKeyword);
            Expression condition = null;

            MakeBlock(() => condition = ParseExpression(), TokenType.LeftParenthesis, TokenType.RightParenthesis);
            MakeBlock(() =>
            {
                while (Current == TokenType.CaseKeyword || Current == TokenType.DefaultKeyword)
                    cases.Add(ParseCaseStatement());
            });

            return new SwitchStatement(CreatePart(keyword.Start), condition, cases);
        }
        private CaseStatement ParseCaseStatement()
        {
            var conditions = new List<Expression>();
            BlockStatement body = null;
            var start = Current;

            while (Current == TokenType.CaseKeyword || Current == TokenType.DefaultKeyword)
            {
                if (Current == TokenType.DefaultKeyword)
                {
                    var keyword = Take(TokenType.DefaultKeyword);
                    TakeSemicolon();

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
            var keyword = Take(TokenType.ModuleKeyword);

            var moduleNameParts = new List<IdentifierExpression>();
            var classes = new List<ClassDeclaration>();
            var interfaces = new List<InterfaceDeclaration>();
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
                var attributes = ParseAttributes();
                var modifier = ParseModifier();

                var member = ParseClassInterfaceMethodOrEnumDeclaration(attributes, modifier);

                switch(member)
                {
                    case ClassDeclaration @class:
                        classes.Add(@class);
                        break;
                    case InterfaceDeclaration @interface:
                        interfaces.Add(@interface);
                        break;
                    case MethodDeclaration method:
                        methods.Add(method);
                        break;
                    case EnumDeclaration @enum:
                        enums.Add(@enum);
                        break;

                    default:
                        throw SyntaxError($"Unexpected '{member.Kind}', expected class, interface, method or enum", member.FilePart, Severity.Error);
                }
            });

            return new ModuleDeclaration(CreatePart(keyword.Start), string.Join(".", moduleNameParts.Select(identifier => identifier.Name)), classes, interfaces, methods, enums);
        }
        private Declaration ParseClassInterfaceMethodOrEnumDeclaration(IEnumerable<AttributeSyntax> attributes, SyntaxModifier modifier)
        {
            _genericTypes.Push(new List<TypeExpression>());

            try
            {
                switch (Current.TokenType)
                {
                    case TokenType.ClassKeyword:
                        return ParseClassDeclaration(attributes, modifier);
                    case TokenType.InterfaceKeyword:
                        return ParseInterfaceDeclaration(attributes, modifier);
                    case TokenType.EnumKeyword:
                        return ParseEnumDeclaration(attributes, modifier);
                    case TokenType.Identifier:
                    case TokenType.BoolKeyword:
                    case TokenType.ByteKeyword:
                    case TokenType.SByteKeyword:
                    case TokenType.IntKeyword:
                    case TokenType.UIntKeyword:
                    case TokenType.ShortKeyword:
                    case TokenType.UShortKeyword:
                    case TokenType.LongKeyword:
                    case TokenType.ULongKeyword:
                    case TokenType.FloatKeyword:
                    case TokenType.DoubleKeyword:
                    case TokenType.DecimalKeyword:
                    case TokenType.StringKeyword:
                    //case TokenType.CharKeyword:
                    //case TokenType.ObjectKeyword:
                    case TokenType.VoidKeyword:
                        {
                            // Method declaration
                            var returnType = ParseTypeExpression();

                            if (Current != TokenType.Identifier)
                            {
                                throw SyntaxError($"Unexpected '{Current.Value}', expecting identifier", CreatePart(Last.End), Severity.Error);
                            }

                            var name = ParseIdentifierName();

                            return ParseMethodDeclaration(name, returnType, attributes, modifier);
                        }
                    default:
                        {
                            throw SyntaxError($"Unexpected '{Current.Value}', expecting 'class', 'interface', 'enum' or type", CreatePart(Current.Start), Severity.Error);
                        }
                }
            }
            finally
            {
                _genericTypes.Pop();
            }            
        }
        private ClassDeclaration ParseClassDeclaration(IEnumerable<AttributeSyntax> attributes, SyntaxModifier modifier)
        {
            var inhertitors = new List<IdentifierExpression>();
            var constructors = new List<ConstructorDeclaration>();
            var fields = new List<FieldDeclaration>();
            var methods = new List<MethodDeclaration>();
            var properties = new List<PropertyDeclaration>();
            var genericParameters = new List<TypeExpression>();

            //_genericTypes.Push(genericParameters);

            var keyword = Take(TokenType.ClassKeyword);
            var name = ParseIdentifierName();

            // TODO(Dan): Need to parse any generic class parameters here...
            if (Current == TokenType.LessThan)
            {
                var parameters = ParseGenericTypeParameters();

                // NOTE(Dan): This forcibly removes the case where a generic class is defined unsuitably
                //            for example: class MyType<T> is OK but class MyType<T<U>> is not!
                if (parameters.Any(param => param.GenericParameters.Any()))
                {
                    var error = parameters.First(param => param.GenericParameters.Any()).GenericParameters.First();

                    AddError("Generic parameters for an interface declaration cannot contain nested type parameters.", error.FilePart, Severity.Error);
                }

                genericParameters.AddRange(parameters.Select(type => new GenericConstraintTypeExpression(type.TypeKind, type.Identifier, type.GenericParameters, type.FilePart)));
            }

            if (Current == TokenType.Colon)
                inhertitors.AddRange(ParseBaseInheritors());

            MakeBlock(() =>
            {
                var memberAttributes = ParseAttributes();
                var memberModifier = ParseModifier();
                var member = ParseClassMember(memberAttributes, memberModifier);

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

            return new ClassDeclaration(CreatePart(keyword.Start), name, modifier, fields, properties, methods, constructors, genericParameters, inhertitors, attributes);
        }
        private SyntaxNode ParseClassMember(IEnumerable<AttributeSyntax> attributes, SyntaxModifier modifier)
        {
            if (Current == TokenType.ConstructorKeyword)
                return ParseConstructorDeclaration(attributes, modifier);

            var returnType = ParseTypeExpression();
            var name = ParseIdentifierName();

            switch (Current.TokenType)
            {
                case TokenType.LeftBracket:
                case TokenType.FatArrow:
                    return ParseClassPropertyDeclaration(name, returnType, attributes, modifier);

                case TokenType.LeftParenthesis:
                case TokenType.LessThan:
                    return ParseMethodDeclaration(name, returnType, attributes, modifier);

                case TokenType.Semicolon:
                case TokenType.Assignment:
                    return ParseFieldDeclaration(name, returnType, attributes, modifier);

                default:
                    throw UnexpectedToken("Field, Property or Method Declaration.");
            }
        }
        private InterfaceDeclaration ParseInterfaceDeclaration(IEnumerable<AttributeSyntax> attributes, SyntaxModifier modifier)
        {
            var inhertitors = new List<IdentifierExpression>();
            var properties = new List<PropertyDeclaration>();
            var methods = new List<MethodDeclaration>();
            var genericParameters = new List<TypeExpression>();

            var keyword = Take(TokenType.InterfaceKeyword);
            var name = ParseIdentifierName();

            // TODO(Dan): Need to parse any generic class parameters here...
            if (Current == TokenType.LessThan)
            {
                var parameters = ParseGenericTypeParameters();

                // NOTE(Dan): This forcibly removes the case where a generic class is defined unsuitably
                //            for example: class MyType<T> is OK but class MyType<T<U>> is not!
                if (parameters.Any(param => param.GenericParameters.Any()))
                {
                    var error = parameters.First(param => param.GenericParameters.Any()).GenericParameters.First();

                    AddError("Generic parameters for an interface declaration cannot contain nested type parameters.", error.FilePart, Severity.Error);
                }
                
                genericParameters.AddRange(parameters.Select(type => new GenericConstraintTypeExpression(type.TypeKind, type.Identifier, type.GenericParameters, type.FilePart)));
            }

            if (Current == TokenType.Colon)
                inhertitors.AddRange(ParseBaseInheritors());

            MakeBlock(() =>
            {
                var memberAttributes = ParseAttributes();
                var member = ParseInterfaceMember(memberAttributes);

                switch (member)
                {
                    case PropertyDeclaration property:
                        properties.Add(property);
                        break;

                    case MethodDeclaration method:
                        methods.Add(method);
                        break;
                }
            });

            return new InterfaceDeclaration(CreatePart(keyword.Start), modifier, name, properties, methods, attributes);
        }
        private Declaration ParseInterfaceMember(IEnumerable<AttributeSyntax> attributes)
        {
            var returnType = ParseTypeExpression();
            var name = ParseIdentifierName();

            switch (Current.Value)
            {
                case "{":
                case "=>":
                    return ParseInterfacePropertyDeclaration(name, returnType, attributes);

                case "(":
                //case "<":
                    return ParseInterfaceMethodDeclaration(name, returnType, attributes);

                default:
                    throw UnexpectedToken("Property or Method Declaration");
            }
        }
        private ConstructorDeclaration ParseConstructorDeclaration(IEnumerable<AttributeSyntax> attributes, SyntaxModifier modifier)
        {
            var keyword = Take(TokenType.ConstructorKeyword);
            var parameters = ParseParameters();
            var body = ParseScope();

            return new ConstructorDeclaration(CreatePart(keyword.Start), modifier, keyword.Value, parameters, body, attributes);
        }
        private MethodDeclaration ParseMethodDeclaration(string name, TypeExpression returnType, IEnumerable<AttributeSyntax> attributes, SyntaxModifier modifier)
        {
            var token = Current;
            var genericParameters = new List<TypeExpression>();

            if (Current == TokenType.LessThan)
            {
                genericParameters.AddRange(ParseGenericTypeParameters());

                if (genericParameters.Any(p => p.Name == returnType.Name))
                    returnType = new InferredTypeExpression(new IdentifierExpression(returnType.FilePart, returnType.Name), returnType.FilePart);
            }

            // NOTE(Dan): Account for the case where in a declaration we can't use a predefined type / keyword type, e.g. 'int'
            //            This is OK in a MethodCall because the int will be part of the type being inferred, but here we 100% do not want
            //            to use this as an inferred type as it will impact the usage of the keyword type.
            foreach (var genericParameter in genericParameters.Where(p => p is PredefinedTypeExpression))
                AddError($"Unexpected '{genericParameter.Name}', expecting {TokenType.Identifier.Value()}", genericParameter.FilePart, Severity.Error);

            //_genericTypes.Push(genericParameters);

            var parameters = ParseParameters();

            if (Current == TokenType.FatArrow)
            {
                var method = ParseExpressionBodiedMember(name, returnType, parameters, attributes, modifier);

                TakeSemicolon();

                return method;
            }

            var end = Last;

            var body = ParseScope();

            return new MethodDeclaration(CreatePart(token.Start, end.End), modifier, name, returnType, genericParameters, parameters, body, attributes);
        }
        private MethodDeclaration ParseInterfaceMethodDeclaration(string name, TypeExpression returnType, IEnumerable<AttributeSyntax> attributes)
        {
            var token = Current;
            var parameters = ParseParameters();

            // TODO(Dan): Generic constraints

            TakeSemicolon();

            return new MethodDeclaration(CreatePart(token.Start), SyntaxModifier.Public, name, returnType, new List<TypeExpression>(), parameters, null, attributes);
        }
        private FieldDeclaration ParseFieldDeclaration(string name, TypeExpression returnType, IEnumerable<AttributeSyntax> attributes, SyntaxModifier modifier)
        {
            var token = Current;

            Expression defaultValue = null;

            if (Current == TokenType.Assignment)
            {
                Take();
                defaultValue = ParseExpression();
            }

            TakeSemicolon();

            return new FieldDeclaration(CreatePart(token.Start), modifier, name, returnType, defaultValue, attributes);
        }
        private PropertyDeclaration ParseClassPropertyDeclaration(string name, TypeExpression returnType, IEnumerable<AttributeSyntax> attributes, SyntaxModifier modifier)
        {
            var token = Current;

            MethodDeclaration getMethod = null;
            MethodDeclaration setMethod = null;
            var flags = SyntaxFlags.None;

            if (Current == TokenType.FatArrow)
            {
                getMethod = ParseExpressionBodiedMember($"get_{name}", returnType, Enumerable.Empty<ParameterDeclaration>(), attributes, modifier);
                TakeSemicolon();
            }
            else
            {

                MakeBlock(() =>
                {
                    switch (Current.Value)
                    {
                        // TODO(Dan): Replace with get/set keywords
                        case "get":
                            {
                                var get = Take();

                                switch (Current.TokenType)
                                {
                                    case TokenType.Semicolon:
                                        {
                                            //var part = CreatePart(get.Start);
                                            //// TODO(Dan): Need to add this compiler generated field to the AST
                                            //var backingField = new FieldDeclaration(part, SyntaxModifier.Private, $"_{name.FirstToLower()}", returnType, null, Enumerable.Empty<AttributeSyntax>());
                                            //var body = new BlockStatement(part, new[] { new ReturnStatement(CreatePart(get.Start), new IdentifierExpression(part, $"{backingField.Name}")) });
                                            //getMethod = new MethodDeclaration(part, modifier, $"get_{name}", returnType, Enumerable.Empty<ParameterDeclaration>(), body);
                                            TakeSemicolon();
                                            flags |= SyntaxFlags.AutoImplementedPropertyGetter;
                                            break;
                                        }
                                        
                                    case TokenType.FatArrow:
                                        { 
                                            getMethod = ParseExpressionBodiedMember($"get_{name}", returnType, Enumerable.Empty<ParameterDeclaration>(), attributes, modifier);
                                            TakeSemicolon();
                                            break;
                                        }

                                    default:
                                        {
                                            var body = ParseScope();

                                            if (getMethod != null)
                                                AddError($"Multiple getters for property: {name}", CreatePart(get.Start), Severity.Error);
                                            else
                                                getMethod = new MethodDeclaration(CreatePart(get.Start), modifier, $"get_{name}", returnType, new List<TypeExpression>(), Enumerable.Empty<ParameterDeclaration>(), body, attributes);

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
                                            //var backingField = new FieldDeclaration(part, SyntaxModifier.Private, $"_{name.FirstToLower()}", returnType, null, Enumerable.Empty<AttributeSyntax>());
                                            //var body = new BlockStatement(part, new[] { new BinaryExpression(part, new IdentifierExpression(part, backingField.Name), new IdentifierExpression(part, "value"), BinaryOperator.Assign) });
                                            //var methodDeclaration = new MethodDeclaration(CreatePart(set.Start), modifier, $"set_{name}", new PredefinedTypeExpression(SyntaxKind.VoidKeyword, TypeExpressionKind.NonOptional, new IdentifierExpression(part, "void"), Enumerable.Empty<TypeExpression>(), new SourceFilePart()), new[] { new ParameterDeclaration(part, "value", returnType) }, body);
                                            TakeSemicolon();
                                            flags |= SyntaxFlags.AutoImplementedPropertySetter;
                                            break;
                                        }

                                    // TODO(Dan): Allow { get => _value; set => _value = value; }
                                    case TokenType.FatArrow:
                                        {
                                            setMethod = ParseExpressionBodiedMember($"set_{name}", returnType, Enumerable.Empty<ParameterDeclaration>(), attributes, modifier);
                                            TakeSemicolon();
                                            break;
                                        }

                                    default:
                                        {
                                            var body = ParseScope();

                                            if (getMethod != null)
                                                AddError($"Multiple getters for property: {name}", CreatePart(set.Start), Severity.Error);
                                            else
                                            {
                                                var type = new PredefinedTypeExpression(SyntaxKind.VoidKeyword, TypeExpressionKind.NonOptional, new IdentifierExpression(new SourceFilePart(null, null, null, null), "void"), Enumerable.Empty<TypeExpression>(), new SourceFilePart(null, null, null, null));
                                                setMethod = new MethodDeclaration(CreatePart(set.Start), modifier, $"set_{name}", type, new List<TypeExpression>(), new[] { new ParameterDeclaration(CreatePart(set.Start), "value", returnType) }, body, attributes);
                                            }

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

            return new PropertyDeclaration(flags, modifier, name, returnType, getMethod, setMethod, attributes, CreatePart(token.Start, Last.End));
        }
        private PropertyDeclaration ParseInterfacePropertyDeclaration(string name, TypeExpression returnType, IEnumerable<AttributeSyntax> attributes)
        {
            var token = Current;

            MethodDeclaration getMethod = null;
            MethodDeclaration setMethod = null;

            MakeBlock(() =>
            {
                switch (Current.Value)
                {
                    case "get":
                        {
                            var get = Take();
                            TakeSemicolon();
                            break;
                        }
                    case "set":
                        {
                            var set = Take();
                            TakeSemicolon();
                            break;
                        }
                    default:
                        throw UnexpectedToken("get or set");
                }
            });
            

            if (Current == TokenType.Semicolon)
            {
                var semicolon = TakeSemicolon();
                AddError("Possibly mistaken empty statement", CreatePart(semicolon.Start, semicolon.End), Severity.Warning);
            }

            return new PropertyDeclaration(SyntaxFlags.None, SyntaxModifier.Public, name, returnType, getMethod, setMethod, attributes, CreatePart(token.Start));
        }
        private TypeExpression ParseTypeExpression()
        {
            var topLevelGenericParameters = _genericTypes.Peek();
            var token = Current;
            var isPredefinedType = SyntaxFacts.IsPredefinedType(token);
            var name = isPredefinedType
                ? new IdentifierExpression(CreatePart(token.Start), Take().Value)
                : (IdentifierExpression)ParseIdentifierExpression();

            if (topLevelGenericParameters.Any(p => p.Name == name.Name))
                return new InferredTypeExpression(name, CreatePart(token.Start));

            var genericParameters = new List<TypeExpression>();

            // TODO(Dan): Allow things like: int[] and generics etc
            if (Current == TokenType.LessThan)
                genericParameters.AddRange(ParseGenericTypeParameters());

            var optionalType = TypeExpressionKind.NonOptional;

            if (Current == TokenType.Question)
            {
                Take(TokenType.Question);
                optionalType = TypeExpressionKind.Optional;
            }

            if (isPredefinedType)
                return new PredefinedTypeExpression(SyntaxFacts.PredefinedTypeExpressionKind(token), optionalType, name, genericParameters, CreatePart(token.Start));

            return new UserDefinedTypeExpression(optionalType, name, genericParameters, CreatePart(token.Start));
        }
        private IEnumerable<TypeExpression> ParseGenericTypeParameters()
        {
            var topLevelGenericParameters = _genericTypes.Peek();

            var genericTypes = new List<TypeExpression>();

            MakeBlock(() =>
            {
                do
                {
                    // NOTE(Dan): HACK... Improve how we do this...
                    if (genericTypes.Count > 0 && Current == TokenType.Comma)
                        Take(TokenType.Comma);

                    var type = ParseTypeExpression();

                    genericTypes.Add(type);

                } while (Current == TokenType.Comma);

                // TODO(Dan): Can we avoid having to do this? We don't really have enough context when tokenizing the file...
                if (Current == TokenType.BitShiftRight)
                {
                    var bitshift = _tokenStream.Dequeue();
                    _tokenStream.Enqueue(new Token(TokenType.GreaterThan, ">", bitshift.Start, new SourceFileLocation(bitshift.Start.Column + 1, bitshift.Start.Index + 1, bitshift.Start.LineNumber)));
                    _tokenStream.Enqueue(new Token(TokenType.GreaterThan, ">", new SourceFileLocation(bitshift.Start.Column + 1, bitshift.Start.Index + 1, bitshift.Start.LineNumber), bitshift.End));
                }
            },
            TokenType.LessThan,
            TokenType.GreaterThan);

            topLevelGenericParameters.AddRange(genericTypes);

            return genericTypes;
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
            var name = Take(TokenType.Identifier);
            var colon = Take(TokenType.Colon);
            var type = ParseTypeExpression();

            return new ParameterDeclaration(CreatePart(name.Start), name.Value, type);
        }
        private VariableDeclaration ParseVariableDeclaration()
        {
            Token start = null;

            if (Current == TokenType.ConstKeyword)
                start = Take(TokenType.ConstKeyword);
            else
                start = Take(TokenType.LetKeyword);

            var isMutable = start == TokenType.LetKeyword;
            
            var name = (IdentifierExpression)ParseIdentifierExpression();
            var type = new InferredTypeExpression(name, CreatePart(start.Start));
            Expression value = null;

            if (Current == TokenType.Assignment)
            {
                Take();

                value = ParseExpression();
            }

            var mutabilityType = isMutable
                ? VariableMutabilityType.Mutable
                : VariableMutabilityType.Immutable;

            return new VariableDeclaration(CreatePart(start.Start), name.Name, type, value, mutabilityType);
        }
        private MethodDeclaration ParseExpressionBodiedMember(string methodName, TypeExpression returnType, IEnumerable<ParameterDeclaration> parameters, IEnumerable<AttributeSyntax> attributes, SyntaxModifier modifier)
        {
            Take(TokenType.FatArrow);

            var expression = ParseExpression();
            var span = CreatePart(expression.FilePart.Start, expression.FilePart.End);

            ReturnStatement returnStatement = null;

            if (returnType.Name != "void")
                returnStatement = new ReturnStatement(span, expression);

            return new MethodDeclaration(span, modifier, methodName, returnType, new List<TypeExpression>(), parameters, new BlockStatement(span, new[] { (SyntaxNode)returnStatement ?? expression }), attributes);
        }
        private EnumDeclaration ParseEnumDeclaration(IEnumerable<AttributeSyntax> attributes, SyntaxModifier modifier)
        {
            var start = Take(TokenType.EnumKeyword);
            var name = ParseIdentifierName();
            var members = new List<EnumMemberDeclaration>();

            MakeBlock(() =>
            {
                members.Add(ParseEnumMemberDeclaration());

                if (Current == TokenType.Comma && Next != TokenType.RightBracket)
                    Take(TokenType.Comma);
            });

            return new EnumDeclaration(CreatePart(start.Start), modifier, name, members, attributes);
        }
        private EnumMemberDeclaration ParseEnumMemberDeclaration()
        {
            var attributes = new List<AttributeSyntax>();

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

            Rewind();

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

            Rewind();

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

            Rewind();
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
                // HACK(Dan): Unfortunately, calling a generic method can be ambigous.
                //            For example: const x = y < z
                //                         vs.
                //                         const x = y < z(10)
                //                         vs.
                //                         const x = y<int>(10)
                //                         etc.
                if (Current == TokenType.LessThan && TryParseExpressionOrReset(() => ParseMethodCallExpression(left), out var expression))
                    return expression;

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
            if (Current == TokenType.Identifier || SyntaxFacts.IsPredefinedType(Current))
            {
                if (Next == TokenType.Dot)
                {
                    return ParseReferenceExpression();
                }
                // TODO(Dan): Need to account for Generics here... e.g. const x = GenericFunction<int>(10);
                //            Currently we get a generic "Expecting 'Expression term'..." messsage.
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
            else if (Current.Category == TokenCategory.Constant || Current == TokenType.TrueKeyword || Current == TokenType.FalseKeyword)
            {
                return ParseConstantExpression();
            }
            else if (Current == TokenType.LeftParenthesis)
            {
                return ParseOverrideExpression();
            }
            else if (Current == TokenType.NewKeyword)
            {
                return ParseNewExpression();
            }
            else if (Current == TokenType.Semicolon && Last == TokenType.ReturnKeyword)
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
            var references = new List<Expression>
            {
                hint
            };

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
            var genericParameters = new List<TypeExpression>();

            // TODO(Dan): I need to potentially have top level parameters here so need to refactor this
            //            so that we keep track of the current set of generic parameters from parents
            if (Current == TokenType.LessThan)
                genericParameters.AddRange(ParseGenericTypeParameters());

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

            var expression = new MethodCallExpression(CreatePart(hint.FilePart.Start), hint, args, genericParameters);

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

            if (Current == TokenType.TrueKeyword || Current == TokenType.FalseKeyword)
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
                throw UnexpectedToken("Constant expression");
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
            var start = Take(TokenType.NewKeyword);
            var references = new List<SyntaxNode>();
            var arguments = new List<Expression>();
            var genericParameters = new List<TypeExpression>();

            references.Add(ParseTypeExpression());

            if (Current == TokenType.LessThan)
                genericParameters.AddRange(ParseGenericTypeParameters());

            while (Current == TokenType.Dot)
            {
                Take(TokenType.Dot);
                references.Add(ParseIdentifierExpression());
            }

            SyntaxNode reference = null;

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
        private Expression ParseBinaryExpression(Expression left)
        {
            var op = ParseBinaryOperator();
            var right = ParseBitwiseExpression();

            return new BinaryExpression(CreatePart(left.FilePart.Start), left, right, op);
        }

        // Errors
        private void AddError(string message, SourceFilePart part, Severity severity)
        {
            _errorSink.AddError($"{message}", part, severity);
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

            var message = $"Unexpected '{value}'. Expected '{expected}'";

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
            return _tokenStream.Peek(ahead);
        }
        private void Advance()
        {
            _tokenStream.Advance();
        }
        private void Rewind()
        {
            _tokenStream.Rewind();
        }
        private bool IsMakingProgress(int lastTokenPosition)
        {
            return _tokenStream.IsMakingProgress(lastTokenPosition);
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
        //private Token Take(string contextualKeyword)
        //{
        //    if (Current != TokenType.Identifier && Current != contextualKeyword)
        //        throw UnexpectedToken(contextualKeyword);
        //
        //    return Take();
        //}
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
            var take = (end.LineNumber - start.LineNumber) < 1 ? 1 : (end.LineNumber - skip);
            var content = _currentSourceFile.Lines.Skip(start.LineNumber - 1).Take(take).ToArray();
            return new SourceFilePart(_currentSourceFile.Name, start, end, content);
        }
        private string ParseIdentifierName()
        {
            if (Current == TokenType.Identifier)
                return Take(TokenType.Identifier).Value;

            throw UnexpectedToken("Identifier");
        }
        //private string ParseTypeName()
        //{
        //    switch (Current.TokenType)
        //    {
        //        case TokenType.InterfaceKeyword:
        //            return Take(TokenType.InterfaceKeyword).Value;
        //        case TokenType.BoolKeyword:
        //            return Take(TokenType.BoolKeyword).Value;
        //        case TokenType.ByteKeyword:
        //            return Take(TokenType.ByteKeyword).Value;
        //        case TokenType.SByteKeyword:
        //            return Take(TokenType.SByteKeyword).Value;
        //        case TokenType.IntKeyword:
        //            return Take(TokenType.IntKeyword).Value;
        //        case TokenType.UIntKeyword:
        //            return Take(TokenType.UIntKeyword).Value;
        //        case TokenType.ShortKeyword:
        //            return Take(TokenType.ShortKeyword).Value;
        //        case TokenType.UShortKeyword:
        //            return Take(TokenType.UShortKeyword).Value;
        //        case TokenType.LongKeyword:
        //            return Take(TokenType.LongKeyword).Value;
        //        case TokenType.ULongKeyword:
        //            return Take(TokenType.ULongKeyword).Value;
        //        case TokenType.FloatKeyword:
        //            return Take(TokenType.FloatKeyword).Value;
        //        case TokenType.DoubleKeyword:
        //            return Take(TokenType.DoubleKeyword).Value;
        //        case TokenType.DecimalKeyword:
        //            return Take(TokenType.DecimalKeyword).Value;
        //        case TokenType.StringKeyword:
        //            return Take(TokenType.StringKeyword).Value;
        //        //case TokenType.//CharKeyword:
        //        //case TokenType.//ObjectKeyword:
        //        case TokenType.VoidKeyword:
        //            return Take(TokenType.VoidKeyword).Value;
        //
        //        default:
        //            return ParseIdentifierName();
        //    }
        //}
        private void MakeBlock(Action action, TokenType open = TokenType.LeftBracket, TokenType close = TokenType.RightBracket)
        {
            Take(open);

            MakeStatement(action, open, close);
        }
        private void MakeStatement(Action action, TokenType? open = null, TokenType close = TokenType.Semicolon)
        {
            try
            {
                var startIndex = _tokenStream.Position;

                while (Current != close && Current != TokenType.EOF)
                {
                    action();

                    if (!IsMakingProgress(startIndex))
                        throw SyntaxError($"Unexpected '{Current.Value}'", CreatePart(Current.Start), Severity.Error);

                    startIndex = _tokenStream.Position;
                }
            }
            catch (SyntaxException)
            {
                // TODO(Dan): Fix this in a better way...
                var take = 1;

                if (open != null)
                {
                    while (take > 0 && Current != TokenType.EOF)
                    {
                        var token = Take();

                        if (token == open.Value)
                            take++;

                        if (token == close)
                            take--;
                    }
                }
                else
                {
                    while (Current != close && Current != TokenType.EOF)
                        Take();
                }
            }
            finally
            {
                // TODO(Dan): Improve error recovery - this will be awful!
                if (_error)
                {
                    if (Last == close)
                        _tokenStream.Rewind();

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
        private bool TryParseExpressionOrReset(Func<Expression> action, out Expression result)
        {
            // HACK(Dan): We need to do this properly, and not just catch the exception, this is awful for perf
            //            which is bad enough as it is!
            result = default;
            _errorSink.IsWritable = false;
            var index = _tokenStream.Position;
            var stopwatch = Stopwatch.StartNew();

            try
            {
               
                result = action();
                var end = stopwatch.ElapsedMilliseconds;
                return true;
            } 
            catch (SyntaxException)
            {
                var end = stopwatch.ElapsedMilliseconds;

                _tokenStream.Rewind(_tokenStream.Position - index);

                stopwatch.Stop();

                var afterRewind = stopwatch.ElapsedMilliseconds;
                return false;
            }
            finally
            {
                _errorSink.IsWritable = true;
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
            _genericTypes = new Stack<List<TypeExpression>>(new[] { new List<TypeExpression>() });
        }
    }
}
