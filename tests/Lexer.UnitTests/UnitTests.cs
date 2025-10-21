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
                "SELECT 2025;", [
                    new Token(TokenType.Select),
                    new Token(TokenType.NumericLiteral, new TokenValue(2025)),
                    new Token(TokenType.Semicolon),
                ]
            },
            {
                "SELECT first_name FROM student;", [
                    new Token(TokenType.Select),
                    new Token(TokenType.Identifier, new TokenValue("first_name")),
                    new Token(TokenType.From),
                    new Token(TokenType.Identifier, new TokenValue("student")),
                    new Token(TokenType.Semicolon),
                ]
            },
            {
                "SELECT first_name, last_name, email FROM student;", [
                    new Token(TokenType.Select),
                    new Token(TokenType.Identifier, new TokenValue("first_name")),
                    new Token(TokenType.Comma),
                    new Token(TokenType.Identifier, new TokenValue("last_name")),
                    new Token(TokenType.Comma),
                    new Token(TokenType.Identifier, new TokenValue("email")),
                    new Token(TokenType.From),
                    new Token(TokenType.Identifier, new TokenValue("student")),
                    new Token(TokenType.Semicolon),
                ]
            },
            {
                "select first_name, last_name, email FrOM student;", [
                    new Token(TokenType.Select),
                    new Token(TokenType.Identifier, new TokenValue("first_name")),
                    new Token(TokenType.Comma),
                    new Token(TokenType.Identifier, new TokenValue("last_name")),
                    new Token(TokenType.Comma),
                    new Token(TokenType.Identifier, new TokenValue("email")),
                    new Token(TokenType.From),
                    new Token(TokenType.Identifier, new TokenValue("student")),
                    new Token(TokenType.Semicolon),
                ]
            },
            {
                "SELECT count + 1 FROM counter;", [
                    new Token(TokenType.Select),
                    new Token(TokenType.Identifier, new TokenValue("count")),
                    new Token(TokenType.PlusSign),
                    new Token(TokenType.NumericLiteral, new TokenValue(1)),
                    new Token(TokenType.From),
                    new Token(TokenType.Identifier, new TokenValue("counter")),
                    new Token(TokenType.Semicolon),
                ]
            },
            {
                "SELECT starts_at - ends_at + 1 FROM meeting;", [
                    new Token(TokenType.Select),
                    new Token(TokenType.Identifier, new TokenValue("starts_at")),
                    new Token(TokenType.MinusSign),
                    new Token(TokenType.Identifier, new TokenValue("ends_at")),
                    new Token(TokenType.PlusSign),
                    new Token(TokenType.NumericLiteral, new TokenValue(1)),
                    new Token(TokenType.From),
                    new Token(TokenType.Identifier, new TokenValue("meeting")),
                    new Token(TokenType.Semicolon),
                ]
            },
            {
                "SELECT to_be OR NOT to_be;", [
                    new Token(TokenType.Select),
                    new Token(TokenType.Identifier, new TokenValue("to_be")),
                    new Token(TokenType.Or),
                    new Token(TokenType.Not),
                    new Token(TokenType.Identifier, new TokenValue("to_be")),
                    new Token(TokenType.Semicolon),
                ]
            },
            {
                "SELECT id FROM circle WHERE radius > 10 AND radius < 25 AND NOT is_deleted;", [
                    new Token(TokenType.Select),
                    new Token(TokenType.Identifier, new TokenValue("id")),
                    new Token(TokenType.From),
                    new Token(TokenType.Identifier, new TokenValue("circle")),
                    new Token(TokenType.Where),
                    new Token(TokenType.Identifier, new TokenValue("radius")),
                    new Token(TokenType.GreaterThan),
                    new Token(TokenType.NumericLiteral, new TokenValue(10)),
                    new Token(TokenType.And),
                    new Token(TokenType.Identifier, new TokenValue("radius")),
                    new Token(TokenType.LessThan),
                    new Token(TokenType.NumericLiteral, new TokenValue(25)),
                    new Token(TokenType.And),
                    new Token(TokenType.Not),
                    new Token(TokenType.Identifier, new TokenValue("is_deleted")),
                    new Token(TokenType.Semicolon),
                ]
            },
            {
                "SELECT id FROM circle WHERE radius >= 10 AND radius <= 25;", [
                    new Token(TokenType.Select),
                    new Token(TokenType.Identifier, new TokenValue("id")),
                    new Token(TokenType.From),
                    new Token(TokenType.Identifier, new TokenValue("circle")),
                    new Token(TokenType.Where),
                    new Token(TokenType.Identifier, new TokenValue("radius")),
                    new Token(TokenType.GreaterThanOrEqual),
                    new Token(TokenType.NumericLiteral, new TokenValue(10)),
                    new Token(TokenType.And),
                    new Token(TokenType.Identifier, new TokenValue("radius")),
                    new Token(TokenType.LessThanOrEqual),
                    new Token(TokenType.NumericLiteral, new TokenValue(25)),
                    new Token(TokenType.Semicolon),
                ]
            },
            {
                """
                --Find circles with radius
                -- in range [10; 25]
                SELECT id -- column name
                FROM circle -- table name
                WHERE radius >= 10 AND radius <= 25;
                """,
                [
                    new Token(TokenType.Select),
                    new Token(TokenType.Identifier, new TokenValue("id")),
                    new Token(TokenType.From),
                    new Token(TokenType.Identifier, new TokenValue("circle")),
                    new Token(TokenType.Where),
                    new Token(TokenType.Identifier, new TokenValue("radius")),
                    new Token(TokenType.GreaterThanOrEqual),
                    new Token(TokenType.NumericLiteral, new TokenValue(10)),
                    new Token(TokenType.And),
                    new Token(TokenType.Identifier, new TokenValue("radius")),
                    new Token(TokenType.LessThanOrEqual),
                    new Token(TokenType.NumericLiteral, new TokenValue(25)),
                    new Token(TokenType.Semicolon),
                ]
            },
            {
                """
                /*
                  Find circles with radius
                  in range [10; 25]
                */
                SELECT id /* column name */
                FROM circle /* table name */
                WHERE radius >= 10 AND radius <= 25;
                """,
                [
                    new Token(TokenType.Select),
                    new Token(TokenType.Identifier, new TokenValue("id")),
                    new Token(TokenType.From),
                    new Token(TokenType.Identifier, new TokenValue("circle")),
                    new Token(TokenType.Where),
                    new Token(TokenType.Identifier, new TokenValue("radius")),
                    new Token(TokenType.GreaterThanOrEqual),
                    new Token(TokenType.NumericLiteral, new TokenValue(10)),
                    new Token(TokenType.And),
                    new Token(TokenType.Identifier, new TokenValue("radius")),
                    new Token(TokenType.LessThanOrEqual),
                    new Token(TokenType.NumericLiteral, new TokenValue(25)),
                    new Token(TokenType.Semicolon),
                ]
            },
            {
                "SELECT radius, 2 * 3.14159265358 * radius FROM circle;", [
                    new Token(TokenType.Select),
                    new Token(TokenType.Identifier, new TokenValue("radius")),
                    new Token(TokenType.Comma),
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                    new Token(TokenType.MultiplySign),
                    new Token(TokenType.NumericLiteral, new TokenValue(3.14159265358m)),
                    new Token(TokenType.MultiplySign),
                    new Token(TokenType.Identifier, new TokenValue("radius")),
                    new Token(TokenType.From),
                    new Token(TokenType.Identifier, new TokenValue("circle")),
                    new Token(TokenType.Semicolon),
                ]
            },
            {
                "SELECT duration / 2 FROM phone_call;", [
                    new Token(TokenType.Select),
                    new Token(TokenType.Identifier, new TokenValue("duration")),
                    new Token(TokenType.DivideSign),
                    new Token(TokenType.NumericLiteral, new TokenValue(2m)),
                    new Token(TokenType.From),
                    new Token(TokenType.Identifier, new TokenValue("phone_call")),
                    new Token(TokenType.Semicolon),
                ]
            },
            {
                "SELECT first_name + ' ' + last_name FROM student;", [
                    new Token(TokenType.Select),
                    new Token(TokenType.Identifier, new TokenValue("first_name")),
                    new Token(TokenType.PlusSign),
                    new Token(TokenType.StringLiteral, new TokenValue(" ")),
                    new Token(TokenType.PlusSign),
                    new Token(TokenType.Identifier, new TokenValue("last_name")),
                    new Token(TokenType.From),
                    new Token(TokenType.Identifier, new TokenValue("student")),
                    new Token(TokenType.Semicolon),
                ]
            },
            {
                """
                SELECT
                  '\\' AS backslash,
                  '\'' AS single_quote
                ;
                """,
                [
                    new Token(TokenType.Select),
                    new Token(TokenType.StringLiteral, new TokenValue("\\")),
                    new Token(TokenType.As),
                    new Token(TokenType.Identifier, new TokenValue("backslash")),
                    new Token(TokenType.Comma),
                    new Token(TokenType.StringLiteral, new TokenValue("'")),
                    new Token(TokenType.As),
                    new Token(TokenType.Identifier, new TokenValue("single_quote")),
                    new Token(TokenType.Semicolon),
                ]
            },
            {
                "SELECT MIN(admission_year), MAX(admission_year) FROM student;", [
                    new Token(TokenType.Select),
                    new Token(TokenType.Identifier, new TokenValue("MIN")),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.Identifier, new TokenValue("admission_year")),
                    new Token(TokenType.CloseParenthesis),
                    new Token(TokenType.Comma),
                    new Token(TokenType.Identifier, new TokenValue("MAX")),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.Identifier, new TokenValue("admission_year")),
                    new Token(TokenType.CloseParenthesis),
                    new Token(TokenType.From),
                    new Token(TokenType.Identifier, new TokenValue("student")),
                    new Token(TokenType.Semicolon),
                ]
            },
            {
                "SELECT 5 / 2, 5 % 2;", [
                    new Token(TokenType.Select),
                    new Token(TokenType.NumericLiteral, new TokenValue(5)),
                    new Token(TokenType.DivideSign),
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                    new Token(TokenType.Comma),
                    new Token(TokenType.NumericLiteral, new TokenValue(5)),
                    new Token(TokenType.ModuloSign),
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                    new Token(TokenType.Semicolon),
                ]
            },
            {
                "SELECT 2 ^ 3 ^ 2;", [
                    new Token(TokenType.Select),
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                    new Token(TokenType.ExponentiationSign),
                    new Token(TokenType.NumericLiteral, new TokenValue(3)),
                    new Token(TokenType.ExponentiationSign),
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                    new Token(TokenType.Semicolon),
                ]
            },
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