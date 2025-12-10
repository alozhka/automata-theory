using Ast;

using Execution;

using Runtime;

using Semantics;
using Semantics.Exceptions;

using Xunit;

namespace Parser.UnitTests;

public class ParseTopLevelStatementsTest
{
    [Fact]
    public void Can_parse_input_output()
    {
        Builtins builtins = new();
        FakeEnvironment fakeEnvironment = new([new Value(42)]);
        Context context = new();

        string code = @"
        maincraft()
        {
            dayzint x;
            raid(x);
            exodus(x);
        }";

        Parser parser = new(code);

        List<AstNode> nodes = parser.ParseProgram();
        SemanticsChecker checker = new(builtins.Functions, builtins.Types);
        checker.Check(nodes);
        AstEvaluator evaluator = new(context, fakeEnvironment);
        foreach (AstNode node in nodes)
        {
            evaluator.Evaluate(node);
        }

        Assert.Equal(new Value(42), fakeEnvironment.Results[0]);
    }

    [Theory]
    [MemberData(nameof(GetExpressionTopLevelTestData))]
    public void Can_parse_top_level(string code, object[] expected)
    {
        Builtins builtins = new();
        FakeEnvironment environment = new();
        Context context = new();
        Parser parser = new(code);
        List<AstNode> nodes = parser.ParseProgram();
        SemanticsChecker checker = new(builtins.Functions, builtins.Types);
        checker.Check(nodes);
        AstEvaluator evaluator = new(context, environment);
        foreach (AstNode node in nodes)
        {
            evaluator.Evaluate(node);
        }

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

    [Theory]
    [MemberData(nameof(GetSemanticViolationsData))]
    public void Rejects_code_with_semantic_violations(string code, Type expectedExceptionType)
    {
        Builtins builtins = new();
        Parser parser = new(code);
        List<AstNode> nodes = parser.ParseProgram();
        SemanticsChecker checker = new(builtins.Functions, builtins.Types);

        Assert.Throws(expectedExceptionType, () => checker.Check(nodes));
    }

    public static TheoryData<string, Type> GetSemanticViolationsData()
    {
        return new TheoryData<string, Type>
        {
            {
                """
                maincraft()
                {
                    fallout radius;
                    fallout radius;
                }
                """,
                typeof(DuplicateSymbolException)
            },
            {
                """
                maincraft()
                {
                    fallout radius = 5.0;
                    monument fallout radius = 3.0;
                }
                """,
                typeof(DuplicateSymbolException)
            },
            {
                """
                maincraft()
                {
                    monument fallout radius = 3.0;
                    fallout radius = 5.0;
                }
                """,
                typeof(DuplicateSymbolException)
            },
            {
                """
                maincraft()
                {
                    raid(n);
                }
                """,
                typeof(UnknownSymbolException)
            },
            {
                """
                maincraft()
                {
                    exodus(n);
                }
                """,
                typeof(UnknownSymbolException)
            },
            {
                """
                maincraft()
                {
                    iffy(n < 0) {
                        exodus(1);
                    }
                }
                """,
                typeof(UnknownSymbolException)
            },
            {
                """
                maincraft()
                {
                    iffy(n < 0) {
                        exodus(1);
                    } elysian {
                        exodus(0);
                    }
                }
                """,
                typeof(UnknownSymbolException)
            },
            {
                """
                maincraft()
                {
                    returnal n;
                }
                """,
                typeof(UnknownSymbolException)
            },
            {
                """
                maincraft()
                {
                    monument fallout radius = 3.0;
                    radius = 5.0;
                }
                """,
                typeof(InvalidAssignmentException)
            },
            {
                """
                funkotron factorial(fallout n)
                {
                }
                
                maincraft()
                {
                    factorial();
                }
                """,
                typeof(InvalidFunctionCallException)
            },
            {
                """
                funkotron factorial()
                {
                    breakout;
                }

                maincraft()
                {
                    factorial();
                }
                """,
                typeof(InvalidExpressionException)
            },
            {
                """
                funkotron factorial()
                {
                    contra;
                }

                maincraft()
                {
                    factorial();
                }
                """,
                typeof(InvalidExpressionException)
            },
            {
                """
                maincraft()
                {
                    exodus(1 + 'hello');
                }
                """,
                typeof(TypeErrorException)
            },
            {
                """
                maincraft()
                {
                    exodus('a' - 'b');
                }
                """,
                typeof(TypeErrorException)
            },
            {
                """
                maincraft()
                {
                    exodus('text' * 2);
                }
                """,
                typeof(TypeErrorException)
            },
            {
                """
                maincraft()
                {
                    exodus('text' / 2);
                }
                """,
                typeof(TypeErrorException)
            },
            {
                """
                maincraft()
                {
                    exodus('a' < 1);
                }
                """,
                typeof(TypeErrorException)
            },
            {
                """
                maincraft()
                {
                    exodus('a' > 1);
                }
                """,
                typeof(TypeErrorException)
            },
            {
                """
                maincraft()
                {
                    exodus(3.14 == 'pi');
                }
                """,
                typeof(TypeErrorException)
            },
            {
                """
                maincraft()
                {
                    exodus(-'hello');
                }
                """,
                typeof(TypeErrorException)
            },
            {
                """
                maincraft()
                {
                    dayzint x = 'text';
                }
                """,
                typeof(TypeErrorException)
            },
            {
                """
                maincraft()
                {
                    strike s = 42;
                }
                """,
                typeof(TypeErrorException)
            },
            {
                """
                maincraft()
                {
                    fallout f = 'text';
                }
                """,
                typeof(TypeErrorException)
            },
            {
                """
                maincraft()
                {
                    exodus(floor('text'));
                }
                """,
                typeof(TypeErrorException)
            },
            {
                """
                maincraft()
                {
                    exodus(str_at(123, 0));
                }
                """,
                typeof(TypeErrorException)
            },
            {
                """
                maincraft()
                {
                    dayzint x = 5;
                    x = 'text';
                }
                """,
                typeof(TypeErrorException)
            },
            {
                """
                funkotron factorial(): dayzint
                {
                    strike s;
                    returnal s;
                }

                maincraft()
                {
                    factorial();
                }
                """,
                typeof(TypeErrorException)
            },
            {
                """
                funkotron factorial(): dayzint
                {
                    strike s;
                    iffy (1 < 5) {
                        returnal s;
                    }
                }

                maincraft()
                {
                    factorial();
                }
                """,
                typeof(TypeErrorException)
            },
            {
                """
                funkotron factorial(): dayzint
                {
                }

                maincraft()
                {
                    factorial();
                }
                """,
                typeof(TypeErrorException)
            },
            {
                """
                funkotron factorial(): dayzint
                {
                    iffy (5 < 0)
                    {
                        returnal 0;
                    }
                }

                maincraft()
                {
                    factorial();
                }
                """,
                typeof(TypeErrorException)
            },
        };
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
                    exodus(min(10, 3));
                }",
                [3]
            },
            {
                @"
                maincraft()
                {
                    dayzint x = 1;
                    exodus(min(x, 3));
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