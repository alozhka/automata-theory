using Ast;

using Execution;

using Semantics;

namespace Interpreter;

/// <summary>
/// ������������� ����� Kaleidoscope
/// </summary>
public class Interpreter
{
    private readonly Builtins _builtins;
    private readonly IEnvironment _environment;

    public Interpreter(IEnvironment environment)
    {
        _builtins = new Builtins();
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
        Parser.Parser parser = new(sourceCode);
        List<AstNode> nodes = parser.ParseProgram();

        SemanticsChecker checker = new(_builtins.Functions, _builtins.Types);
        checker.Check(nodes);

        AstEvaluator evaluator = new(context, _environment);
        foreach (AstNode node in nodes)
        {
            evaluator.Evaluate(node);
        }
    }
}