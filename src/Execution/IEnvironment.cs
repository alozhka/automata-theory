using Runtime;

namespace Execution;

/// <summary>
/// Представляет окружение для выполнения программы.
/// Прежде всего это функции ввода/вывода.
/// </summary>
public interface IEnvironment
{
    /// <summary>
    /// Вызывается после вычисления результата очередной инструкции программы.
    /// </summary>
    public void AddResult(Value result);

    /// <summary>
    /// Считывает значение из входных данных.
    /// </summary>
    public Value ReadInput();
}