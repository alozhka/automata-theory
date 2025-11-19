using Parser;

using Xunit;

namespace Parser.UnitTests;

public class ParseTopLevelStatementsTest
{
    [Fact]
    public void Can_parse_input_output()
    {
        FakeEnvironment fakeEnvironment = new([42]);

        string code = @"
        maincraft()
        {
            dayzint x;
            raid(x);
            exodus(x);
        }";

        Parser parser = new(code, fakeEnvironment);

        parser.ParseProgram();

        Assert.Equal(42.0, fakeEnvironment.Results[0]);
    }

    [Theory]
    [MemberData(nameof(GetExpressionTopLevelTestData))]
    public void Can_parse_top_level(string code, object[] expected)
    {
        FakeEnvironment environment = new();
        Parser parser = new(code, environment);
        parser.ParseProgram();

        Assert.Equal(expected.Length, environment.Results.Count);
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(Convert.ToDouble(expected[i]), environment.Results[i]);
        }
    }

    public static TheoryData<string, object[]> GetExpressionTopLevelTestData()
    {
        return new TheoryData<string, object[]>
        {
            {
                @"
                maincraft()
                {
                    exodus(7);
                }",
                [7]
            },
            {
                @"
                maincraft()
                {
                    dayzint x = 5;
                    exodus(x);
                }",
                [5]
            },
            {
                @"
                dayzint x = 10;
                maincraft()
                {
                    exodus(x);
                }",
                [10]
            },
            {
                @"
                maincraft()
                {
                    exodus(min(10, 3, 15));
                }",
                [3]
            },
            {
                @"
                maincraft()
                {
                    dayzint x = 1;
                    exodus(min(x, 3, 15));
                }",
                [1]
            },
            {
                @"
                dayzint x;
                maincraft()
                {
                    exodus(x);
                }",
                [0]
            },
            {
                @"
                monument dayzint x = 66.6;
                maincraft()
                {
                    exodus(x);
                }",
                [66.6]
            },
        };
    }
}