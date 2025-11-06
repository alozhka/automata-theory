using System.Globalization;

namespace Lexer;

/// <summary>
///  Лексический анализатор строки. Поддерживает подмножество строки, необходимое этому проекту.
/// </summary>
public class Lexer
{
    private static readonly Dictionary<string, TokenType> Keywords = new()
    {
        { "DAYZINT", TokenType.Dayzint },
        { "FALLOUT", TokenType.Fallout },
        { "STATUM", TokenType.Statum },
        { "STRIKE", TokenType.Strike },
        { "ARAYA", TokenType.Araya },
        { "GHOST", TokenType.Ghost },
        { "READY", TokenType.Ready },
        { "NOREADY", TokenType.Noready },
        { "IFFY", TokenType.Iffy },
        { "ELYSIAN", TokenType.Elysian },
        { "ELYSIFFY", TokenType.Elysiffy },
        { "VALORANT", TokenType.Valorant },
        { "FORZA", TokenType.Forza },
        { "BREAKOUT", TokenType.Breakout },
        { "CONTRA", TokenType.Contra },
        { "FUNKOTRON", TokenType.Funkotron },
        { "RETURNAL", TokenType.Returnal },
        { "RAID", TokenType.Raid },
        { "EXODUS", TokenType.Exodus },
        { "EXODUSLN", TokenType.Exodusln },
    };

    private readonly TextScanner _scanner;

    public Lexer(string sql)
    {
        _scanner = new TextScanner(sql);
    }

    /// <summary>
    ///  Распознаёт следующий токен.
    ///  Дополнительные правила:
    ///   1) Если ввод закончился, то возвращаем токен EndOfFile
    ///   2) Пробельные символы пропускаются
    /// </summary>
    public Token ParseToken()
    {
        SkipWhiteSpacesAndComments();

        if (_scanner.IsEnd())
        {
            return new Token(TokenType.EndOfFile);
        }

        char c = _scanner.Peek();
        if (char.IsLetter(c) || c == '_')
        {
            return ParseIdentifierOrKeyword();
        }

        if (char.IsAsciiDigit(c))
        {
            return ParseNumericLiteral();
        }

        if (c == '\'' || c == '"')
        {
            return ParseStringLiteral();
        }

        switch (c)
        {
            case '?':
                _scanner.Advance();
                return new Token(TokenType.Nullable);
            case ';':
                _scanner.Advance();
                return new Token(TokenType.Semicolon);
            case ':':
                _scanner.Advance();
                return new Token(TokenType.Colon);
            case ',':
                _scanner.Advance();
                return new Token(TokenType.Comma);
            case '+':
                _scanner.Advance();
                return new Token(TokenType.PlusSign);
            case '-':
                _scanner.Advance();
                return new Token(TokenType.MinusSign);
            case '*':
                _scanner.Advance();
                return new Token(TokenType.MultiplySign);
            case '/':
                _scanner.Advance();
                return new Token(TokenType.DivideSign);
            case '=':
                _scanner.Advance();
                if (_scanner.Peek() == '=')
                {
                    _scanner.Advance();
                    return new Token(TokenType.Equal);
                }

                return new Token(TokenType.Assign);
            case '%':
                _scanner.Advance();
                return new Token(TokenType.ModuloSign);
            case '^':
                _scanner.Advance();
                return new Token(TokenType.ExponentiationSign);
            case '!':
                _scanner.Advance();
                if (_scanner.Peek() == '=')
                {
                    _scanner.Advance();
                    return new Token(TokenType.Unequal);
                }

                return new Token(TokenType.LogicalNot);
            case '<':
                _scanner.Advance();
                if (_scanner.Peek() == '=')
                {
                    _scanner.Advance();
                    return new Token(TokenType.LessThanOrEqual);
                }

                return new Token(TokenType.LessThan);
            case '>':
                _scanner.Advance();
                if (_scanner.Peek() == '=')
                {
                    _scanner.Advance();
                    return new Token(TokenType.GreaterThanOrEqual);
                }

                return new Token(TokenType.GreaterThan);
            case '|':
                _scanner.Advance();
                if (_scanner.Peek() == '|')
                {
                    _scanner.Advance();
                    return new Token(TokenType.LogicalOr);
                }

                _scanner.Advance();
                return new Token(TokenType.Error, new TokenValue(c.ToString()));
            case '&':
                _scanner.Advance();
                if (_scanner.Peek() == '&')
                {
                    _scanner.Advance();
                    return new Token(TokenType.LogicalAnd);
                }

                _scanner.Advance();
                return new Token(TokenType.Error, new TokenValue(c.ToString()));
            case '(':
                _scanner.Advance();
                return new Token(TokenType.OpenParenthesis);
            case ')':
                _scanner.Advance();
                return new Token(TokenType.CloseParenthesis);
            case '{':
                _scanner.Advance();
                return new Token(TokenType.OpenBrace);
            case '}':
                _scanner.Advance();
                return new Token(TokenType.CloseBrace);
        }

        _scanner.Advance();
        return new Token(TokenType.Error, new TokenValue(c.ToString()));
    }

