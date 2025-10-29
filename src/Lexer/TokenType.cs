namespace SqlLexer;

public enum TokenType
{
    /// <summary>
    ///  �������� ����� SELECT.
    /// </summary>
    Select,

    /// <summary>
    ///  �������� ����� FROM.
    /// </summary>
    From,

    /// <summary>
    ///  �������� ����� WHERE.
    /// </summary>
    Where,

    /// <summary>
    ///  �������� ����� AS.
    /// </summary>
    As,

    /// <summary>
    ///  �������� ����� AND.
    /// </summary>
    And,

    /// <summary>
    ///  �������� ����� OR.
    /// </summary>
    Or,

    /// <summary>
    ///  �������� ����� NOT.
    /// </summary>
    Not,

    /// <summary>
    ///  ������������� (��� �������).
    /// </summary>
    Identifier,

    /// <summary>
    ///  ������� �����.
    /// </summary>
    NumericLiteral,

    /// <summary>
    ///  ������� ������.
    /// </summary>
    StringLiteral,

    /// <summary>
    ///  �������� ��������.
    /// </summary>
    PlusSign,

    /// <summary>
    ///  �������� ���������.
    /// </summary>
    MinusSign,

    /// <summary>
    ///  �������� ���������.
    /// </summary>
    MultiplySign,

    /// <summary>
    ///  �������� �������.
    /// </summary>
    DivideSign,

    /// <summary>
    ///  �������� ������� �� ������.
    /// </summary>
    ModuloSign,

    /// <summary>
    ///  �������� ���������� � �������.
    /// </summary>
    ExponentiationSign,

    /// <summary>
    ///  �������� ��������� "������".
    /// </summary>
    LessThan,

    /// <summary>
    ///  �������� ��������� "������ ��� �����".
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    ///  �������� ��������� "������".
    /// </summary>
    GreaterThan,

    /// <summary>
    ///  �������� ��������� "������ ��� �����".
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    ///  ����������� ������� ������ '('.
    /// </summary>
    OpenParenthesis,

    /// <summary>
    ///  ����������� ������� ������ ')'.
    /// </summary>
    CloseParenthesis,

    /// <summary>
    ///  ������� ','
    /// </summary>
    Comma,

    /// <summary>
    ///  ����������� ';'
    /// </summary>
    Semicolon,

    /// <summary>
    ///  ����� �����.
    /// </summary>
    EndOfFile,

    /// <summary>
    ///  ������������ �������.
    /// </summary>
    Error,
}