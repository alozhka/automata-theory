# EBNF для лексем

Язык: C++

```ebnf
(* 1. Идентификатор *)

identifier = identifier_start, {identifier_continue};
identifier_start = letter | "_";
identifier_continue = letter | digit | "_";

digit = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9";
letter = "A" | "B" | "C" | "D" | "E" | "F" | "G" | "H" | "I" | "J" | "K" | "L" | "M" 
    | "N" | "O" | "P" | "Q" | "R" | "S" | "T" | "U" | "V" | "W" | "X" | "Y" | "Z" 
    | "a" | "b" | "c" | "d" | "e" | "f" | "g" | "h" | "i" | "j"  | "k" | "l" | "m" 
    | "n" | "o" | "p" | "q" | "r" | "s" | "t" | "u" | "v"  | "w" | "x" | "y" | "z" ;

(* 2. Литералы целых чисел *)

integer_literal = (decimal_literal | octal_literal | hex_literal | binary_digit), [integer_suffix];

decimal_literal = non_xero_digit, {digit | "'"}
octal_literal = "0", {octal_digit | "'"}
hex_literal = ("0x" | "0X"), hex_digit, {hex_digit | "'"}
binary_digit = ("0b" | "0B"), binary_digit, {binary_digit | "'"}

non_xero_digit = "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9";
octal_digit = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7";
hex_digit = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"
    | "A" | "B" | "C" | "D" | "E" | "F"
    | "a" | "b" | "c" | "d" | "e" | "f";
binary_digit = "0" | "1";

integer_suffix = unsigned_suffix, [long_suffix] | long_suffix, [unsigned_suffix];
unsigned_suffix = "u" | "U";
long_suffix = "l" | "L" | "ll" | "LL";

(* 3. Числа с плавающей точкой *)

floating_literal = fractional_constant, [exponent_part], [floating_suffix]
    | digit_sequence, exponent_part, [floating_suffix];

fractional_constant = [digit_sequence], ".", digit_sequence | digit_sequence, ".";
exponent_part = ("e" | "E"), [sign], digit_sequence;

digit_sequence = digit, {digit};
floating_suffix = "f" | "l" | "F" | "L";
sign = "+" | "-";

(* 4. Строковые литералы *)

string_literal = ('"' | "L"), s_char, {Rs_char}, '"';
s_char = supported_char | escape_sequence;
supported_char = ANY_CHAR - "\"" - "\\";

escape_sequence = simple_escape_sequence | octal_ecsape_sequence | hex_escape_sequence;
simple_escape_sequence = "\'" | '\"' | "\?" | "\\" | "\a" | "\b" | "\f" | "\n" | "\r" | "\t" | "\v";
octal_ecsape_sequence = "\", (octal_digit | octal_digit, octal_digit | octal_digit, octal_digit, octal_digit);
hex_escape_sequence = "\x", hex_digit, {hex_digit};
```