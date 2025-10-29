using System.Text;

namespace SqlLexer;

public class Token(
    TokenType type,
    TokenValue? value = null
)
{
    public TokenType Type { get; } = type;

    public TokenValue? Value { get; } = value;

    /// <summary>
    ///  ���������� ������ �� ���� � ��������.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is Token other)
        {
            return Type == other.Type && Equals(Value, other.Value);
        }

        return false;
    }

    /// <summary>
    ///  ���������� ��� �� ������� ������.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine((int)Type, Value);
    }

    /// <summary>
    /// ����������� ����� � ����� "Type (Value)".
    /// </summary>
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append(Type.ToString());
        if (Value != null)
        {
            sb.Append($" ({Value})");
        }

        return sb.ToString();
    }
}