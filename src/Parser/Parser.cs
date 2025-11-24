using Execution;

using Lexer;

namespace Parser;

public class Parser
{
    private readonly IEnvironment _environment;
    private readonly TokenStream _tokens;
    private readonly Dictionary<string, object> _symbols = new();

    public Parser(string code, IEnvironment environment)
    {
        _environment = environment;
        _tokens = new TokenStream(code);
    }

    /// <summary>
    /// программа = {глобальная_инструкция}, точка_входа;
    /// </summary>
    public void ParseProgram()
    {
        while (_tokens.Peek().Type != TokenType.EndOfFile &&
               _tokens.Peek().Type != TokenType.Maincraft)
        {
            ParseGlobalDeclaration();
        }

        ParseMaincraft();
    }

    /// <summary>
    /// Выполняет разбор выражения и возвращает результат.
    /// </summary>
    public Row EvaluateExpression()
    {
        object result = ParseExpression();
        return new Row(result);
    }

    /// <summary>
    /// глобальная_инструкция = объявление_переменной | объявление_константы | объявление_функции;
    /// </summary>
    private void ParseGlobalDeclaration()
    {
        Token token = _tokens.Peek();

        if (token.Type == TokenType.Monument)
        {
            ParseConstantDeclaration();
        }
        else if (IsTypeToken(token.Type) || token.Type == TokenType.Nullable)
        {
            ParseVariableDeclaration();
        }
        else
        {
            throw new UnexpectedLexemeException("global declaration", token);
        }
    }

    /// <summary>
    /// объявление_переменной = тип, идентификатор, ["=", выражение], ";";
    /// </summary>
    private void ParseVariableDeclaration()
    {
        bool isNullable = false;
        if (_tokens.Peek().Type == TokenType.Nullable)
        {
            isNullable = true;
            _tokens.Advance();
        }

        string type = ParseType();
        string name = ParseIdentifier();

        object value = GetDefaultValue(type, isNullable);

        if (_tokens.Peek().Type == TokenType.Assign)
        {
            _tokens.Advance();
            value = ParseExpression();
        }

        Match(TokenType.Semicolon);
        _symbols[name] = value;
    }

    /// <summary>
    /// объявление_константы = "monument", тип, идентификатор, ["=", выражение], ";";
    /// </summary>
    private void ParseConstantDeclaration()
    {
        Match(TokenType.Monument);

        if (_tokens.Peek().Type == TokenType.Nullable)
        {
            _tokens.Advance();
        }

        ParseType();
        string name = ParseIdentifier();

        Match(TokenType.Assign);
        object value = ParseExpression();
        Match(TokenType.Semicolon);

        _symbols[name] = value;
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
            ParseStatement();
        }

