namespace Execution;

public class ConsoleEnvironment : IEnvironment
{
    /// <summary>
    /// ������ ����� �� ������� (raid)
    /// </summary>
    public double ReadInput()
    {
        while (true)
        {
            string input = Console.ReadLine()!;
            if (double.TryParse(input, out double result))
            {
                return result;
            }

            Console.WriteLine("Ошибка! Введите корректное число:");
        }
    }

    /// <summary>
    /// ����� ���������� � ������� (exodus/exodusln)
    /// </summary>
    public void AddResult(double result)
    {
        Console.WriteLine(result);
    }
}