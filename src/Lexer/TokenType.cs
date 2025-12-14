namespace Lexer;

public enum TokenType
{
    /// <summary>
    /// Главная функция программы maincraft
    /// </summary>
    Maincraft,

    /// <summary>
    /// Объявление константы monument
    /// </summary>
    Monument,

    /// <summary>
    ///  �������� ����� dayzint.
    /// </summary>
    Dayzint,

    /// <summary>
    ///  �������� ����� fallout.
    /// </summary>
    Fallout,

    /// <summary>
    ///  �������� ����� strike.
    /// </summary>
    Strike,

    /// <summary>
    ///  �������� ����� raid.
    /// </summary>
    Raid,

    /// <summary>
    ///  �������� ����� exodus.
    /// </summary>
    Exodus,

    /// <summary>
    ///  �������� ����� iffy.
    /// </summary>
    Iffy,

    /// <summary>
    ///  �������� ����� elysian.
    /// </summary>
    Elysian,

    /// <summary>
    ///  �������� ����� valorant.
    /// </summary>
    Valorant,

    /// <summary>
    ///  �������� ����� forza.
    /// </summary>
    Forza,

    /// <summary>
    ///  �������� ����� breakout.
    /// </summary>
    Breakout,

    /// <summary>
    ///  �������� ����� contra.
    /// </summary>
    Contra,

    /// <summary>
    ///  �������� ����� funkotron.
    /// </summary>
    Funkotron,

    /// <summary>
    ///  �������� ����� returnal.
    /// </summary>
    Returnal,

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
    ///  �������� ������������.
    /// </summary>
    Assign,

    /// <summary>
    ///  �������� ���������.
    /// </summary>
    Equal,

    /// <summary>
    ///  �������� �����������.
    /// </summary>
    Unequal,

    /// <summary>
    ///  �������� ���������.
    /// </summary>
    MinusSign,

    /// <summary>
    ///  ���������� �.
    /// </summary>
    LogicalAnd,

    /// <summary>
    ///  ���������� ���.
    /// </summary>
    LogicalOr,

    /// <summary>
    ///  ���������� ��.
    /// </summary>
    LogicalNot,

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
    ///  ����������� �������� ������ '{'.
    /// </summary>
    OpenBrace,

    /// <summary>
    ///  ����������� �������� ������ '}'.
    /// </summary>
    CloseBrace,

    /// <summary>
    ///  ������� ','
    /// </summary>
    Comma,

    /// <summary>
    ///  ����������� ';'
    /// </summary>
    Semicolon,

    /// <summary>
    ///  ����������� ':'
    /// </summary>
    Colon,

    /// <summary>
    ///  ����� �����.
    /// </summary>
    EndOfFile,

    /// <summary>
    ///  ������������ �������.
    /// </summary>
    Error,
}