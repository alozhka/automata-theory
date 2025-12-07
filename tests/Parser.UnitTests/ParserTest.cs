using Execution;

using Runtime;

using Xunit;

namespace Parser.UnitTests;

public class ParserTest
{
    [Theory]
    [MemberData(nameof(GetExpressionTestData))]
    public void Can_parse_expressions(string code, object expected)
    {
        FakeEnvironment environment = new();
        Context context = new();
        Parser parser = new(context, code, environment);
        Row result = parser.EvaluateExpression();

        Value resultValue = result.GetValue(0);

        if (expected is string expectedString)
        {
            Assert.Equal(Runtime.ValueType.String, resultValue.GetValueType());
            Assert.Equal(expectedString, resultValue.AsString());
        }
        else
        {
            double actualValue = resultValue.GetValueType() switch
            {
                Runtime.ValueType.Int => resultValue.AsInt(),
                Runtime.ValueType.Double => resultValue.AsDouble(),
                Runtime.ValueType.String => double.Parse(resultValue.AsString()),
                _ => throw new InvalidOperationException($"Unexpected value type: {resultValue.GetValueType()}"),
            };

            double expectedValue = Convert.ToDouble(expected);

            if (expected is int)
            {
                Assert.Equal((int)expectedValue, (int)actualValue);
            }
            else
            {
                Assert.Equal(expectedValue, actualValue, 6);
            }
        }
    }

    public static TheoryData<string, object> GetExpressionTestData()
    {
        return new TheoryData<string, object>
        {
            { "2025", 2025 },
            { "3.14", 3.14 },
            { "ready", 1 },
            { "noready", 0 },
            { "1 + 2", 3m },
            { "5 - 3", 2m },
            { "2 * 3", 6m },
            { "6 / 2", 3m },
            { "7 % 3", 1m },
            { "-5", -5m },
            { "!ready", 0 },
            { "ready && noready", 0 },
            { "ready || noready", 1 },
            { "5 == 5", 1 },
            { "5 == 3", 0 },
            { "5 != 3", 1 },
            { "5 != 5", 0 },
            { "3 < 5", 1 },
            { "5 < 5", 0 },
            { "6 < 5", 0 },
            { "3 > 5", 0 },
            { "5 > 5", 0 },
            { "6 > 5", 1 },
            { "3 <= 5", 1 },
            { "5 <= 5", 1 },
            { "6 <= 5", 0 },
            { "3 >= 5", 0 },
            { "5 >= 5", 1 },
            { "6 >= 5", 1 },
            { "10 - 3 - 2", 5m },
            { "12 / 3 / 2", 2m },
            { "-3 + 2", -1m },
            { "-ready && noready", 0 },
            { "3 + 2 * 3", 9m },
            { "1 + 2 > 2", 1 },
            { "1 < 2 == 5 > 4", 1 },
            { "5 == 5 && noready", 0 },
            { "ready || ready && noready", 1 },
            { "min(7, 3, 5)", 3m },
            { "max(2, 8, 4)", 8m },
            { "abs(-5)", 5m },
            { "round(3.6)", 4m },
            { "ceil(3.2)", 4m },
            { "floor(3.8)", 3m },
            { "length('test')", 4 },
            { "length('')", 0 },
            { "str_at('test', 3)", "t" },
        };
    }
}