using Ast;
using Ast.Declarations;
using Ast.Expressions;
using Lexer;
using Runtime;

using ValueType = Runtime.ValueType;

namespace Parser;

public class Parser
{
    private readonly TokenStream _tokens;
    private ValueType? _currentVariableType;

    public Parser(string code)
    {
        _tokens = new TokenStream(code);
    }

    /// <summary>
    /// программа = {глобальная_инструкция}, точка_входа;
    /// </summary>
    public List<AstNode> ParseProgram()
    {
        List<AstNode> nodes = new();
        while (_tokens.Peek().Type != TokenType.EndOfFile &&
               _tokens.Peek().Type != TokenType.Maincraft)
        {
            AstNode nodeGlobalDeclaration = ParseGlobalDeclaration();
            nodes.Add(nodeGlobalDeclaration);
        }

        List<AstNode> maincraftNodes = ParseMaincraft();
        nodes.AddRange(maincraftNodes);

        return nodes;
    }

    /// <summary>
    /// Выполняет разбор выражения и возвращает результат.
    /// </summary>
    public Expression EvaluateExpression()
    {
        return ParseExpression();
    }

    /// <summary>
    /// глобальная_инструкция = объявление_переменной | объявление_константы | объявление_функции;
    /// </summary>
    private AstNode ParseGlobalDeclaration()
    {
        Token token = _tokens.Peek();

        if (token.Type == TokenType.Monument)
        {
            return ParseConstantDeclaration();
        }

        if (IsTypeToken(token.Type) || token.Type == TokenType.Nullable)
        {
            return ParseVariableDeclaration();
        }

        if (token.Type == TokenType.Funkotron)
        {
            return ParseFunctionDeclaration();
        }

        throw new UnexpectedLexemeException("global declaration", token);
    }

    /// <summary>
    /// объявление_функции = "funkotron", идентификатор, "(", [параметры], ")", блок_функции;
    /// </summary>
    private AstNode ParseFunctionDeclaration()
    {
        Match(TokenType.Funkotron);

        string functionName = ParseIdentifier();

        List<ParameterDeclaration> parameters = ParseParameterList();

        string? returnTypeName = null;

        if (_tokens.Peek().Type == TokenType.Colon)
        {
            _tokens.Advance();

            returnTypeName = ParseTypeName();
        }

        List<AstNode> nodes = ParseFunctionBlock();

        return new FunctionDeclaration(functionName, parameters, returnTypeName, nodes);
    }

    private string ParseTypeName()
    {
        Token token = _tokens.Peek();
        if (IsTypeToken(token.Type))
        {
            _tokens.Advance();
            return token.Type switch
            {
                TokenType.Dayzint => "dayzint",
                TokenType.Fallout => "fallout",
                TokenType.Strike => "strike",
                _ => throw new UnexpectedLexemeException("type", token),
            };
        }

        throw new UnexpectedLexemeException("type", token);
    }

    private List<ParameterDeclaration> ParseParameterList()
    {
        Match(TokenType.OpenParenthesis);
        List<ParameterDeclaration> parameters = [];

        if (_tokens.Peek().Type != TokenType.CloseParenthesis)
        {
            string typeName = ParseTypeName();
            string name = ParseIdentifier();
            parameters.Add(new ParameterDeclaration(name, typeName));

            while (_tokens.Peek().Type == TokenType.Comma)
            {
                _tokens.Advance();
                typeName = ParseTypeName();
                name = ParseIdentifier();
                parameters.Add(new ParameterDeclaration(name, typeName));
            }
        }

        Match(TokenType.CloseParenthesis);
        return parameters;
    }

    /// <summary>
    /// объявление_переменной = тип, идентификатор, ["=", выражение], ";";
    /// </summary>
    private AstNode ParseVariableDeclaration()
    {
        if (_tokens.Peek().Type == TokenType.Nullable)
        {
            _tokens.Advance();
        }

        string typeName = ParseTypeName();
        _currentVariableType = ConvertTypeNameToValueType(typeName);

        string name = ParseIdentifier();
        Expression? value = null;

        if (_tokens.Peek().Type == TokenType.Assign)
        {
            _tokens.Advance();
            value = ParseExpression();
        }

        Match(TokenType.Semicolon);

        _currentVariableType = null;

        return new VariableDeclaration(name, typeName, value);
    }

    private ValueType ConvertTypeNameToValueType(string typeName)
    {
        return typeName.ToLower() switch
        {
            "dayzint" => ValueType.Int,
            "fallout" => ValueType.Double,
            "strike" => ValueType.String,
            _ => throw new Exception($"Unknown type: {typeName}"),
        };
    }

