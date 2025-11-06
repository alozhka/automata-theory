namespace Lexer;

/// <summary>
///  ��������� ����� SQL-�������, ������������ ��� ��������: Peek(N), Advance() � IsEnd().
/// </summary>
public class TextScanner(string sql)
{
    private readonly string _sql = sql;
    private int _position;

    /// <summary>
    ///  ������ �� N �������� ����� ������� ������� (�� ��������� N=0).
    /// </summary>
    public char Peek(int n = 0)
    {
        int position = _position + n;
        return position >= _sql.Length ? '\0' : _sql[position];
    }

    /// <summary>
    ///  �������� ������� ������� �� ���� ������.
    /// </summary>
    public void Advance()
    {
        _position++;
    }

    public bool IsEnd()
    {
        return _position >= _sql.Length;
    }
}