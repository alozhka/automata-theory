using System.Globalization;

using Execution;

using Reqnroll;

using Runtime;

using ValueType = Runtime.ValueType;

namespace Interpreter.Specs.Steps;

[Binding]
public class InterpreterSteps
{
    private const int Precision = 5;
    private static readonly double Tolerance = Math.Pow(0.1, Precision);

    private Context? _context;
    private FakeEnvironment? _environment;
    private string? _currentProgram;

    [Given(@"я запустил программу:")]
    public void GivenЯЗапустилПрограмму(string program)
    {
        _context = new Context();
        _environment = new FakeEnvironment();
        _currentProgram = program;
    }

    [Given(@"я установил входные данные:")]
    public void GivenЯУстановилВходныеДанные(Table table)
    {
        if (_environment == null)
        {
            throw new InvalidOperationException("Сначала нужно запустить программу");
        }

        List<Value> inputs = table.Rows.Select(r =>
        {
            string value = r["Число"];

            if (int.TryParse(value, out int intVal))
            {
                return new Value(intVal);
            }

            if (double.TryParse(value, CultureInfo.InvariantCulture, out double doubleVal))
            {
                return new Value(doubleVal);
            }

            return new Value(value);
        }).ToList();

        _environment = new FakeEnvironment(inputs.ToArray());
    }

    [When(@"я выполняю программу")]
    public void WhenЯВыполняюПрограмму()
    {
        if (_context == null || _environment == null || _currentProgram == null)
        {
            throw new InvalidOperationException("Сначала нужно запустить программу и установить входные данные");
        }

        Parser.Parser parser = new(_context, _currentProgram, _environment);
        parser.ParseProgram();
    }

    [Then(@"я получаю результаты:")]
    public void ThenЯПолучаюРезультаты(Table table)
    {
        if (_environment == null)
        {
            throw new InvalidOperationException("Программа не была выполнена");
        }

        IReadOnlyList<Value> actual = _environment.Results;

        if (table.Rows.Count != actual.Count)
        {
            Assert.Fail(
                $"Actual results count does not match expected. Expected: {table.Rows.Count}, Actual: {actual.Count}."
            );
        }

        for (int i = 0; i < table.Rows.Count; i++)
        {
            string expectedStr = table.Rows[i]["Результат"];
            Value actualValue = actual[i];

            switch (actualValue.GetValueType())
            {
                case ValueType.Int:
                    if (int.TryParse(expectedStr, out int expectedInt))
                    {
                        if (expectedInt != actualValue.AsInt())
                        {
                            Assert.Fail($"Expected does not match actual at index {i}: {expectedInt} != {actualValue.AsInt()}");
                        }
                    }
                    else
                    {
                        Assert.Fail($"Cannot parse expected value '{expectedStr}' as int at index {i}");
                    }

                    break;

                case ValueType.Double:
                    if (double.TryParse(expectedStr, CultureInfo.InvariantCulture, out double expectedDouble))
                    {
                        if (Math.Abs(expectedDouble - actualValue.AsDouble()) >= Tolerance)
                        {
                            Assert.Fail($"Expected does not match actual at index {i}: {expectedDouble} != {actualValue.AsDouble()}");
                        }
                    }
                    else
                    {
                        Assert.Fail($"Cannot parse expected value '{expectedStr}' as double at index {i}");
                    }

                    break;

                case ValueType.String:
                    if (expectedStr != actualValue.AsString())
                    {
                        Assert.Fail($"Expected does not match actual at index {i}: '{expectedStr}' != '{actualValue.AsString()}'");
                    }

                    break;

                default:
                    Assert.Fail($"Unexpected type {actualValue.GetValueType()} at index {i}");
                    break;
            }
        }
    }
}