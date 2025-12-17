# MysticGameScript

Представляет собой c-подобный язык

## Общая структура проекта

```
./
├── docs/                         # Документация к проекту
│   ├── competitors/              # Примеры кода на других языках программирования и их анализ
│   │   ├── *.md
│   │   └── ...
│   ├── specification/            # Спецификация своего языка программирования
│   │   └── *.md
│   └── theory/                   # Памятки по теории
│       └── *.md
├── src/                          # Исходный код на C#, кроме тестов
│   ├── Lexer/
│   │   └── Lexer.csproj
│   ├── ...
│   └── Interpreter/
│       └── Interpreter.csproj
├── tests/                        # Тесты, модульные и приёмочные.
│   ├── Lexer.UnitTests/
│   │   └── Lexer.UnitTests.csproj
│   ├── ...
│   └── Interpreter.Specs/
│       └── Interpreter.Specs.csproj
├── .editorconfig
├── Directory.Build.props         # Общие параметры сборки
└── MySolution.sln                # Файл решения
```

## Запуск тестов

```shell
dotnet test
```