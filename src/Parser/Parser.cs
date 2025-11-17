using SqlLexer;

namespace Parser;

/// <summary>
/// Выполняет синтаксический разбор выражений языка MysticGameScript.
/// Грамматика описана в файле `docs/specification/expressions-grammar.md`.
/// </summary>
public class Parser
{
	private readonly TokenStream _tokens;

	private Parser(string code)
	{
		_tokens = new TokenStream(code);
	}

	/// <summary>
	/// Выполняет разбор выражения и возвращает результат.
	/// </summary>
	public static Row EvaluateExpression(string code)
	{
		Parser p = new(code);
		object result = p.ParseExpression();
		return new Row(result);
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
			Token operatorToken = _tokens.Peek();
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
			Token operatorToken = _tokens.Peek();
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

			// Вместо рекурсивного вызова ParseUnaryExpression() вызываем ParseExpressionPart()
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
				return token.Value!.ToString() ?? "";

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
		string functionName = identifierToken.Value!.ToString();
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
			return BuiltinFunctions.Invoke(functionName, arguments);
		}

		throw new UnexpectedLexemeException("function call", identifierToken);
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
			_ => throw new Exception($"Unsupported unary operator: {operatorType}")
		};
	}

	private object EvaluateMultiplicativeOperator(object left, object right, TokenType operatorType)
	{
		if (left?.GetType() != right?.GetType())
		{
			throw new Exception($"Cannot compare different types: {left?.GetType().Name} and {right?.GetType().Name}");
		}
		decimal leftNum = Convert.ToDecimal(left);
		decimal rightNum = Convert.ToDecimal(right);

		return operatorType switch
		{
			TokenType.MultiplySign => leftNum * rightNum,
			TokenType.DivideSign => rightNum != 0 ? leftNum / rightNum : throw new DivideByZeroException(),
			TokenType.ModuloSign => rightNum != 0 ? leftNum % rightNum : throw new DivideByZeroException(),
			_ => throw new Exception($"Unsupported multiplicative operator: {operatorType}")
		};
	}

	private object EvaluateAdditiveOperator(object left, object right, TokenType operatorType)
	{
		if (left is string || right is string)
		{
			return (left?.ToString() ?? "") + (right?.ToString() ?? "");
		}

		if (left?.GetType() != right?.GetType())
		{
			throw new Exception($"Cannot compare different types: {left?.GetType().Name} and {right?.GetType().Name}");
		}

		decimal leftNum = Convert.ToDecimal(left);
		decimal rightNum = Convert.ToDecimal(right);

		return operatorType switch
		{
			TokenType.PlusSign => leftNum + rightNum,
			TokenType.MinusSign => leftNum - rightNum,
			_ => throw new Exception($"Unsupported additive operator: {operatorType}")
		};
	}

	private object EvaluateComparison(object left, object right, TokenType operatorType)
	{
		if (left?.GetType() == right?.GetType())
		{
			return operatorType switch
			{
				TokenType.LessThan => Convert.ToDecimal(left) < Convert.ToDecimal(right),
				TokenType.GreaterThan => Convert.ToDecimal(left) > Convert.ToDecimal(right),
				TokenType.LessThanOrEqual => Convert.ToDecimal(left) <= Convert.ToDecimal(right),
				TokenType.GreaterThanOrEqual => Convert.ToDecimal(left) >= Convert.ToDecimal(right),
				TokenType.Equal => left!.Equals(right),
				TokenType.Unequal => !left!.Equals(right),
				_ => throw new Exception($"Unsupported comparison operator: {operatorType}")
			};
		}

		throw new Exception($"Cannot compare different types: {left?.GetType().Name} and {right?.GetType().Name}");
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