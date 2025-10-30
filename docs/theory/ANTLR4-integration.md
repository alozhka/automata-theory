# Интеграция ANTLR4 в проект C# 12 / .NET 8

## Способ интеграции: пакет Antlr4BuildTasks

Этот метод автоматически генерирует код C# из грамматик ANTLR4 во время сборки проекта.

---

## Шаг 1. Установка NuGet-пакетов

Добавьте два пакета в проект:

```bash
dotnet add package Antlr4.Runtime.Standard
dotnet add package Antlr4BuildTasks
```

**Проверка:**
```bash
dotnet list package
```

Должны отобразиться установленные пакеты `Antlr4.Runtime.Standard` и `Antlr4BuildTasks`.

### Дополнительно:

Установите java, если её ещё нет

```bash
brew install openjdk@17
java -version
```

На ней будет запускаться генератор.

---

## Шаг 2. Создание грамматики

Создайте файл грамматики с расширением `.g4` в корне проекта или в отдельной папке (например, `Grammar/`):

```antlr
grammar Example;

// Правила парсера
program: statement+ ;
statement: ID '=' expr ';' ;
expr: ID | NUMBER ;

// Правила лексера
ID: [a-zA-Z]+ ;
NUMBER: [0-9]+ ;
WS: [ \t\r\n]+ -> skip ;
```

---

## Шаг 3. Настройка генерации (опционально)

По умолчанию `Antlr4BuildTasks` генерирует код для всех `.g4` файлов в проекте.

Для настройки параметров генерации добавьте в `.csproj`:

```xml
<ItemGroup>
  <Antlr4 Include="Grammar\*.g4">
    <Listener>true</Listener>
    <Visitor>true</Visitor>
  </Antlr4>
</ItemGroup>
```

Параметры:
- `Listener` — генерировать интерфейсы слушателей (по умолчанию `true`)
- `Visitor` — генерировать интерфейсы посетителей (по умолчанию `false`)

---

## Шаг 4. Сборка проекта

Выполните сборку:

```bash
dotnet build
```

**Проверка:**

Убедитесь, что в выводе появились строки о генерации ANTLR4:
```
Antlr4BuildTasks -> generating files for Example.g4
```

Проверьте наличие сгенерированных файлов в `obj/Debug/net8.0/`:
```bash
ls obj/Debug/net8.0/ | grep Example
```

Должны появиться файлы:
- `ExampleLexer.cs`
- `ExampleParser.cs`
- `ExampleBaseListener.cs` / `ExampleListener.cs`
- `ExampleBaseVisitor.cs` / `ExampleVisitor.cs` (если включен Visitor)

---

## Шаг 5. Использование сгенерированного кода

Пример базового использования парсера:

```csharp
using Antlr4.Runtime;

// Создание входного потока
var input = new AntlrInputStream("x = 42;");

// Создание лексера
var lexer = new ExampleLexer(input);
var tokens = new CommonTokenStream(lexer);

// Создание парсера
var parser = new ExampleParser(tokens);

// Парсинг с корневого правила
var tree = parser.program();

// Вывод дерева разбора
Console.WriteLine(tree.ToStringTree(parser));
```

**Проверка:**
```bash
dotnet run
```

Должен вывестись результат парсинга в виде дерева.

---

## Примечания

- Грамматики автоматически перекомпилируются при изменении `.g4` файлов
- Сгенерированные файлы находятся в папке `obj/` и автоматически включаются в компиляцию
- Для чистой пересборки используйте: `dotnet clean && dotnet build`
- Для отладки грамматик можно использовать инструмент ANTLR4 для Java или онлайн-редакторы