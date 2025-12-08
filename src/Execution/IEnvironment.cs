using Runtime;

namespace Execution;

/// <summary>
/// ������������ ��������� ��� ���������� ���������.
/// ������ ����� ��� ������� �����/������.
/// </summary>
public interface IEnvironment
{
    /// <summary>
    /// ���������� ����� ���������� ���������� ��������� ���������� ���������.
    /// </summary>
    public void AddResult(Value result);

    public Value ReadInput();
}