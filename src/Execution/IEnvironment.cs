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
    public void AddResult(double result);

    public double ReadInput();
}