    /// <summary>
    /// объявление_константы = "monument", тип, идентификатор, ["=", выражение], ";";
    /// </summary>
    private AstNode ParseConstantDeclaration()
    {
        Match(TokenType.Monument);

        if (_tokens.Peek().Type == TokenType.Nullable)
        {
            _tokens.Advance();
        }

        string typeName = ParseTypeName();
        _currentVariableType = ConvertTypeNameToValueType(typeName);

        string name = ParseIdentifier();

        Match(TokenType.Assign);
        Expression value = ParseExpression();
        Match(TokenType.Semicolon);

        _currentVariableType = null;

        return new ConstantDeclaration(name, typeName, value);
    }

    /// <summary>
    /// точка_входа = "maincraft", "(", ")", блок_функции;
    /// </summary>
    private List<AstNode> ParseMaincraft()
    {
        Match(TokenType.Maincraft);
        Match(TokenType.OpenParenthesis);
        Match(TokenType.CloseParenthesis);
        return ParseBlock();
    }

    private List<AstNode> ParseFunctionBlock()
    {
        Match(TokenType.OpenBrace);
        List<AstNode> nodes = [];

        while (_tokens.Peek().Type != TokenType.CloseBrace &&
               _tokens.Peek().Type != TokenType.EndOfFile)
        {
            AstNode node = ParseStatement();
            nodes.Add(node);
        }

        Match(TokenType.CloseBrace);

        return nodes;
    }

    /// <summary>
    /// блок_функции = "{", {инструкция_функции}, "}";
    /// </summary>
    private List<AstNode> ParseBlock()
    {
        List<AstNode> nodes = new();
        Match(TokenType.OpenBrace);

        while (_tokens.Peek().Type != TokenType.CloseBrace &&
               _tokens.Peek().Type != TokenType.EndOfFile)
        {
            AstNode node = ParseStatement();
            nodes.Add(node);
        }

        Match(TokenType.CloseBrace);

        return nodes;
    }

    /// <summary>
    /// инструкция_функции = объявление_переменной | объявление_константы | присваивание | условие |
    /// цикл_while | цикл_for | вызов_функции | вызов_заложенной_функции | возврат;
    /// </summary>
    private AstNode ParseStatement()
    {
        Token token = _tokens.Peek();

        if (token.Type == TokenType.Monument)
        {
            return ParseConstantDeclaration();
        }

        if (IsTypeToken(token.Type) || token.Type == TokenType.Nullable)
        {
            return ParseVariableDeclaration();
        }

        if (token.Type == TokenType.Iffy)
        {
            return ParseIffyStatement();
        }

        if (token.Type == TokenType.Breakout)
        {
            return ParseBreakoutStatement();
        }

        if (token.Type == TokenType.Contra)
        {
            return ParseContraStatement();
        }

        if (token.Type == TokenType.Forza)
        {
            return ParseForzaStatement();
        }

        if (token.Type == TokenType.Identifier)
        {
            return ParseAssignmentOrFunctionCall();
        }

        if (token.Type == TokenType.Exodus || token.Type == TokenType.Exodusln)
        {
            return ParseOutputStatement();
        }

        if (token.Type == TokenType.Raid)
        {
            return ParseInputStatement();
        }

        if (token.Type == TokenType.Valorant)
        {
            return ParseValorantStatement();
        }

        if (token.Type == TokenType.Returnal)
        {
            return ParseReturnStatement();
        }

        throw new UnexpectedLexemeException("statement", token);
    }

    private AstNode ParseBreakoutStatement()
    {
        Match(TokenType.Breakout);
        Match(TokenType.Semicolon);
        return new BreakExpression();
    }

    private AstNode ParseContraStatement()
    {
        Match(TokenType.Contra);
        Match(TokenType.Semicolon);
        return new ContinueExpression();
    }

    private AstNode ParseReturnStatement()
    {
        Match(TokenType.Returnal);

        Expression returnValue = ParseExpression();

        Match(TokenType.Semicolon);

        return new ReturnExpression(returnValue);
    }

