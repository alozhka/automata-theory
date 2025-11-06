using System.Text;

namespace Lexer;

public static class LexerStats
{
    public static string CollectFromFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"File not found: {path}");
        }

        string sourceCode = File.ReadAllText(path);
        return CollectFromSource(sourceCode);
    }

    /// <summary>
    /// �������� ���������� ������ �� ��������� ����
    /// </summary>
    private static string CollectFromSource(string sourceCode)
    {
        Dictionary<string, int> categories = new()
        {
            [Categories.Keywords] = 0,
            [Categories.Identifier] = 0,
            [Categories.NumberLiterals] = 0,
            [Categories.StringLiterals] = 0,
            [Categories.Operators] = 0,
            [Categories.OtherLexemes] = 0,
        };

        Lexer lexer = new(sourceCode);
        Token token;

        do
        {
            token = lexer.ParseToken();

            if (token.Type == TokenType.EndOfFile)
            {
                break;
            }

            string category = GetTokenCategory(token.Type);

            categories[category]++;
        }
        while (token.Type != TokenType.EndOfFile);

        return FormatStatistics(categories);
    }

    /// <summary>
    /// ���������� ��������� ������
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
                => Categories.Keywords,

            TokenType.Identifier => Categories.Identifier,

            TokenType.NumericLiteral => Categories.NumberLiterals,

            TokenType.StringLiteral => Categories.StringLiterals,

            TokenType.PlusSign or TokenType.MinusSign or TokenType.MultiplySign or
                TokenType.DivideSign or TokenType.ModuloSign or TokenType.ExponentiationSign or
                TokenType.Assign or TokenType.Equal or TokenType.LessThan or
                TokenType.GreaterThan or TokenType.LessThanOrEqual or TokenType.GreaterThanOrEqual or
                TokenType.LogicalAnd or TokenType.LogicalOr or TokenType.LogicalNot
                => Categories.Operators,

            _ => Categories.OtherLexemes,
        };
    }

    /// <summary>
    /// ����������� ���������� � ������
    /// </summary>
    private static string FormatStatistics(Dictionary<string, int> categories)
    {
        StringBuilder sb = new();

        string[] orderedCategories =
        [
            Categories.Keywords, Categories.Identifier, Categories.NumberLiterals, Categories.StringLiterals,
            Categories.Operators, Categories.OtherLexemes,
        ];

        foreach (string category in orderedCategories)
        {
            sb.AppendLine($"{category}: {categories[category]}");
        }

        return sb.ToString().TrimEnd();
    }

    private static class Categories
    {
        public const string Keywords = "keywords";
        public const string Identifier = "identifier";
        public const string NumberLiterals = "number literals";
        public const string StringLiterals = "string literals";
        public const string Operators = "operators";
        public const string OtherLexemes = "other lexemes";
    }
}