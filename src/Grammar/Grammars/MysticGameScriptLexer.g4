lexer grammar MysticGameScriptLexer;

// ============================================================================
// KEYWORDS (Ключевые слова)
// ============================================================================

// Data types (Типы данных)
DAYZINT     : 'dayzint';
FALLOUT     : 'fallout';
STRIKE      : 'strike';
STATUM      : 'statum';
ARAYA       : 'araya';

// Boolean constants (Логические константы)
READY       : 'ready';
NOREADY     : 'noready';
GHOST       : 'ghost';

// Control flow (Управляющие конструкции)
IFFY        : 'iffy';
ELYSIFFY    : 'elysiffy';
ELYSIAN     : 'elysian';
VALORANT    : 'valorant';
FORZA       : 'forza';
BREAKOUT    : 'breakout';
CONTRA      : 'contra';

// Functions and declarations (Функции и объявления)
FUNKOTRON   : 'funkotron';
RETURNAL    : 'returnal';
MONUMENT    : 'monument';

// Built-in functions (Встроенные функции)
MIN         : 'min';
MAX         : 'max';
ABS         : 'abs';
ROUND       : 'round';
CEIL        : 'ceil';
FLOOR       : 'floor';
EXODUS      : 'exodus';
EXODUSLN    : 'exodusln';
RAID        : 'raid';

// ============================================================================
// OPERATORS (Операторы)
// ============================================================================

// Arithmetic operators (Арифметические операторы)
PLUS        : '+';
MINUS       : '-';
STAR        : '*';
SLASH       : '/';
DOUBLESLASH : '//';
PERCENT     : '%';

// Logical operators (Логические операторы)
AND         : '&&';
OR          : '||';
NOT         : '!';

// Comparison operators (Операторы сравнения)
EQ          : '==';
NEQ         : '!=';
LTE         : '<=';
GTE         : '>=';
LT          : '<';
GT          : '>';

// Assignment operator (Оператор присваивания)
ASSIGN      : '=';

// ============================================================================
// DELIMITERS (Разделители)
// ============================================================================

LPAREN      : '(';
RPAREN      : ')';
LBRACE      : '{';
RBRACE      : '}';
LBRACKET    : '[';
RBRACKET    : ']';
COMMA       : ',';
SEMICOLON   : ';';
COLON       : ':';
QUESTION    : '?';

// ============================================================================
// LITERALS (Литералы)
// ============================================================================

// Integer literal (Целочисленный литерал)
INT_LITERAL
    : DIGIT+
    ;

// Float literal (Вещественный литерал)
FLOAT_LITERAL
    : DIGIT+ '.' DIGIT+
    ;

// String literal (Строковый литерал)
STRING_LITERAL
    : '"' (~["\r\n])* '"'
    | '\'' (~['\r\n])* '\''
    ;

// Identifier (Идентификатор)
// идентификатор = буква, {буква | цифра | спец_символ}
//               | спец_символ, буква, {буква | цифра | спец_символ};
IDENTIFIER
    : LETTER (LETTER | DIGIT | '_')*
    | '_' LETTER (LETTER | DIGIT | '_')*
    ;

// ============================================================================
// COMMENTS (Комментарии)
// ============================================================================

// Single-line comment (Однострочный комментарий)
LINE_COMMENT
    : '#' ~[\r\n]* -> skip
    ;

// Multi-line comment (Многострочный комментарий)
BLOCK_COMMENT
    : '\\*' .*? '*\\' -> skip
    ;

// ============================================================================
// FRAGMENTS (Фрагменты)
// ============================================================================

fragment DIGIT: [0-9];

fragment LETTER: [a-zA-Z];