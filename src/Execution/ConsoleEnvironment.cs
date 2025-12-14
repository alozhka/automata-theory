using Runtime;

namespace Execution;

public class ConsoleEnvironment : IEnvironment
{
    /// <summary>
    /// ������ ����� �� ������� (raid)
    /// </summary>
    public Value ReadInput()
    {
        while (true)
        {
            string input = Console.ReadLine()!;

            if (int.TryParse(input, out int intResult))
            {
                return new Value(intResult);
            }

            return new Value(input);
        }
    }

    /// <summary>
    /// ����� ���������� � ������� (exodus)
    /// </summary>
    public void AddResult(Value result)
    {
        Console.Write(result.ToString());
    }
}