using System;
using System.Collections.Generic;

using Execution;

namespace Parser;

public class ConsoleEnvironment : IEnvironment
{
    /// <summary>
    /// Чтение ввода из консоли (raid)
    /// </summary>
    public object ReadInput()
    {
        return Console.ReadLine() ?? "";
    }

    /// <summary>
    /// Вывод результата в консоль (exodus/exodusln)
    /// </summary>
    public void AddResult(double result)
    {
        Console.WriteLine(result);
    }
}