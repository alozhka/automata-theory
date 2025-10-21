namespace SqlLexer;

/// <summary>
///  —канирует текст SQL-запроса, предоставл€€ три операции: Peek(N), Advance() и IsEnd().
/// </summary>
public class TextScanner(string sql)
{
	private readonly string _sql = sql;
	private int _position;

	/// <summary>
	///  „итает на N символов вперЄд текущей позиции (по умолчанию N=0).
	/// </summary>
	public char Peek(int n = 0)
	{
		int position = _position + n;
		return position >= _sql.Length ? '\0' : _sql[position];
	}

	/// <summary>
	///  —двигает текущую позицию на один символ.
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