    private Token ParseIdentifierOrKeyword()
    {
        string value = _scanner.Peek().ToString();
        _scanner.Advance();

        for (char c = _scanner.Peek(); char.IsLetter(c) || c == '_' || char.IsAsciiDigit(c); c = _scanner.Peek())
        {
            value += c;
            _scanner.Advance();
        }

        if (Keywords.TryGetValue(value.ToUpper(CultureInfo.InvariantCulture), out TokenType type))
        {
            return new Token(type);
        }

        return new Token(TokenType.Identifier, new TokenValue(value));
    }

    private Token ParseNumericLiteral()
    {
        decimal value = GetDigitValue(_scanner.Peek());
        _scanner.Advance();

        for (char c = _scanner.Peek(); char.IsAsciiDigit(c); c = _scanner.Peek())
        {
            value = value * 10 + GetDigitValue(c);
            _scanner.Advance();
        }

        if (_scanner.Peek() == '.')
        {
            _scanner.Advance();
            decimal factor = 0.1m;
            for (char c = _scanner.Peek(); char.IsAsciiDigit(c); c = _scanner.Peek())
            {
                _scanner.Advance();
                value += factor * GetDigitValue(c);
                factor *= 0.1m;
            }
        }

        return new Token(TokenType.NumericLiteral, new TokenValue(value));

        int GetDigitValue(char c)
        {
            return c - '0';
        }
    }

    private Token ParseStringLiteral()
    {
        char quoteChar = _scanner.Peek();
        _scanner.Advance();

        string contents = "";
        while (_scanner.Peek() != quoteChar)
        {
            if (_scanner.IsEnd())
            {
                return new Token(TokenType.Error, new TokenValue(contents));
            }

            if (TryParseStringLiteralEscapeSequence(out char unescaped))
            {
                contents += unescaped;
            }
            else
            {
                contents += _scanner.Peek();
                _scanner.Advance();
            }
        }

        _scanner.Advance();

        return new Token(TokenType.StringLiteral, new TokenValue(contents));
    }

    private bool TryParseStringLiteralEscapeSequence(out char unescaped)
    {
        if (_scanner.Peek() == '\\')
        {
            _scanner.Advance();
            char nextChar = _scanner.Peek();

            switch (nextChar)
            {
                case '\'':
                    _scanner.Advance();
                    unescaped = '\'';
                    return true;
                case '\"':
                    _scanner.Advance();
                    unescaped = '\"';
                    return true;
                case '\\':
                    _scanner.Advance();
                    unescaped = '\\';
                    return true;
                case 'n':
                    _scanner.Advance();
                    unescaped = '\n';
                    return true;
                case 't':
                    _scanner.Advance();
                    unescaped = '\t';
                    return true;
            }
        }

        unescaped = '\0';
        return false;
    }

    private void SkipWhiteSpacesAndComments()
    {
        do
        {
            SkipWhiteSpaces();
        }
        while (TryParseMultilineComment() || TryParseSingleLineComment());
    }

    private void SkipWhiteSpaces()
    {
        while (char.IsWhiteSpace(_scanner.Peek()))
        {
            _scanner.Advance();
        }
    }

    private bool TryParseMultilineComment()
    {
        if (_scanner.Peek() == '/' && _scanner.Peek(1) == '*')
        {
            do
            {
                _scanner.Advance();
            }
            while (!(_scanner.Peek() == '*' && _scanner.Peek(1) == '/'));

            _scanner.Advance();
            _scanner.Advance();
            return true;
        }

        return false;
    }

    private bool TryParseSingleLineComment()
    {
        if (_scanner.Peek() == '-' && _scanner.Peek(1) == '-')
        {
            do
            {
                _scanner.Advance();
            }
            while (_scanner.Peek() != '\n' && _scanner.Peek() != '\r');

            return true;
        }

        return false;
    }
}