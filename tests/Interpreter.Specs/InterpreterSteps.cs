using Execution;

namespace Interpreter.Specs;

public class InterpreterSteps
{
    /// TODO: Gherkin сделать к 5 лабораторной
    [Fact]
    public void Should_execute_sum_number_program()
    {
        string program = """
            maincraft()
            {
                dayzint a;
                dayzint b;

                raid(a);
                raid(b);

                exodusln(a + b);
            }
            """;

        FakeEnvironment fakeEnvironment = new([10, 20]);
        Interpreter interpreter = new(fakeEnvironment);
        interpreter.Execute(program);

        Assert.Equal(30, fakeEnvironment.Results[0]);
    }

    [Fact]
    public void Should_execute_circle_square_program()
    {
        string program = """
            monument dayzint PI = 3.14;

            maincraft()
            {
                dayzint radius;
                raid(radius);

                exodusln(PI * radius * radius);
            }
            """;

        FakeEnvironment fakeEnvironment = new([10]);
        Interpreter interpreter = new(fakeEnvironment);
        interpreter.Execute(program);

        Assert.Equal(314, fakeEnvironment.Results[0]);
    }

    [Fact]
    public void Should_execute_miles_to_km_program()
    {
        string program = """
            monument dayzint MILES_PER_KM = 1.60934;

            maincraft()
            {
                dayzint miles;
                raid(miles);

                exodusln(miles * MILES_PER_KM);
            }
            """;

        FakeEnvironment fakeEnvironment = new([10]);
        Interpreter interpreter = new(fakeEnvironment);
        interpreter.Execute(program);

        Assert.Equal(16.0934, fakeEnvironment.Results[0]);
    }
}