    private AstNode ParseForzaStatement()
    {
        Match(TokenType.Forza);
        Match(TokenType.OpenParenthesis);

        if (_tokens.Peek().Type == TokenType.Nullable)
        {
            _tokens.Advance();
        }

        string typeName = ParseTypeName();
        _currentVariableType = ConvertTypeNameToValueType(typeName);

        string iteratorName = ParseIdentifier();
        Match(TokenType.Assign);
        Expression startValue = ParseExpression();

        _currentVariableType = null;
        Match(TokenType.Semicolon);

        Expression endCondition = ParseExpression();
        Match(TokenType.Semicolon);

        string stepVarName = ParseIdentifier();
        Match(TokenType.Assign);
        Expression stepExpression = ParseExpression();
        Expression stepAssign = new AssignmentExpression(stepVarName, stepExpression);

        Match(TokenType.CloseParenthesis);

        Match(TokenType.OpenBrace);
        List<AstNode> body = [];

        while (_tokens.Peek().Type != TokenType.CloseBrace &&
               _tokens.Peek().Type != TokenType.EndOfFile)
        {
            AstNode node = ParseStatement();
            body.Add(node);
        }

        Match(TokenType.CloseBrace);

        return new ForLoopExpression(typeName, iteratorName, startValue, endCondition, stepAssign, body);
    }

    private AstNode ParseValorantStatement()
    {
        Match(TokenType.Valorant);
        Match(TokenType.OpenParenthesis);

        Expression condition = ParseExpression();
        Match(TokenType.CloseParenthesis);

        Match(TokenType.OpenBrace);
        List<AstNode> thenBranch = [];

        while (_tokens.Peek().Type != TokenType.CloseBrace &&
               _tokens.Peek().Type != TokenType.EndOfFile)
        {
            AstNode node = ParseStatement();
            thenBranch.Add(node);
        }

        Match(TokenType.CloseBrace);

        return new WhileLoopExpression(condition, thenBranch);
    }

    private AstNode ParseIffyStatement()
    {
        Match(TokenType.Iffy);
        Match(TokenType.OpenParenthesis);

        Expression condition = ParseExpression();
        Match(TokenType.CloseParenthesis);

        Match(TokenType.OpenBrace);
        List<AstNode> thenBranch = [];

        while (_tokens.Peek().Type != TokenType.CloseBrace &&
               _tokens.Peek().Type != TokenType.EndOfFile)
        {
            AstNode node = ParseStatement();
            thenBranch.Add(node);
        }

        Match(TokenType.CloseBrace);

        if (_tokens.Peek().Type == TokenType.Elysian)
        {
            Match(TokenType.Elysian);
            Match(TokenType.OpenBrace);
            List<AstNode> elseBranch = [];
            while (_tokens.Peek().Type != TokenType.CloseBrace &&
                   _tokens.Peek().Type != TokenType.EndOfFile)
            {
                AstNode node = ParseStatement();
                elseBranch.Add(node);
            }

            Match(TokenType.CloseBrace);

            return new IfElseExpression(condition, thenBranch, elseBranch);
        }

        return new IfExpression(condition, thenBranch);
    }

    private Expression ParseInputStatement()
    {
        Match(TokenType.Raid);
        Match(TokenType.OpenParenthesis);

        string variableName = ParseIdentifier();

        Match(TokenType.CloseParenthesis);
        Match(TokenType.Semicolon);

        return new RaidExpression(variableName);
    }

    private AstNode ParseAssignmentOrFunctionCall()
    {
        string name = ParseIdentifier();

        if (_tokens.Peek().Type == TokenType.Assign)
        {
            return ParseAssignment(name);
        }

        if (_tokens.Peek().Type == TokenType.OpenParenthesis)
        {
            _tokens.Advance();

            List<Expression> arguments = new();

            if (_tokens.Peek().Type != TokenType.CloseParenthesis)
            {
                arguments.Add(ParseExpression());

                while (_tokens.Peek().Type == TokenType.Comma)
                {
                    _tokens.Advance();
                    arguments.Add(ParseExpression());
                }
            }

            Match(TokenType.CloseParenthesis);
            Match(TokenType.Semicolon);

            return new FunctionCall(name, arguments);
        }

        throw new UnexpectedLexemeException("assignment or function call", _tokens.Peek());
    }

    private AstNode ParseAssignment(string variableName)
    {
        Match(TokenType.Assign);
        Expression value = ParseExpression();
        Match(TokenType.Semicolon);

        return new AssignmentExpression(variableName, value);
    }

    private AstNode ParseOutputStatement()
    {
        _tokens.Advance();
        Match(TokenType.OpenParenthesis);

        Expression value = ParseExpression();

        Match(TokenType.CloseParenthesis);
        Match(TokenType.Semicolon);

        return new ExodusExpression(value);
    }

