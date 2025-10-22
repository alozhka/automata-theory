using System.Text;

namespace SqlLexer;

public static class LexicalStats
{
	public static string CollectFromFile(string path)
	{
		if (!File.Exists(path))
			throw new FileNotFoundException($"File not found: {path}");

		string sourceCode = File.ReadAllText(path);
		return CollectFromSource(sourceCode);
	}

	/// <summary>
	/// Собирает статистику лексем из исходного кода
	/// </summary>
	private static string CollectFromSource(string sourceCode)
	{
		var categories = new Dictionary<string, int>
		{
			["keywords"] = 0,
			["identifier"] = 0,
			["number literals"] = 0,
			["string literals"] = 0,
			["operators"] = 0,
			["other lexemes"] = 0
		};

		Lexer lexer = new(sourceCode);
		Token token;

		do
		{
			token = lexer.ParseToken();

			if (token.Type == TokenType.EndOfFile)
				break;

			string category = GetTokenCategory(token.Type);

			categories[category]++;
		}
		while (token.Type != TokenType.EndOfFile);

		return FormatStatistics(categories);
	}

	/// <summary>
	/// Определяет категорию токена
	/// </summary>
	private static string GetTokenCategory(TokenType type)
	{
		return type switch
		{
			TokenType.Dayzint or TokenType.Fallout or TokenType.Statum or
			TokenType.Strike or TokenType.Araya or TokenType.Ghost or
			TokenType.Ready or TokenType.Noready or TokenType.Iffy or
			TokenType.Elysian or TokenType.Elysiffy or TokenType.Valorant or
			TokenType.Forza or TokenType.Breakout or TokenType.Contra or
			TokenType.Funkotron or TokenType.Returnal or TokenType.Raid or
			TokenType.Exodus or TokenType.Exodusln
				=> "keywords",

			TokenType.Identifier => "identifier",

			TokenType.NumericLiteral => "number literals",

			TokenType.StringLiteral => "string literals",

			TokenType.PlusSign or TokenType.MinusSign or TokenType.MultiplySign or
			TokenType.DivideSign or TokenType.ModuloSign or TokenType.ExponentiationSign or
			TokenType.Assign or TokenType.Equal or TokenType.LessThan or
			TokenType.GreaterThan or TokenType.LessThanOrEqual or TokenType.GreaterThanOrEqual or 
			TokenType.LogicalAnd or TokenType.LogicalOr or TokenType.LogicalNot
				=> "operators",

			_ => "other lexemes"
		};
	}

	/// <summary>
	/// Форматирует статистику в строку
	/// </summary>
	private static string FormatStatistics(Dictionary<string, int> categories)
	{
		var sb = new StringBuilder();

		string[] orderedCategories = {
			"keywords",
			"identifier",
			"number literals",
			"string literals",
			"operators",
			"other lexemes"
		};

		foreach (string category in orderedCategories)
		{
			sb.AppendLine($"{category}: {categories[category]}");
		}

		return sb.ToString().TrimEnd();
	}
}