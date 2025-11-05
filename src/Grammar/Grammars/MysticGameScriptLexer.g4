lexer grammar MysticGameScriptLexer;

// -----------------------
// Ключевые слова
// -----------------------

// Типы данных
DAYZINT     : 'dayzint';
FALLOUT     : 'fallout';
STRIKE      : 'strike';
STATUM      : 'statum';
ARAYA       : 'araya';

// Логические константы
READY       : 'ready';
NOREADY     : 'noready';
GHOST       : 'ghost';

// Управляющие конструкции
IFFY        : 'iffy';
ELYSIFFY    : 'elysiffy';
ELYSIAN     : 'elysian';
VALORANT    : 'valorant';
FORZA       : 'forza';
BREAKOUT    : 'breakout';
CONTRA      : 'contra';

// Функции и объявления
FUNKOTRON   : 'funkotron';
RETURNAL    : 'returnal';
MONUMENT    : 'monument';

// Встроенные функции
MIN         : 'min';
MAX         : 'max';
ABS         : 'abs';
ROUND       : 'round';
CEIL        : 'ceil';
FLOOR       : 'floor';
EXODUS      : 'exodus';
EXODUSLN    : 'exodusln';
RAID        : 'raid';

// -----------------------
// Операторы
// -----------------------

// Арифметические операторы
PLUS        : '+';
MINUS       : '-';
STAR        : '*';
SLASH       : '/';
DOUBLESLASH : '//';
PERCENT     : '%';

// Логические операторы
AND         : '&&';
OR          : '||';
NOT         : '!';

// Операторы сравнения
EQ          : '==';
NEQ         : '!=';
LTE         : '<=';
GTE         : '>=';
LT          : '<';
GT          : '>';

// Оператор присваивания
ASSIGN      : '=';

// -----------------------
// Разделители
// -----------------------

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

// -----------------------
// Литералы
// -----------------------

// Целочисленный литерал
INT_LITERAL : DIGIT+;

// Вещественный литерал
FLOAT_LITERAL : DIGIT+ '.' DIGIT+;

// Строковый литерал
STRING_LITERAL
    : '"' (~["\r\n])* '"'
    | '\'' (~['\r\n])* '\'';

// идентификатор = буква, {буква | цифра | спец_символ}
//               | спец_символ, буква, {буква | цифра | спец_символ};
IDENTIFIER
    : LETTER (LETTER | DIGIT | '_')*
    | '_' LETTER (LETTER | DIGIT | '_')*;

// -----------------------
// Комментарии
// -----------------------

// Однострочный комментарий
LINE_COMMENT: '#' ~[\r\n]* -> skip;

// Многострочный комментарий
BLOCK_COMMENT: '\\*' .*? '*\\' -> skip;

// -----------------------
// Фрагменты
// -----------------------

fragment DIGIT: [0-9];

fragment LETTER: [a-zA-Z];