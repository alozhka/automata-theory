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
            "dayzint x;",

            // 2. целочисленная переменная со значением по умолчанию ghost
            "?dayzint x; ? dayzint x;",

            // 3. целочисленная переменная с заданным значением
            "dayzint x2 = 5;",

            // 4. переменная с плавающей запятой со значением по умолчанию 0
            "fallout x;",

            // 5. переменная с плавающей запятой со значением по умолчанию ghost
            "?fallout x;",

            // 6. переменная с плавающей запятой с заданным значением
            "fallout x2 = 7.9;",

            // 7. логическая переменная со значением по умолчанию 0
            "statum x;",

            // 8. логическая переменная со значением по умолчанию ghost
            "?statum x;",

            // 9. логическая переменная с заданным значением
            "statum x2 = ready;",

            // 10. строка со значением по умолчанию 0
            "strike x;",

            // 11. строка со значением по умолчанию ghost
            "?strike x;",

            // 12. строка с заданным значением
            "strike x2 = \"sere\";",

            // Массивы
            // 13. массив с типом dayzint с значением по умолчанию
            "araya<dayzint> arr;",

            // 14. массив с типом dayzint с значением по умолчанию ghost
            "?araya<dayzint> arr;",

            // Встроенные функции
            // 15. чтение данных в переменную
            "raid(x);",

            // 16. вывод данных переменной
            "exodus(x);",

            // 17. вывод ghost'a
            "exodus(ghost);",

            // 18. вывод с переносом строки
            "exodusln(x);",

            // Управляющие конструкции
            // 19. условие iffy
            "iffy (x == 5) { }",

            // 20. выполнение кода, если условие оказалось ложным elysian
            "iffy (x == 5) { } elysian { }",

            // 21. условие, если предыдущее оказалось ложным elysiffy
            "iffy (x == 5) { } elysiffy (x == 10) { }",

            // 22. цикл пока
            "valorant (x < 5) { }",

            // 23. цикл for
            "forza (dayzint j = 0; j < 5; j = j + 1) { }",

            // 24. выход из функции (breakout)
            "valorant (x < 5) { breakout; }",

            // 25. переход к следующей итерации цикла
            "valorant (x < 5) { contra; }",

            // Функции
            // 26. объявление функции без параметров
            "funkotron x(): dayzint { returnal 0; }",

            // 27. объявление функции с параметрами
            "funkotron divide(dayzint a, dayzint b): ?fallout { returnal a / b; }",

            // 28. возврат данных из функции
            "funkotron test(): dayzint { dayzint z; returnal z; }",

            // Операторы - арифметические
            // 29. оператор +
            "dayzint result = x + y;",

            // 30. оператор -
            "dayzint result = x - y;",

            // 31. оператор *
            "dayzint result = x * y;",

            // 32. оператор /
            "dayzint result = x / y;",

            // 33. оператор %
            "dayzint result = x % y;",

            // Операторы - присваивание
            // 34. оператор =
            "x = y;",

            // Операторы - сравнение
            // 35. оператор ==
            "statum result = x == y;",

            // 36. оператор !=
            "statum result = x != y;",

            // 37. оператор <
            "statum result = x < y;",

            // 38. оператор >
            "statum result = x > y;",

            // 39. оператор <=
            "statum result = x <= y;",

            // 40. оператор >=
            "statum result = x >= y;",

            // Операторы - логические
            // 41. оператор &&
            "statum result = x && y;",

            // 42. оператор ||
            "statum result = x || y;",

            // 43. оператор !
            "statum result = !x;",

            // Дополнительные встроенные функции
            // 44. функция min
            "dayzint result = min(x, y);",

            // 45. функция max
            "dayzint result = max(x, y);",

            // 46. функция abs
            "dayzint result = abs(x);",

            // 47. функция round
            "dayzint result = round(x);",

            // 48. функция ceil
            "dayzint result = ceil(x);",

            // 49. функция floor
            "dayzint result = floor(x);",

            // Комплексные выражения
            // 50. сложное выражение с приоритетом операторов
            "dayzint result = x + y * z;",

            // 51. выражение со скобками
            "dayzint result = (x + y) * z;",

            // 52. вложенные скобки
            "dayzint result = ((x + y) * z) / (a - b);",

            // Комментарии
            // 53. однострочный комментарий
            "# это комментарий\ndayzint x;",

            // 54. многострочный комментарий
            "\\* это многострочный\nкомментарий *\\\ndayzint x;",

            // Дополнительные синтаксически валидные конструкции
            // 55. функция без типа возврата, но с returnal (синтаксически валидно)
            "funkotron test() { returnal 5; }",

            // 56. breakout вне цикла
            "breakout;",

            // 57. целочисленная константа с заданным значением
            "monument dayzint x3 = 10;",

            // Новая функциональность - массивы с инициализацией
            // 58. пустой массив
            "araya<dayzint> arr = {};",

            // 59. массив с несколькими элементами
            "araya<dayzint> arr2 = {4, 5};",

            // 60. массив с nullable типом и ghost значениями
            "araya<?dayzint> arr3 = {3, 4, ghost, 5};",

            // Новая функциональность - escape последовательности в строках
            // 61. экранированная двойная кавычка
            "strike str = \"Hello \\\"World\\\"\";",

            // 62. экранированная одинарная кавычка
            "strike str2 = 'It\\'s working';",

            // 63. экранированный обратный слэш
            "strike str3 = \"C:\\\\Users\\\\test\";",

            // 64. перенос строки в строке
            "strike str4 = \"Line 1\\nLine 2\";",

            // 65. табуляция в строке
            "strike str5 = \"Col1\\tCol2\";",

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
        };
    }
}