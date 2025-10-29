using SqlLexer;

namespace SqlLexer.UnitTests;

public class LexerTest
{
    [Theory]
    [MemberData(nameof(GetTokenizeSqlData))]
    public void Can_tokenize_SQL(string sql, List<Token> expected)
    {
        List<Token> actual = Tokenize(sql);
        Assert.Equal(expected, actual);
    }

    public static TheoryData<string, List<Token>> GetTokenizeSqlData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
				"dayzint x;", [
                new Token(TokenType.Dayzint),
                new Token(TokenType.Identifier, new TokenValue("x")),
                new Token(TokenType.Semicolon),
                ]
            },
			{
				"?dayzint x;", [
				new Token(TokenType.Nullable),
				new Token(TokenType.Dayzint),
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.Semicolon),
				]
			},
			{
				"dayzint x = 5;", [
				new Token(TokenType.Dayzint),
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.Assign),
				new Token(TokenType.NumericLiteral, new TokenValue(5)),
				new Token(TokenType.Semicolon),
				]
			},
			{
				"fallout x;", [
				new Token(TokenType.Fallout),
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.Semicolon),
				]
			},
			{
				"?fallout x;", [
				new Token(TokenType.Nullable),
				new Token(TokenType.Fallout),
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.Semicolon),
				]
			},
			{
				"fallout x = 5;", [
				new Token(TokenType.Fallout),
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.Assign),
				new Token(TokenType.NumericLiteral, new TokenValue(5)),
				new Token(TokenType.Semicolon),
				]
			},
			{
				"statum x;", [
				new Token(TokenType.Statum),
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.Semicolon),
				]
			},
			{
				"?statum x;", [
				new Token(TokenType.Nullable),
				new Token(TokenType.Statum),
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.Semicolon),
				]
			},
			{
				"statum x = ready;", [
				new Token(TokenType.Statum),
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.Assign),
				new Token(TokenType.Ready),
				new Token(TokenType.Semicolon),
				]
			},
			{
				"strike x;", [
				new Token(TokenType.Strike),
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.Semicolon),
				]
			},
			{
				"?strike x;", [
				new Token(TokenType.Nullable),
				new Token(TokenType.Strike),
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.Semicolon),
				]
			},
			{
				"strike x = \"sere\";", [
				new Token(TokenType.Strike),
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.Assign),
				new Token(TokenType.StringLiteral, new TokenValue("sere")),
				new Token(TokenType.Semicolon),
				]
			},
			{
				"araya<dayzint> x;", [
				new Token(TokenType.Araya),
				new Token(TokenType.LessThan),
				new Token(TokenType.Dayzint),
				new Token(TokenType.GreaterThan),
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.Semicolon),
				]
			},
			{
				"?araya<dayzint> x;", [
				new Token(TokenType.Nullable),
				new Token(TokenType.Araya),
				new Token(TokenType.LessThan),
				new Token(TokenType.Dayzint),
				new Token(TokenType.GreaterThan),
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.Semicolon),
				]
			},
			{
				"araya<dayzint> x = {4, 5};", [
				new Token(TokenType.Araya),
				new Token(TokenType.LessThan),
				new Token(TokenType.Dayzint),
				new Token(TokenType.GreaterThan),
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.Assign),
				new Token(TokenType.OpenBrace),
				new Token(TokenType.NumericLiteral, new TokenValue(4)),
				new Token(TokenType.Comma),
				new Token(TokenType.NumericLiteral, new TokenValue(5)),
				new Token(TokenType.CloseBrace),
				new Token(TokenType.Semicolon),
				]
			},
			{
				"raid(x);", [
				new Token(TokenType.Raid),
				new Token(TokenType.OpenParenthesis),
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.CloseParenthesis),
				new Token(TokenType.Semicolon),
				]
			},
			{
				"exodus(x);", [
				new Token(TokenType.Exodus),
				new Token(TokenType.OpenParenthesis),
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.CloseParenthesis),
				new Token(TokenType.Semicolon),
				]
			},
			{
				"exodus(ghost);", [
				new Token(TokenType.Exodus),
				new Token(TokenType.OpenParenthesis),
				new Token(TokenType.Ghost),
				new Token(TokenType.CloseParenthesis),
				new Token(TokenType.Semicolon),
				]
			},
			{
				"exodusln(x);", [
				new Token(TokenType.Exodusln),
				new Token(TokenType.OpenParenthesis),
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.CloseParenthesis),
				new Token(TokenType.Semicolon),
				]
			},
			{
				"iffy (x == 5) {", [
				new Token(TokenType.Iffy),
				new Token(TokenType.OpenParenthesis),
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.Equal),
				new Token(TokenType.NumericLiteral, new TokenValue(5)),
				new Token(TokenType.CloseParenthesis),
				new Token(TokenType.OpenBrace),
				]
			},
			{
				"} elysian {", [
				new Token(TokenType.CloseBrace),
				new Token(TokenType.Elysian),
				new Token(TokenType.OpenBrace),
				]
			},
			{
				"} elysiffy (x == 5) {", [
				new Token(TokenType.CloseBrace),
				new Token(TokenType.Elysiffy),
				new Token(TokenType.OpenParenthesis),
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.Equal),
				new Token(TokenType.NumericLiteral, new TokenValue(5)),
				new Token(TokenType.CloseParenthesis),
				new Token(TokenType.OpenBrace),
				]
			},
			{
				"valorant (x < 5) {", [
				new Token(TokenType.Valorant),
				new Token(TokenType.OpenParenthesis),
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.LessThan),
				new Token(TokenType.NumericLiteral, new TokenValue(5)),
				new Token(TokenType.CloseParenthesis),
				new Token(TokenType.OpenBrace),
				]
			},
			{
				"forza (dayzint j = 0; j < 5; j = j + 1) {", [
				new Token(TokenType.Forza),
				new Token(TokenType.OpenParenthesis),
				new Token(TokenType.Dayzint),
				new Token(TokenType.Identifier, new TokenValue("j")),
				new Token(TokenType.Assign),
				new Token(TokenType.NumericLiteral, new TokenValue(0)),
				new Token(TokenType.Semicolon),
				new Token(TokenType.Identifier, new TokenValue("j")),
				new Token(TokenType.LessThan),
				new Token(TokenType.NumericLiteral, new TokenValue(5)),
				new Token(TokenType.Semicolon),
				new Token(TokenType.Identifier, new TokenValue("j")),
				new Token(TokenType.Assign),
				new Token(TokenType.Identifier, new TokenValue("j")),
				new Token(TokenType.PlusSign),
				new Token(TokenType.NumericLiteral, new TokenValue(1)),
				new Token(TokenType.CloseParenthesis),
				new Token(TokenType.OpenBrace),
				]
			},
			{
				"breakout;", [
				new Token(TokenType.Breakout),
				new Token(TokenType.Semicolon),
				]
			},
			{
				"contra;", [
				new Token(TokenType.Contra),
				new Token(TokenType.Semicolon),
				]
			},
			{
				"funkotron x(): dayzint {", [
				new Token(TokenType.Funkotron),
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.OpenParenthesis),
				new Token(TokenType.CloseParenthesis),
				new Token(TokenType.Colon),
				new Token(TokenType.Dayzint),
				new Token(TokenType.OpenBrace),
				]
			},
			{
				"funkotron divire(dayzint a, dayzint b): ?dayzint {", [
				new Token(TokenType.Funkotron),
				new Token(TokenType.Identifier, new TokenValue("divire")),
				new Token(TokenType.OpenParenthesis),
				new Token(TokenType.Dayzint),
				new Token(TokenType.Identifier, new TokenValue("a")),
				new Token(TokenType.Comma),
				new Token(TokenType.Dayzint),
				new Token(TokenType.Identifier, new TokenValue("b")),
				new Token(TokenType.CloseParenthesis),
				new Token(TokenType.Colon),
				new Token(TokenType.Nullable),
				new Token(TokenType.Dayzint),
				new Token(TokenType.OpenBrace),
				]
			},
			{
				"returnal z / b;", [
				new Token(TokenType.Returnal),
				new Token(TokenType.Identifier, new TokenValue("z")),
				new Token(TokenType.DivideSign),
				new Token(TokenType.Identifier, new TokenValue("b")),
				new Token(TokenType.Semicolon),
				]
			},
			{
				"x + y", [
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.PlusSign),
				new Token(TokenType.Identifier, new TokenValue("y")),
				]
			},
			{
				"x - y", [
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.MinusSign),
				new Token(TokenType.Identifier, new TokenValue("y")),
				]
			},
			{
				"x * y", [
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.MultiplySign),
				new Token(TokenType.Identifier, new TokenValue("y")),
				]
			},
			{
				"x / y", [
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.DivideSign),
				new Token(TokenType.Identifier, new TokenValue("y")),
				]
			},
			{
				"x % y", [
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.ModuloSign),
				new Token(TokenType.Identifier, new TokenValue("y")),
				]
			},
			{
				"x = 5", [
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.Assign),
				new Token(TokenType.NumericLiteral, new TokenValue(5)),
				]
			},
			{
				"x == y", [
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.Equal),
				new Token(TokenType.Identifier, new TokenValue("y")),
				]
			},
			{
				"x != y", [
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.Unequal),
				new Token(TokenType.Identifier, new TokenValue("y")),
				]
			},
			{
				"x < y", [
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.LessThan),
				new Token(TokenType.Identifier, new TokenValue("y")),
				]
			},
			{
				"x > y", [
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.GreaterThan),
				new Token(TokenType.Identifier, new TokenValue("y")),
				]
			},
			{
				"x <= y", [
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.LessThanOrEqual),
				new Token(TokenType.Identifier, new TokenValue("y")),
				]
			},
			{
				"x >= y", [
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.GreaterThanOrEqual),
				new Token(TokenType.Identifier, new TokenValue("y")),
				]
			},
			{
				"x && y", [
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.LogicalAnd),
				new Token(TokenType.Identifier, new TokenValue("y")),
				]
			},
			{
				"x || y", [
				new Token(TokenType.Identifier, new TokenValue("x")),
				new Token(TokenType.LogicalOr),
				new Token(TokenType.Identifier, new TokenValue("y")),
				]
			},
			{
				"!x", [
				new Token(TokenType.LogicalNot),
				new Token(TokenType.Identifier, new TokenValue("x")),
				]
			}
		};
    }

    private List<Token> Tokenize(string sql)
    {
        List<Token> results = [];
        Lexer lexer = new(sql);

        for (Token t = lexer.ParseToken(); t.Type != TokenType.EndOfFile; t = lexer.ParseToken())
        {
            results.Add(t);
        }

        return results;
    }
}