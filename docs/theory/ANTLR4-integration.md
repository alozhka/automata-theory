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
        <Visitor>false</Visitor>
        <GAtn>false</GAtn>
        <Error>false</Error>
        <Package>MysticGameScript</Package>
    </Antlr4>
</ItemGroup>
```

Параметры:

- `Listener` — генерировать интерфейсы слушателей (по умолчанию `true`)
- `Visitor` — генерировать интерфейсы посетителей (по умолчанию `false`)
- `GAtn` — генерировать файлы ATN (Augmented Transition Network) в формате DOT для визуализации грамматики (по умолчанию
  `false`)
- `Error` — генерировать файлы с информацией об ошибках для отладки грамматики (по умолчанию `false`)
- `Package` — указать пространство имён (namespace) для генерируемых классов

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

---

## Шаг 5. Использование сгенерированного кода

Пример базового использования парсера:

```csharp
using Antlr4.Runtime;
using Grammar.Listener;
using MysticGameScript;

public static class MysticGameScript
{
    public static void ValidateQuery(string statement)
    {
        // Создание входного потока
        AntlrInputStream stream = new(statement);
        
        // Создание лексера
        MysticGameScriptLexer lexer = new(stream);
        // Создание потока токенов
        CommonTokenStream tokenStream = new(lexer);
        
        // Создание парсера
        MysticGameScriptParser parser = new(tokenStream);

        // Парсинг с корневого правила
        parser.program();
    }
}
```

---

## Примечания

- Сгенерированные файлы находятся в папке `obj/` и автоматически включаются в компиляцию
- Для чистой пересборки используйте: `dotnet clean && dotnet build`
