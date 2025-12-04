parser grammar MysticGameScriptParser;

options {
    tokenVocab = MysticGameScriptLexer;
}

// -----------------------
// PROGRAM (Программа)
// -----------------------

program
    : (globalStatement)* mainCraftFunction EOF
    ;

// точка_входа = "maincraft", "()", блок_функции;
mainCraftFunction: MAINCRAFT LPAREN RPAREN functionBlock;

// глобальная_инструкция = объявление_переменной | объявление_константы | объявление_функции | инструкция;
globalStatement
    : variableDeclaration SEMICOLON
    | constantDeclaration SEMICOLON
    | functionDeclaration
    ;

// -----------------------
// TYPES (Типы данных)
// -----------------------

// базовый_тип = "dayzint" | "fallout" | "strike" | "statum";
baseType: DAYZINT | FALLOUT | STRIKE | STATUM;

// тип_массива = "araya", "<", тип, ">";
arrayType
    : ARAYA LT type GT
    ;

// нулл_тип = "?", базовый_тип | "?", тип_массива;
nullableType: QUESTION baseType | QUESTION arrayType;

// тип = базовый_тип | тип_массива | нулл_тип;
type: baseType | arrayType | nullableType;

// -----------------------
// EXPRESSIONS (Выражения)
// -----------------------

// литерал_выражения = число | строка | "ready" | "noready" | "ghost" | массив;
literalExpression
    : INT_LITERAL
    | FLOAT_LITERAL
    | STRING_LITERAL
    | READY
    | NOREADY
    | GHOST
    | arrayLiteral
    ;

// Инициализация массива: {value1, value2, ...}
arrayLiteral: LBRACE (expression (COMMA expression)*)? RBRACE;

// элемент_массива = идентификатор, "[", выражение, "]";
arrayElement: IDENTIFIER LBRACKET expression RBRACKET;

// вызов_функции = идентификатор, "(", [выражение, {",", выражение}], ")";
functionCall: IDENTIFIER LPAREN (expression (COMMA expression)*)? RPAREN;

// вызов_заложенной_функции = ...
builtInFunctionCall
    : MIN LPAREN expression COMMA expression (COMMA expression)* RPAREN
    | MAX LPAREN expression COMMA expression (COMMA expression)* RPAREN
    | ABS LPAREN expression RPAREN
    | ROUND LPAREN expression RPAREN
    | CEIL LPAREN expression RPAREN
    | FLOOR LPAREN expression RPAREN
    | EXODUS LPAREN expression RPAREN
    | EXODUSLN LPAREN expression RPAREN
    | RAID LPAREN IDENTIFIER RPAREN
    ;

// часть_выражения = литерал_выражения | идентификатор
//              | вызов_функции | вызов_заложенной_функции
//              | элемент_массива | "(", выражение, ")";
primaryExpression
    : literalExpression
    | IDENTIFIER
    | functionCall
    | builtInFunctionCall
    | arrayElement
    | LPAREN expression RPAREN
    ;

// унарное_выражение = [унарный_оператор], часть_выражения;
unaryExpression: (MINUS | NOT)? primaryExpression;

// мультипликативное_выражение = унарное_выражение, {мультипликативный_оператор, унарное_выражение};
multiplicativeExpression: unaryExpression ((STAR | SLASH | DOUBLESLASH | PERCENT) unaryExpression)*;

// аддитивное_выражение = мультипликативное_выражение, {адитивный_оператор, мультипликативное_выражение};
additiveExpression: multiplicativeExpression ((PLUS | MINUS) multiplicativeExpression)*;

// выражение_сравнения = аддитивное_выражение, [оператор_сравнения, аддитивное_выражение];
comparisonExpression: additiveExpression ((LT | GT | EQ | NEQ | LTE | GTE) additiveExpression)?;

// логическое_выражение = выражение_сравнения, {логический_оператор, выражение_сравнения};
logicalExpression: comparisonExpression ((AND | OR) comparisonExpression)*;

// выражение = логическое_выражение;
expression: logicalExpression;

// -----------------------
// STATEMENTS (Инструкции)
// -----------------------

// присваивание = идентификатор, "=", выражение;
assignment: IDENTIFIER ASSIGN expression;

// объявление_переменной = ["?"], тип, идентификатор, ["=", выражение];
variableDeclaration: type IDENTIFIER (ASSIGN expression)?;

// объявление_константы = "monument", тип, присваивание;
constantDeclaration: MONUMENT type IDENTIFIER ASSIGN expression;

// блок_функции = "{", {инструкция_функции}, "}";
functionBlock: LBRACE statement* RBRACE;

// если = "iffy", "(", выражение, ")", блок;
ifStatement: IFFY LPAREN expression RPAREN functionBlock;

// иначе = "elysian", блок;
elseStatement: ELYSIAN functionBlock;

// условие = если, [иначе];
conditionalStatement: ifStatement elseStatement?;

// прерывание_цикла = "breakout", ";" | "contra", ";";
loopBreak: BREAKOUT SEMICOLON | CONTRA SEMICOLON;

// цикл_while = "valorant", "(", выражение, ")", блок;
whileLoop: VALORANT LPAREN expression RPAREN functionBlock;

// цикл_for = "forza", "(", (объявление_переменной | присваивание), ";", выражение, ";", присваивание, ")", блок;
forLoop: FORZA LPAREN (variableDeclaration | assignment) SEMICOLON expression SEMICOLON assignment RPAREN functionBlock;

// возврат = "returnal", [выражение], ";";
returnStatement: RETURNAL expression? SEMICOLON;

// инструкция = объявление_переменной | объявление_константы | присваивание | условие
//              | цикл_while | цикл_for | вызов_функции | вызов_заложенной_функции
//              | прерывание_цикла | возврат;
statement
    : variableDeclaration SEMICOLON
    | constantDeclaration SEMICOLON
    | assignment SEMICOLON
    | conditionalStatement
    | whileLoop
    | forLoop
    | functionCall SEMICOLON
    | builtInFunctionCall SEMICOLON
    | loopBreak
    | returnStatement
    ;

// -----------------------
// FUNCTIONS (Функции)
// -----------------------

// параметр = тип, идентификатор;
parameter: type IDENTIFIER;

// список_параметров = параметр, {",", параметр};
parameterList: parameter (COMMA parameter)*;

// объявление_функции = "funkotron", идентификатор, "(", [список_параметров], ")", [":", тип], блок_функции;
functionDeclaration: FUNKOTRON IDENTIFIER LPAREN parameterList? RPAREN (COLON type)? functionBlock;