    private string ParseIdentifier()
    {
        Token token = _tokens.Peek();
        if (token.Type == TokenType.Identifier)
        {
            _tokens.Advance();
            return token.Value?.ToString() ?? throw new InvalidOperationException("Identifier value cannot be null");
        }

        throw new UnexpectedLexemeException("identifier", token);
    }

    private bool IsTypeToken(TokenType type)
    {
        return type == TokenType.Dayzint || type == TokenType.Fallout || type == TokenType.Strike;
    }

    /// <summary>
    /// Разбирает выражение.
    /// Правила: выражение = логическое_выражение;
    /// </summary>
    private Expression ParseExpression()
    {
        return ParseLogicalExpression();
    }

    /// <summary>
    /// Разбирает логическое выражение.
    /// Правила: логическое_выражение = выражение_сравнения, {логический_оператор, выражение_сравнения};
    /// </summary>
    private Expression ParseLogicalExpression()
    {
        return ParseLogicalOrExpression();
    }

    /// <summary>
    /// Разбирает логическое ИЛИ выражение.
    /// Правила: логическое_ИЛИ_выражение = логическое_И_выражение, { "||", логическое_И_выражение };
    /// </summary>
    private Expression ParseLogicalOrExpression()
    {
        Expression left = ParseLogicalAndExpression();

        while (_tokens.Peek().Type == TokenType.LogicalOr)
        {
            _tokens.Advance();
            Expression right = ParseLogicalAndExpression();
            left = new BinaryOperationExpression(left, BinaryOperation.LogicalOr, right);
        }

        return left;
    }

    /// <summary>
    /// Разбирает логическое И выражение.
    /// Правила: логическое_И_выражение = выражение_сравнения, { "&&", выражение_сравнения };
    /// </summary>
    private Expression ParseLogicalAndExpression()
    {
        Expression left = ParseComparisonExpression();

        while (_tokens.Peek().Type == TokenType.LogicalAnd)
        {
            _tokens.Advance();
            Expression right = ParseComparisonExpression();
            left = new BinaryOperationExpression(left, BinaryOperation.LogicalAnd, right);
        }

        return left;
    }

    /// <summary>
    /// Разбирает выражение сравнения.
    /// Правила: выражение_сравнения = аддитивное_выражение, [оператор_сравнения, аддитивное_выражение];
    /// </summary>
    private Expression ParseComparisonExpression()
    {
        Expression left = ParseAdditiveExpression();

        if (IsComparisonOperator(_tokens.Peek().Type))
        {
            Token operatorToken = _tokens.Peek();
            _tokens.Advance();
            Expression right = ParseAdditiveExpression();

            BinaryOperation operation = operatorToken.Type switch
            {
                TokenType.LessThan => BinaryOperation.LessThan,
                TokenType.GreaterThan => BinaryOperation.GreaterThan,
                TokenType.LessThanOrEqual => BinaryOperation.LessThanOrEqual,
                TokenType.GreaterThanOrEqual => BinaryOperation.GreaterThanOrEqual,
                TokenType.Equal => BinaryOperation.Equal,
                TokenType.Unequal => BinaryOperation.NotEqual,
                _ => throw new Exception($"Unsupported comparison operator: {operatorToken.Type}"),
            };

            return new BinaryOperationExpression(left, operation, right);
        }

        return left;
    }

    /// <summary>
    /// Разбирает аддитивное выражение.
    /// Правила: аддитивное_выражение = мультипликативное_выражение, {адитивный_оператор, мультипликативное_выражение};
    /// </summary>
    private Expression ParseAdditiveExpression()
    {
        Expression left = ParseMultiplicativeExpression();

        while (_tokens.Peek().Type == TokenType.PlusSign || _tokens.Peek().Type == TokenType.MinusSign)
        {
            Token operatorToken = _tokens.Peek();
            _tokens.Advance();
            Expression right = ParseMultiplicativeExpression();

            BinaryOperation operation = operatorToken.Type == TokenType.PlusSign
                ? BinaryOperation.Plus
                : BinaryOperation.Minus;

            left = new BinaryOperationExpression(left, operation, right);
        }

        return left;
    }

