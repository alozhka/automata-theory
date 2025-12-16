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
    /// Ключевое слово dayzint.
    /// </summary>
    Dayzint,

    /// <summary>
    /// Ключевое слово fallout.
    /// </summary>
    Fallout,

    /// <summary>
    /// Ключевое слово strike.
    /// </summary>
    Strike,

    /// <summary>
    /// Ключевое слово raid.
    /// </summary>
    Raid,

    /// <summary>
    /// Ключевое слово exodus.
    /// </summary>
    Exodus,

    /// <summary>
    /// Ключевое слово iffy.
    /// </summary>
    Iffy,

    /// <summary>
    /// Ключевое слово elysian.
    /// </summary>
    Elysian,

    /// <summary>
    /// Ключевое слово valorant.
    /// </summary>
    Valorant,

    /// <summary>
    /// Ключевое слово forza.
    /// </summary>
    Forza,

    /// <summary>
    /// Ключевое слово breakout.
    /// </summary>
    Breakout,

    /// <summary>
    ///  Ключевое слово contra.
    /// </summary>
    Contra,

    /// <summary>
    /// Ключевое слово funkotron.
    /// </summary>
    Funkotron,

    /// <summary>
    /// Ключевое слово returnal.
    /// </summary>
    Returnal,

    /// <summary>
    /// Идентификатор (имя символа).
    /// </summary>
    Identifier,

    /// <summary>
    /// Литерал числа.
    /// </summary>
    NumericLiteral,

    /// <summary>
    /// Литерал строки.
    /// </summary>
    StringLiteral,

    /// <summary>
    /// Оператор сложения.
    /// </summary>
    PlusSign,

    /// <summary>
    /// Оператор присваивания.
    /// </summary>
    Assign,

    /// <summary>
    /// Оператор равенства.
    /// </summary>
    Equal,

    /// <summary>
    /// Оператор нерваенства.
    /// </summary>
    Unequal,

    /// <summary>
    /// Оператор вычитания.
    /// </summary>
    MinusSign,

    /// <summary>
    /// Логическое И.
    /// </summary>
    LogicalAnd,

    /// <summary>
    /// Логическое ИЛИ.
    /// </summary>
    LogicalOr,

    /// <summary>
    /// Логическое НЕ.
    /// </summary>
    LogicalNot,

    /// <summary>
    /// Оператор умножения.
    /// </summary>
    MultiplySign,

    /// <summary>
    /// Оператор деления.
    /// </summary>
    DivideSign,

    /// <summary>
    /// Оператор деления по модулю.
    /// </summary>
    ModuloSign,

    /// <summary>
    /// Оператор сравнения "меньше".
    /// </summary>
    LessThan,

    /// <summary>
    /// Оператор сравнения "меньше или равно".
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// Оператор сравнения "больше".
    /// </summary>
    GreaterThan,

    /// <summary>
    /// Оператор сравнения "больше или равно".
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// Открывающая круглая скобка '('.
    /// </summary>
    OpenParenthesis,

    /// <summary>
    /// Закрывающая круглая скобка ')'.
    /// </summary>
    CloseParenthesis,

    /// <summary>
    /// Открывающая фигурная скобка '{'.
    /// </summary>
    OpenBrace,

    /// <summary>
    /// Закрывающая фигурная скобка '}'.
    /// </summary>
    CloseBrace,

    /// <summary>
    /// Запятая ','
    /// </summary>
    Comma,

    /// <summary>
    /// Разделитель ';'
    /// </summary>
    Semicolon,

    /// <summary>
    /// Разделитель ':'
    /// </summary>
    Colon,

    /// <summary>
    /// Конец файла.
    /// </summary>
    EndOfFile,

    /// <summary>
    /// Недопустимая лексема.
    /// </summary>
    Error,
}