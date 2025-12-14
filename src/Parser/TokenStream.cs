using Lexer;

namespace Parser;

public class TokenStream
{
    private readonly Lexer.Lexer _lexer;
    private Token _nextToken;

    public TokenStream(string code)
    {
        _lexer = new Lexer.Lexer(code);
        _nextToken = _lexer.ParseToken();
    }

    public Token Peek()
    {
        return _nextToken;
    }

    public void Advance()
    {
        _nextToken = _lexer.ParseToken();
    }
}