using System;

using Execution;

using Parser;

namespace Interpreter;

/// <summary>
/// Интерпретатор языка Kaleidoscope
/// </summary>
public class Interpreter
{
    private readonly IEnvironment _environment;

    public Interpreter(IEnvironment environment)
    {
        _environment = environment;
    }

    /// <summary>
    /// Выполняет программу на языке Kaleidoscope
    /// </summary>
    /// <param name="sourceCode">Исходный код программы</param>
    public void Execute(string sourceCode)
    {
        if (string.IsNullOrEmpty(sourceCode))
        {
            throw new ArgumentException("Source code cannot be null or empty", nameof(sourceCode));
        }

        // Создаем парсер и выполняем программу
        Parser.Parser parser = new(sourceCode, _environment);
        parser.ParseProgram();
    }
}