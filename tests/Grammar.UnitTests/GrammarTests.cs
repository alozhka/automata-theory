using Antlr4.Runtime.Misc;

namespace Grammar.UnitTests;

public class GrammarTests
{
    [Theory]
    [MemberData(nameof(ListValidStatements))]
    public void Accepts_valid_statements(string stmt)
    {
        MysticGameScriptGrammar.ValidateQuery(stmt);
    }

    [Theory]
    [MemberData(nameof(ListInvalidStatements))]
    public void Rejects_invalid_statements(string stmt)
    {
        Assert.Throws<ParseCanceledException>(() => MysticGameScriptGrammar.ValidateQuery(stmt));
    }

    public static TheoryData<string> ListValidStatements()
    {
        return new TheoryData<string>
        {
            // Объявление переменных - базовые типы
            // 1. целочисленная переменная со значением по умолчанию 0
            "dayzint x; maincraft() {}",

            // 2. целочисленная переменная со значением по умолчанию ghost
            "?dayzint x; ? dayzint x; maincraft() {}",

            // 3. целочисленная переменная с заданным значением
            "dayzint x2 = 5; maincraft() {}",

            // 4. переменная с плавающей запятой со значением по умолчанию 0
            "fallout x; maincraft() {}",

            // 5. переменная с плавающей запятой со значением по умолчанию ghost
            "?fallout x; maincraft() {}",

            // 6. переменная с плавающей запятой с заданным значением
            "fallout x2 = 7.9; maincraft() {}",

            // 7. логическая переменная со значением по умолчанию 0
            "statum x; maincraft() {}",

            // 8. логическая переменная со значением по умолчанию ghost
            "?statum x; maincraft() {}",

            // 9. логическая переменная с заданным значением
            "statum x2 = ready; maincraft() {}",

            // 10. строка со значением по умолчанию 0
            "strike x; maincraft() {}",

            // 11. строка со значением по умолчанию ghost
            "?strike x; maincraft() {}",

            // 12. строка с заданным значением
            "strike x2 = \"sere\"; maincraft() {}",

            // Массивы
            // 13. массив с типом dayzint с значением по умолчанию
            "araya<dayzint> arr; maincraft() {}",

            // 14. массив с типом dayzint с значением по умолчанию ghost
            "?araya<dayzint> arr; maincraft() {}",

            // Встроенные функции
            // 15. чтение данных в переменную
            "maincraft() { raid(x); }",

            // 16. вывод данных переменной
            "maincraft() { exodus(x); }",

            // 17. вывод ghost'a
            "maincraft() { exodus(ghost); }",

            // 18. вывод с переносом строки
            "maincraft() { exodusln(x); }",

            // Управляющие конструкции
            // 19. условие iffy
            "maincraft() { iffy (x == 5) { } }",

            // 20. выполнение кода, если условие оказалось ложным elysian
            "maincraft() { iffy (x == 5) { } elysian { } }",

            // 22. цикл пока
            "maincraft() { valorant (x < 5) { } }",

            // 23. цикл for
            "maincraft() { forza (dayzint j = 0; j < 5; j = j + 1) { } }",

            // 24. выход из функции (breakout)
            "maincraft() { valorant (x < 5) { breakout; } }",

            // 25. переход к следующей итерации цикла
            "maincraft() { valorant (x < 5) { contra; } }",

            // Функции
            // 26. объявление функции без параметров
            "funkotron x(): dayzint { returnal 0; } maincraft() {}",

            // 27. объявление функции с параметрами
            "funkotron divide(dayzint a, dayzint b): ?fallout { returnal a / b; } maincraft() {}",

            // 28. возврат данных из функции
            "funkotron test(): dayzint { dayzint z; returnal z; } maincraft() {}",

            // Операторы - арифметические
            // 29. оператор +
            "dayzint result = x + y; maincraft() {}",

            // 30. оператор -
            "dayzint result = x - y; maincraft() {}",

            // 31. оператор *
            "dayzint result = x * y; maincraft() {}",

            // 32. оператор /
            "dayzint result = x / y; maincraft() {}",

            // 33. оператор %
            "dayzint result = x % y; maincraft() {}",

            // Операторы - присваивание
            // 34. оператор =
            "maincraft() { x = y; }",

            // Операторы - сравнение
            // 35. оператор ==
            "statum result = x == y; maincraft() {}",

            // 36. оператор !=
            "statum result = x != y; maincraft() {}",

            // 37. оператор <
            "statum result = x < y; maincraft() {}",

            // 38. оператор >
            "statum result = x > y; maincraft() {}",

            // 39. оператор <=
            "statum result = x <= y; maincraft() {}",

            // 40. оператор >=
            "statum result = x >= y; maincraft() {}",

            // Операторы - логические
            // 41. оператор &&
            "statum result = x && y; maincraft() {}",

            // 42. оператор ||
            "statum result = x || y; maincraft() {}",

            // 43. оператор !
            "statum result = !x; maincraft() {}",

            // Дополнительные встроенные функции
            // 44. функция min
            "dayzint result = min(x, y); maincraft() {}",

            // 45. функция max
            "dayzint result = max(x, y); maincraft() {}",

            // 46. функция abs
            "dayzint result = abs(x); maincraft() {}",

            // 47. функция round
            "dayzint result = round(x); maincraft() {}",

            // 48. функция ceil
            "dayzint result = ceil(x); maincraft() {}",

            // 49. функция floor
            "dayzint result = floor(x); maincraft() {}",

            // Комплексные выражения
            // 50. сложное выражение с приоритетом операторов
            "dayzint result = x + y * z; maincraft() {}",

            // 51. выражение со скобками
            "dayzint result = (x + y) * z; maincraft() {}",

            // 52. вложенные скобки
            "dayzint result = ((x + y) * z) / (a - b); maincraft() {}",

            // Комментарии
            // 53. однострочный комментарий
            "# это комментарий\ndayzint x; maincraft() {}",

            // 54. многострочный комментарий
            "\\* это многострочный\nкомментарий *\\\ndayzint x; maincraft() {}",

            // Дополнительные синтаксически валидные конструкции
            // 55. функция без типа возврата, но с returnal (синтаксически валидно)
            "funkotron test() { returnal 5; } maincraft() {}",

            // 56. breakout
            "maincraft() { breakout; }",

            // 57. целочисленная константа с заданным значением
            "monument dayzint x3 = 10; maincraft() {}",

            // Новая функциональность - массивы с инициализацией
            // 58. пустой массив
            "araya<dayzint> arr = {}; maincraft() {}",

            // 59. массив с несколькими элементами
            "araya<dayzint> arr2 = {4, 5}; maincraft() {}",

            // 60. массив с nullable типом и ghost значениями
            "araya<?dayzint> arr3 = {3, 4, ghost, 5}; maincraft() {}",

            // Новая функциональность - escape последовательности в строках
            // 61. экранированная двойная кавычка
            "strike str = \"Hello \\\"World\\\"\"; maincraft() {}",

            // 62. экранированная одинарная кавычка
            "strike str2 = 'It\\'s working'; maincraft() {}",

            // 63. экранированный обратный слэш
            "strike str3 = \"C:\\\\Users\\\\test\"; maincraft() {}",

            // 64. перенос строки в строке
            "strike str4 = \"Line 1\\nLine 2\"; maincraft() {}",

            // 65. табуляция в строке
            "strike str5 = \"Col1\\tCol2\"; maincraft() {}",

            // Точка входа maincraft
            // 66. простая maincraft функция
            "maincraft() { }",

            // 67. maincraft с переменными
            "maincraft() { dayzint x = 5; exodus(x); }",

            // 68. Полная программа с глобальными переменными и функциями
            """
            dayzint x; # инициализируется значением по умолчанию = 0
            funkotron printInt(dayzint value)
            {
                exodus(value);
            }
            maincraft()
            {
                iffy (ready) {
                    dayzint x = 5;
                    printInt(x); # выведет 5
                }
                printInt(x); # выведет 0
            }
            """,
            """
            monument fallout PI = 3.14;
            maincraft()
            {
                fallout radius;
                raid(radius);
                
                exodusln(PI * radius * radius);
            }
            """,
            """
            monument fallout MILES_PER_KM = 1.60934;

            maincraft()
            {
                fallout miles;
                raid(miles);
                
                exodusln(miles * MILES_PER_KM);
            }
            """,
            """
            maincraft()
            {
                dayzint a;
                dayzint b;

                raid(a);
                raid(b);
                
                exodusln(a + b);
            }
            """,

            // GCD
            """
            funkotron getGCD(dayzint a, dayzint b): dayzint
            {
                a = abs(a);
                b = abs(b);

                valorant (b != 0) {
                    dayzint temp = b;
                    b = a % b;
                    a = temp;
                }

                returnal a;
            }

            maincraft()
            {
                dayzint a;
                dayzint b;
                
                raid(a);
                raid(b);
                
                exodusln(getGCD(a, b));
            }
            """,

            // Factorial
            """
            funkotron factorial(dayzint n): dayzint
            {
                iffy (n < 0) {
                    returnal -1;
                }

                dayzint factorial = 1;
                forza (dayzint i = 1; i <= n; i = i + 1) {
                    factorial = factorial * i;
                }

                returnal factorial;
            }

            maincraft()
            {
                dayzint n;
                raid(n);
                
                exodusln(factorial(n));
            }
            """,

            // sumDigits
            """
            funkotron sumDigits(dayzint number): dayzint
            {
                monument dayzint RADIX = 10;

                dayzint sum = 0;
                valorant (number != 0) {
                    sum = sum + number % RADIX;
                    number = number // RADIX;
                }   

                returnal sum;
            }

            maincraft()
            {
                dayzint number;
                raid(number);
                
                exodusln(sumDigits(number));
            }
            """,
        };
    }

