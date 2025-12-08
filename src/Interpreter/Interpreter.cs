using Execution;

namespace Interpreter;

/// <summary>
/// ������������� ����� Kaleidoscope
/// </summary>
public class Interpreter
{
    private readonly IEnvironment _environment;

    public Interpreter(IEnvironment environment)
    {
        _environment = environment;
    }

    /// <summary>
    /// ��������� ��������� �� ����� Kaleidoscope
    /// </summary>
    /// <param name="sourceCode">�������� ��� ���������</param>
    public void Execute(string sourceCode)
    {
        if (string.IsNullOrEmpty(sourceCode))
        {
            throw new ArgumentException("Source code cannot be null or empty", nameof(sourceCode));
        }

        Context context = new();
        Parser.Parser parser = new(context, sourceCode, _environment);
        parser.ParseProgram();
    }
}