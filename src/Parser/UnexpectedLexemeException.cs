using SqlLexer;
using System;

namespace Parser;

public class UnexpectedLexemeException : Exception
{
	public UnexpectedLexemeException(TokenType expected, Token actual)
		: base($"Expected {expected}, but got {actual.Type}")
	{
	}

	public UnexpectedLexemeException(string expected, Token actual)
		: base($"Expected {expected}, but got {actual.Type}")
	{
	}
}