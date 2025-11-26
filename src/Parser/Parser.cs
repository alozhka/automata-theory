using Ast;
using Ast.Declarations;
using Ast.Expressions;

using Execution;

using Lexer;

namespace Parser;

public class Parser
{
    private readonly TokenStream _tokens;
    private readonly AstEvaluator _evaluator;

    public Parser(Context context, string code, IEnvironment environment)
    {
        _tokens = new TokenStream(code);
        _evaluator = new AstEvaluator(context, environment);
    }

    /// <summary>
    /// программа = {глобальная_инструкция}, точка_входа;
    /// </summary>
    public void ParseProgram()
    {
        while (_tokens.Peek().Type != TokenType.EndOfFile &&
               _tokens.Peek().Type != TokenType.Maincraft)
        {
            AstNode nodeGlobalDeclaration = ParseGlobalDeclaration();
            _evaluator.Evaluate(nodeGlobalDeclaration);
        }

        ParseMaincraft();
    }

    /// <summary>
    /// Выполняет разбор выражения и возвращает результат.
    /// </summary>
    public Row EvaluateExpression()
    {
        Expression expression = ParseExpression();
        double result = _evaluator.Evaluate(expression);
        return new Row(result);
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

        throw new UnexpectedLexemeException("global declaration", token);
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

        ParseType();
        string name = ParseIdentifier();
        Expression? value = null;

        if (_tokens.Peek().Type == TokenType.Assign)
        {
            _tokens.Advance();
            value = ParseExpression();
        }

        Match(TokenType.Semicolon);
        return new VariableDeclaration(name, value);
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

        ParseType();
        string name = ParseIdentifier();

        Match(TokenType.Assign);
        Expression value = ParseExpression();
        Match(TokenType.Semicolon);

        return new ConstantDeclaration(name, value);
    }

    /// <summary>
    /// точка_входа = "maincraft", "(", ")", блок_функции;
    /// </summary>
    private void ParseMaincraft()
    {
        Match(TokenType.Maincraft);
        Match(TokenType.OpenParenthesis);
        Match(TokenType.CloseParenthesis);
        ParseBlock();
    }

    /// <summary>
    /// блок_функции = "{", {инструкция_функции}, "}";
    /// </summary>
    private void ParseBlock()
    {
        Match(TokenType.OpenBrace);

        while (_tokens.Peek().Type != TokenType.CloseBrace &&
               _tokens.Peek().Type != TokenType.EndOfFile)
        {
            AstNode node = ParseStatement();
            _evaluator.Evaluate(node);
        }

        Match(TokenType.CloseBrace);
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

        throw new UnexpectedLexemeException("statement", token);
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

    private void ParseType()
    {
        Token token = _tokens.Peek();
        if (IsTypeToken(token.Type))
        {
            _tokens.Advance();
            return;
        }

        throw new UnexpectedLexemeException("type", token);
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
        return type == TokenType.Dayzint || type == TokenType.Fallout ||
               type == TokenType.Statum || type == TokenType.Strike;
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
                double numericValue = token.Value!.ToDouble();
                return new LiteralExpression(numericValue);

            case TokenType.Ready:
                _tokens.Advance();
                return new LiteralExpression(1.0);

            case TokenType.Noready:
                _tokens.Advance();
                return new LiteralExpression(0.0);

            case TokenType.Ghost:
                _tokens.Advance();
                return new LiteralExpression(0.0);

            case TokenType.Identifier:
                return ParseFunctionCallOrIdentifier();

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