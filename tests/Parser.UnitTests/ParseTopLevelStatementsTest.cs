using Execution;

using Runtime;

using Xunit;

namespace Parser.UnitTests;

public class ParseTopLevelStatementsTest
{
    [Fact]
    public void Can_parse_input_output()
    {
        FakeEnvironment fakeEnvironment = new([new Value(42)]);
        Context context = new();

        string code = @"
        maincraft()
        {
            dayzint x;
            raid(x);
            exodus(x);
        }";

        Parser parser = new(context, code, fakeEnvironment);

        parser.ParseProgram();

        Assert.Equal(new Value(42), fakeEnvironment.Results[0]);
    }

    [Theory]
    [MemberData(nameof(GetExpressionTopLevelTestData))]
    public void Can_parse_top_level(string code, object[] expected)
    {
        FakeEnvironment environment = new();
        Context context = new();
        Parser parser = new(context, code, environment);
        parser.ParseProgram();

        Assert.Equal(expected.Length, environment.Results.Count);
        for (int i = 0; i < expected.Length; i++)
        {
            Value result = environment.Results[i];
            double actualValue = result.GetValueType() switch
            {
                Runtime.ValueType.Int => result.AsInt(),
                Runtime.ValueType.Double => result.AsDouble(),
                _ => throw new InvalidOperationException($"Unexpected value type: {result.GetValueType()}"),
            };
            double expectedValue = Convert.ToDouble(expected[i]);
            if (expected[i] is double or float or decimal)
            {
                Assert.Equal(expectedValue, actualValue, 6);
            }
            else
            {
                Assert.Equal(expectedValue, actualValue);
            }
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
                monument fallout x = 66.6;
                maincraft()
                {
                    exodus(x);
                }",
                [66.6]
            },
            {
                @"
                maincraft()
                {
                    forza (dayzint i = 1; i <= 5; i = i + 1) {
                        exodus(i);
                    }
                }",
                [1, 2, 3, 4, 5]
            },
            {
                @"
                maincraft()
                {
                    dayzint x = 1;
                    valorant (x < 5) {
                        exodus(x);
                        x = x + 1;
                    }
                }",
                [1, 2, 3, 4]
            },
            {
                @"
                maincraft()
                {
                    dayzint x = 1;
                    iffy (x < 2) {
                        exodus(10);
                    }
                }",
                [10]
            },
            {
                @"
                maincraft()
                {
                    dayzint x = 5;
                    iffy (x < 2) {
                        exodus(10);
                    }
                }",
                []
            },
            {
                @"
                maincraft()
                {
                    dayzint x = 5;
                    iffy (x < 2) {
                        exodus(10);
                    } elysian {
                        exodus(5);
                    }
                }",
                [5]
            },
            {
                @"
                maincraft()
                {
                    forza (dayzint i = 1; i <= 5; i = i + 1) {
                        iffy (i == 3) {
                            breakout;
                        }
                        exodus(i);
                    }
                }",
                [1, 2]
            },
            {
                @"
                maincraft()
                {
                    forza (dayzint i = 1; i <= 5; i = i + 1) {
                        iffy (i == 3) {
                            contra;
                        }
                        exodus(i);
                    }
                }",
                [1, 2, 4, 5]
            },
            {
                @"
                maincraft()
                {
                    dayzint x = 1;
                    valorant (x < 5) {
                        iffy (x == 3) {
                            breakout;
                        }
                        exodus(x);
                        x = x + 1;
                    }
                }",
                [1, 2]
            },
            {
                @"
                maincraft()
                {
                    dayzint x = 1;
                    valorant (x < 5) {
                        iffy (x == 3) {
                            x = x + 1;
                            contra;
                        }
                        exodus(x);
                        x = x + 1;
                    }
                }",
                [1, 2, 4]
            },
        };
    }
}