        Match(TokenType.CloseBrace);
    }

    /// <summary>
    /// инструкция_функции = объявление_переменной | объявление_константы | присваивание | условие |
    /// цикл_while | цикл_for | вызов_функции | вызов_заложенной_функции | возврат;
    /// </summary>
    private void ParseStatement()
    {
        Token token = _tokens.Peek();

        if (token.Type == TokenType.Monument)
        {
            ParseConstantDeclaration();
        }
        else if (IsTypeToken(token.Type) || token.Type == TokenType.Nullable)
        {
            ParseVariableDeclaration();
        }
        else if (token.Type == TokenType.Identifier)
        {
            ParseAssignmentOrFunctionCall();
        }
        else if (token.Type == TokenType.Exodus || token.Type == TokenType.Exodusln)
        {
            ParseOutputStatement();
        }
        else if (token.Type == TokenType.Raid)
        {
            ParseInputStatement();
        }
        else
        {
            throw new UnexpectedLexemeException("statement", token);
        }
    }

    private void ParseInputStatement()
    {
        Match(TokenType.Raid);
        Match(TokenType.OpenParenthesis);

        string variableName = ParseIdentifier();

        if (!_symbols.ContainsKey(variableName))
        {
            throw new Exception($"Undeclared variable: {variableName}");
        }

        object inputValue = _environment.ReadInput();
        _symbols[variableName] = inputValue;

        Match(TokenType.CloseParenthesis);
        Match(TokenType.Semicolon);
    }

    private void ParseAssignmentOrFunctionCall()
    {
        string name = ParseIdentifier();

        if (_tokens.Peek().Type == TokenType.Assign)
        {
            ParseAssignment(name);
        }
        else if (_tokens.Peek().Type == TokenType.OpenParenthesis)
        {
            throw new Exception($"Function calls are not supported yet: {name}");
        }
        else
        {
            throw new UnexpectedLexemeException("assignment or function call", _tokens.Peek());
        }
    }

    private void ParseAssignment(string variableName)
    {
        Match(TokenType.Assign);
        object value = ParseExpression();
        Match(TokenType.Semicolon);

        if (!_symbols.ContainsKey(variableName))
        {
            throw new Exception($"Undeclared variable: {variableName}");
        }

        _symbols[variableName] = value;
    }

    private void ParseOutputStatement()
    {
        _tokens.Advance();
        Match(TokenType.OpenParenthesis);

        object value = ParseExpression();
        double result = Convert.ToDouble(value);
        _environment.AddResult(result);

        Match(TokenType.CloseParenthesis);
        Match(TokenType.Semicolon);
    }

    private string ParseType()
    {
        Token token = _tokens.Peek();
        if (IsTypeToken(token.Type))
        {
            _tokens.Advance();
            return token.Type.ToString().ToLower();
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

    private object GetDefaultValue(string type, bool isNullable)
    {
        if (isNullable)
        {
            return null!;
        }

        return type.ToLower() switch
        {
            "dayzint" => 0m,
            "fallout" => 0.0m,
            "statum" => false,
            "strike" => "",
            _ => throw new Exception($"Unknown type: {type}"),
        };
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
    private object ParseExpression()
    {
        return ParseLogicalExpression();
    }

    /// <summary>
    /// Разбирает логическое выражение.
    /// Правила: логическое_выражение = выражение_сравнения, {логический_оператор, выражение_сравнения};
    /// </summary>
    private object ParseLogicalExpression()
    {
        return ParseLogicalOrExpression();
    }

    /// <summary>
    /// Разбирает логическое ИЛИ выражение.
    /// Правила: логическое_ИЛИ_выражение = логическое_И_выражение, { "||", логическое_И_выражение };
    /// </summary>
    private object ParseLogicalOrExpression()
    {
        object left = ParseLogicalAndExpression();

        while (_tokens.Peek().Type == TokenType.LogicalOr)
        {
            _tokens.Advance();

            object right = ParseLogicalAndExpression();

            bool leftBool = Convert.ToBoolean(left);
            bool rightBool = Convert.ToBoolean(right);
            left = leftBool || rightBool;
        }

        return left;
    }

    /// <summary>
    /// Разбирает логическое И выражение.
    /// Правила: логическое_И_выражение = выражение_сравнения, { "&&", выражение_сравнения };
    /// </summary>
    private object ParseLogicalAndExpression()
    {
        object left = ParseComparisonExpression();

        while (_tokens.Peek().Type == TokenType.LogicalAnd)
        {
            _tokens.Advance();

            object right = ParseComparisonExpression();

            bool leftBool = Convert.ToBoolean(left);
            bool rightBool = Convert.ToBoolean(right);
            left = leftBool && rightBool;
        }

        return left;
    }

    /// <summary>
    /// Разбирает выражение сравнения.
    /// Правила: выражение_сравнения = аддитивное_выражение, [оператор_сравнения, аддитивное_выражение];
    /// </summary>
    private object ParseComparisonExpression()
    {
        object left = ParseAdditiveExpression();

        if (IsComparisonOperator(_tokens.Peek().Type))
        {
            Token operatorToken = _tokens.Peek();
            _tokens.Advance();

            object right = ParseAdditiveExpression();
            left = EvaluateComparison(left, right, operatorToken.Type);
        }

        return left;
    }

    /// <summary>
    /// Разбирает аддитивное выражение.
    /// Правила: аддитивное_выражение = мультипликативное_выражение, {адитивный_оператор, мультипликативное_выражение};
    /// </summary>
    private object ParseAdditiveExpression()
    {
        object left = ParseMultiplicativeExpression();

        while (_tokens.Peek().Type == TokenType.PlusSign || _tokens.Peek().Type == TokenType.MinusSign)
        {
            Token operatorToken = _tokens.Peek();
            _tokens.Advance();

            object right = ParseMultiplicativeExpression();
            left = EvaluateAdditiveOperator(left, right, operatorToken.Type);
        }

        return left;
    }

    /// <summary>
    /// Разбирает мультипликативное выражение.
    /// Правила: мультипликативное_выражение = унарное_выражение, {мультипликативный_оператор, унарное_выражение};
    /// </summary>
    private object ParseMultiplicativeExpression()
    {
        object left = ParseUnaryExpression();

        while (IsMultiplicativeOperator(_tokens.Peek().Type))
        {
            Token operatorToken = _tokens.Peek();
            _tokens.Advance();

            object right = ParseUnaryExpression();
            left = EvaluateMultiplicativeOperator(left, right, operatorToken.Type);
        }

        return left;
    }

    /// <summary>
    /// Разбирает унарное выражение.
    /// Правила: унарное_выражение = [унарный_оператор], часть_выражения;
    /// </summary>
    private object ParseUnaryExpression()
    {
        if (_tokens.Peek().Type == TokenType.MinusSign || _tokens.Peek().Type == TokenType.LogicalNot)
        {
            Token operatorToken = _tokens.Peek();
            _tokens.Advance();

            object operand = ParseExpressionPart();
            return EvaluateUnaryOperator(operand, operatorToken.Type);
        }

        return ParseExpressionPart();
    }

    /// <summary>
    /// Разбирает часть выражения.
    /// Правила: часть_выражения = литерал_выражения | "(", выражение, ")";
    /// </summary>
    private object ParseExpressionPart()
    {
        Token token = _tokens.Peek();

        switch (token.Type)
        {
            case TokenType.NumericLiteral:
                _tokens.Advance();
                return token.Value!.ToDecimal();

            case TokenType.StringLiteral:
                _tokens.Advance();
                return token.Value!.ToString();

            case TokenType.Ready:
                _tokens.Advance();
                return true;

            case TokenType.Noready:
                _tokens.Advance();
                return false;

            case TokenType.Ghost:
                _tokens.Advance();
                return null!;

            case TokenType.Identifier:
                return ParseFunctionCallOrIdentifier();

            case TokenType.OpenParenthesis:
                _tokens.Advance();
                object result = ParseExpression();
                Match(TokenType.CloseParenthesis);
                return result;

            default:
                throw new UnexpectedLexemeException("expression part", token);
        }
    }

    private object ParseFunctionCallOrIdentifier()
    {
        Token identifierToken = _tokens.Peek();
        string name = identifierToken.Value?.ToString() ?? throw new InvalidOperationException("Identifier token value is null");
        _tokens.Advance();

        if (_tokens.Peek().Type == TokenType.OpenParenthesis)
        {
            _tokens.Advance();

            List<decimal> arguments = new List<decimal>();

            if (_tokens.Peek().Type != TokenType.CloseParenthesis)
            {
                arguments.Add(Convert.ToDecimal(ParseExpression()));

                while (_tokens.Peek().Type == TokenType.Comma)
                {
                    _tokens.Advance();
                    arguments.Add(Convert.ToDecimal(ParseExpression()));
                }
            }

            Match(TokenType.CloseParenthesis);

            if (string.IsNullOrEmpty(name))
            {
                throw new InvalidOperationException("Function name cannot be null or empty");
            }

            return BuiltinFunctions.Invoke(name, arguments);
        }
        else
        {
            string variableName = name;
            if (string.IsNullOrEmpty(variableName))
            {
                throw new InvalidOperationException("Variable name cannot be null or empty");
            }

            if (!_symbols.TryGetValue(variableName, out object? value))
            {
                throw new Exception($"Undeclared variable: {variableName}");
            }

            return value;
        }
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

    private object EvaluateUnaryOperator(object operand, TokenType operatorType)
    {
        return operatorType switch
        {
            TokenType.MinusSign => -Convert.ToDecimal(operand),
            TokenType.LogicalNot => !Convert.ToBoolean(operand),
            _ => throw new Exception($"Unsupported unary operator: {operatorType}"),
        };
    }

    private object EvaluateMultiplicativeOperator(object left, object right, TokenType operatorType)
    {
        decimal leftNum = Convert.ToDecimal(left);
        decimal rightNum = Convert.ToDecimal(right);

        return operatorType switch
        {
            TokenType.MultiplySign => leftNum * rightNum,
            TokenType.DivideSign => rightNum != 0 ? leftNum / rightNum : throw new DivideByZeroException(),
            TokenType.ModuloSign => rightNum != 0 ? leftNum % rightNum : throw new DivideByZeroException(),
            _ => throw new Exception($"Unsupported multiplicative operator: {operatorType}"),
        };
    }

    private object EvaluateAdditiveOperator(object left, object right, TokenType operatorType)
    {
        if (left is string || right is string)
        {
            return (left.ToString() ?? "") + (right.ToString() ?? "");
        }

        if (left.GetType() != right.GetType())
        {
            throw new Exception($"Cannot compare different types: {left.GetType().Name} and {right.GetType().Name}");
        }

        decimal leftNum = Convert.ToDecimal(left);
        decimal rightNum = Convert.ToDecimal(right);

        return operatorType switch
        {
            TokenType.PlusSign => leftNum + rightNum,
            TokenType.MinusSign => leftNum - rightNum,
            _ => throw new Exception($"Unsupported additive operator: {operatorType}"),
        };
    }

    private object EvaluateComparison(object left, object right, TokenType operatorType)
    {
        if (left.GetType() == right.GetType())
        {
            return operatorType switch
            {
                TokenType.LessThan => Convert.ToDecimal(left) < Convert.ToDecimal(right),
                TokenType.GreaterThan => Convert.ToDecimal(left) > Convert.ToDecimal(right),
                TokenType.LessThanOrEqual => Convert.ToDecimal(left) <= Convert.ToDecimal(right),
                TokenType.GreaterThanOrEqual => Convert.ToDecimal(left) >= Convert.ToDecimal(right),
                TokenType.Equal => left.Equals(right),
                TokenType.Unequal => !left.Equals(right),
                _ => throw new Exception($"Unsupported comparison operator: {operatorType}"),
            };
        }

        throw new Exception($"Cannot compare different types: {left.GetType().Name} and {right.GetType().Name}");
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