    public static TheoryData<string> ListInvalidStatements()
    {
        return new TheoryData<string>
        {
            // Синтаксические ошибки
            // 1. Отсутствует точка с запятой
            "dayzint x",

            // 2. Неизвестный тип данных
            "integer x;",

            // 3. Неправильный синтаксис объявления массива
            "araya dayzint arr;",

            // 4. Отсутствует закрывающая скобка
            "iffy (x == 5 { }",

            // 5. Отсутствует открывающая фигурная скобка
            "iffy (x == 5) }",

            // 6. Неверный синтаксис функции - отсутствуют скобки
            "funkotron test { }",

            // 7. Неправильное использование оператора
            "dayzint x = + 5;",

            // 8. Двойная точка с запятой
            "dayzint x;;",

            // 9. Незакрытая строка
            "strike x = \"hello;",

            // 10. Использование зарезервированного слова как идентификатора
            "dayzint iffy;",

            // 11. Неправильный синтаксис цикла for - отсутствует точка с запятой
            "forza (dayzint i = 0 i < 5; i = i + 1) { }",
            "valorant () {}",

            // 12. Пустое выражение в условии
            "iffy () { }",

            // 13. Неправильный синтаксис вызова функции (exodus требует аргумент)
            "exodus();",

            // 14. Использование несуществующего оператора
            "dayzint x = y ** z;",

            // 15. Двойное отрицание без скобок
            "statum x = !!y;",

            // 16. Присваивание без переменной
            "= 5;",

            // 17. Объявление константы без значения
            "monument dayzint x;",

            // Новые невалидные конструкции для тестирования новой функциональности
            // 18. Массив с неправильным синтаксисом - пропущена закрывающая скобка
            "araya<dayzint> arr = {1, 2, 3;",

            // 19. Массив с лишней запятой в конце
            "araya<dayzint> arr = {1, 2, 3,};",
            "dayzint result = x  + + y; maincraft() {}",

            // Неправильное указание аргумента
            "funkotron factorial(n dayzint): dayzint{}",
        };
    }
}