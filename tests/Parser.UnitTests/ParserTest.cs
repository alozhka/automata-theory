using Execution;

using Xunit;

namespace Parser.UnitTests;

public class ParserTest
{
    [Theory]
    [MemberData(nameof(GetExpressionTestData))]
    public void Can_parse_expressions(string code, object expected)
    {
        FakeEnvironment environment = new();
        Parser parser = new(code, environment);
        Row result = parser.EvaluateExpression();
        Assert.Equal(expected, result[0]);
    }

    public static TheoryData<string, object> GetExpressionTestData()
    {
        return new TheoryData<string, object>
        {
            { "2025", 2025m },
            { "3.14", 3.14m },
            { "\"hello\"", "hello" },
            { "'world'", "world" },
            { "\"\"", "" },
            { "ready", true },
            { "noready", false },
            { "ghost", null! },
            { "1 + 2", 3m },
            { "5 - 3", 2m },
            { "2 * 3", 6m },
            { "6 / 2", 3m },
            { "7 % 3", 1m },
            { "-5", -5m },
            { "!ready", false },
            { "ready && noready", false },
            { "ready || noready", true },
            { "5 == 5", true },
            { "5 == 3", false },
            { "5 != 3", true },
            { "5 != 5", false },
            { "3 < 5", true },
            { "5 < 5", false },
            { "6 < 5", false },
            { "3 > 5", false },
            { "5 > 5", false },
            { "6 > 5", true },
            { "3 <= 5", true },
            { "5 <= 5", true },
            { "6 <= 5", false },
            { "3 >= 5", false },
            { "5 >= 5", true },
            { "6 >= 5", true },
            { "10 - 3 - 2", 5m },
            { "12 / 3 / 2", 2m },
            { "-3 + 2", -1m },
            { "-ready && noready", false },
            { "3 + 2 * 3", 9m },
            { "1 + 2 > 2", true },
            { "1 < 2 == 5 > 4", true },
            { "5 == 5 && noready", false },
            { "ready || ready && noready", true },
            { "min(7, 3, 5)", 3m },
            { "max(2, 8, 4)", 8m },
            { "abs(-5)", 5m },
            { "round(3.6)", 4m },
            { "ceil(3.2)", 4m },
            { "floor(3.8)", 3m },
        };
    }
}