namespace SqlLexer;

public enum TokenType
{
    /// <summary>
    ///  Ключевое слово SELECT.
    /// </summary>
    Select,

    /// <summary>
    ///  Ключевое слово FROM.
    /// </summary>
    From,

    /// <summary>
    ///  Ключевое слово WHERE.
    /// </summary>
    Where,

    /// <summary>
    ///  Ключевое слово AS.
    /// </summary>
    As,

    /// <summary>
    ///  Ключевое слово AND.
    /// </summary>
    And,

    /// <summary>
    ///  Ключевое слово OR.
    /// </summary>
    Or,

    /// <summary>
    ///  Ключевое слово NOT.
    /// </summary>
    Not,

    /// <summary>
    ///  Идентификатор (имя символа).
    /// </summary>
    Identifier,

    /// <summary>
    ///  Литерал числа.
    /// </summary>
    NumericLiteral,

    /// <summary>
    ///  Литерал строки.
    /// </summary>
    StringLiteral,

    /// <summary>
    ///  Оператор сложения.
    /// </summary>
    PlusSign,

    /// <summary>
    ///  Оператор вычитания.
    /// </summary>
    MinusSign,

    /// <summary>
    ///  Оператор умножения.
    /// </summary>
    MultiplySign,

    /// <summary>
    ///  Оператор деления.
    /// </summary>
    DivideSign,

    /// <summary>
    ///  Оператор деления по модулю.
    /// </summary>
    ModuloSign,

    /// <summary>
    ///  Оператор возведения в степень.
    /// </summary>
    ExponentiationSign,

    /// <summary>
    ///  Оператор сравнения "меньше".
    /// </summary>
    LessThan,

    /// <summary>
    ///  Оператор сравнения "меньше или равно".
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    ///  Оператор сравнения "больше".
    /// </summary>
    GreaterThan,

    /// <summary>
    ///  Оператор сравнения "больше или равно".
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    ///  Открывающая круглая скобка '('.
    /// </summary>
    OpenParenthesis,

    /// <summary>
    ///  Закрывающая круглая скобка ')'.
    /// </summary>
    CloseParenthesis,

    /// <summary>
    ///  Запятая ','
    /// </summary>
    Comma,

    /// <summary>
    ///  Разделитель ';'
    /// </summary>
    Semicolon,

    /// <summary>
    ///  Конец файла.
    /// </summary>
    EndOfFile,

    /// <summary>
    ///  Недопустимая лексема.
    /// </summary>
    Error,
}