    /// <summary>
    /// Разбирает мультипликативное выражение.
    /// Правила: мультипликативное_выражение = унарное_выражение, {мультипликативный_оператор, унарное_выражение};
    /// </summary>
    private Expression ParseMultiplicativeExpression()
    {
        Expression left = ParseUnaryExpression();

        while (IsMultiplicativeOperator(_tokens.Peek().Type))
        {
            Token operatorToken = _tokens.Peek();
            _tokens.Advance();
            Expression right = ParseUnaryExpression();

            BinaryOperation operation = operatorToken.Type switch
            {
                TokenType.MultiplySign => BinaryOperation.Multiply,
                TokenType.DivideSign => BinaryOperation.Divide,
                TokenType.ModuloSign => BinaryOperation.Modulo,
                _ => throw new Exception($"Unsupported multiplicative operator: {operatorToken.Type}"),
            };

            left = new BinaryOperationExpression(left, operation, right);
        }

        return left;
    }

    /// <summary>
    /// Разбирает унарное выражение.
    /// Правила: унарное_выражение = [унарный_оператор], часть_выражения;
    /// </summary>
    private Expression ParseUnaryExpression()
    {
        if (_tokens.Peek().Type == TokenType.MinusSign || _tokens.Peek().Type == TokenType.LogicalNot)
        {
            Token operatorToken = _tokens.Peek();
            _tokens.Advance();
            Expression operand = ParseExpressionPart();

            UnaryOperation operation = operatorToken.Type == TokenType.MinusSign
                ? UnaryOperation.Minus
                : UnaryOperation.LogicalNot;

            return new UnaryOperationExpression(operation, operand);
        }

        return ParseExpressionPart();
    }

    /// <summary>
    /// Разбирает часть выражения.
    /// Правила: часть_выражения = литерал_выражения | "(", выражение, ")";
    /// </summary>
    private Expression ParseExpressionPart()
    {
        Token token = _tokens.Peek();

        switch (token.Type)
        {
            case TokenType.NumericLiteral:
                _tokens.Advance();
                double doubleValue = token.Value!.ToDouble();
                Value numericValue;
                if (_currentVariableType.HasValue)
                {
                    numericValue = _currentVariableType.Value switch
                    {
                        ValueType.Int => new Value((int)doubleValue),
                        _ => new Value(doubleValue),
                    };
                }
                else
                {
                    string numericStr = token.Value.ToString();

                    if (numericStr.Contains('.') || numericStr.Contains(','))
                    {
                        numericValue = new Value(doubleValue);
                    }
                    else
                    {
                        if (Math.Abs(doubleValue % 1) < 0.0000001)
                        {
                            numericValue = new Value((int)doubleValue);
                        }
                        else
                        {
                            numericValue = new Value(doubleValue);
                        }
                    }
                }

                return new LiteralExpression(numericValue);

            case TokenType.Ready:
                _tokens.Advance();
                return new LiteralExpression(new Value(1));

            case TokenType.Noready:
                _tokens.Advance();
                return new LiteralExpression(new Value(0));

            case TokenType.Identifier:
                return ParseFunctionCallOrIdentifier();
            case TokenType.StringLiteral:
                _tokens.Advance();
                string strValue = token.Value?.ToString() ?? "";
                return new LiteralExpression(new Value(strValue));

            case TokenType.OpenParenthesis:
                _tokens.Advance();
                Expression result = ParseExpression();
                Match(TokenType.CloseParenthesis);
                return result;

            default:
                throw new UnexpectedLexemeException("expression part", token);
        }
    }

    private Expression ParseFunctionCallOrIdentifier()
    {
        string name = ParseIdentifier();

        if (_tokens.Peek().Type == TokenType.OpenParenthesis)
        {
            _tokens.Advance();

            List<Expression> arguments = new();

            if (_tokens.Peek().Type != TokenType.CloseParenthesis)
            {
                arguments.Add(ParseExpression());

                while (_tokens.Peek().Type == TokenType.Comma)
                {
                    _tokens.Advance(); // Consume ','
                    arguments.Add(ParseExpression());
                }
            }

            Match(TokenType.CloseParenthesis);
            return new FunctionCall(name, arguments);
        }

        return new VariableExpression(name);
    }

    private bool IsComparisonOperator(TokenType type)
    {
        return type == TokenType.LessThan || type == TokenType.GreaterThan ||
               type == TokenType.Equal || type == TokenType.Unequal ||
               type == TokenType.LessThanOrEqual || type == TokenType.GreaterThanOrEqual;
    }

    private bool IsMultiplicativeOperator(TokenType type)
    {
        return type == TokenType.MultiplySign || type == TokenType.DivideSign ||
               type == TokenType.ModuloSign;
    }

    private void Match(TokenType expected)
    {
        Token t = _tokens.Peek();
        if (t.Type != expected)
        {
            throw new UnexpectedLexemeException(expected, t);
        }

        _tokens.Advance();
    }
}