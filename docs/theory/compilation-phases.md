# Фазы компиляции

Диаграмма основных фаз процесса компиляции исходного кода в исполняемую программу на примере языка C++.

```mermaid
flowchart TD
    A[main.cpp]
    B[Лексический анализ<br/>Lexer]
    C[Синтаксический анализ<br/>Parser]
    D[Семантический анализ<br/>Semantic Analyzer]
    E[Генерация промежуточного кода<br/>IR Generator]
    F[Оптимизация кода<br/>Optimizer]
    G[Генерация ассемблера<br/>Code Generator]
    H[main.o]
    I[Компоновщик<br/>Linker]
    J[main.exe]

    A -->|Поток символов| B
    B -->|Поток токенов| C
    C -->|Abstract Syntax Tree| D
    D -->|Annotated AST| E
    E -->|Intermediate Code| F
    F -->|Optimized IR| G
    G -->|Ассемблерный код| H
    H -->|Объектный файл| I
    I -->|Исполняемый файл| J

    style A fill:#e1f5fe
    style J fill:#c8e6c9
    style B fill:#fff3e0
    style C fill:#fff3e0
    style D fill:#fff3e0
    style E fill:#f3e5f5
    style F fill:#f3e5f5
    style G fill:#f3e5f5
    style H fill:#ffecb3
    style I fill:#ffecb3
```

## Описание фаз

### 1. Лексический анализ (Lexical Analysis / Lexer)
- **Входные данные**: Исходный код C++ (main.cpp)
- **Выходные данные**: Поток токенов (keywords, identifiers, operators, literals)
- **Функция**: Разбивает исходный код на лексемы, удаляет комментарии и пробелы

### 2. Синтаксический анализ (Syntax Analysis / Parser)
- **Входные данные**: Поток токенов
- **Выходные данные**: Abstract Syntax Tree (AST)
- **Функция**: Проверяет синтаксическую корректность C++ и строит дерево разбора

### 3. Семантический анализ (Semantic Analysis)
- **Входные данные**: Abstract Syntax Tree
- **Выходные данные**: Annotated AST
- **Функция**: Проверяет типизацию C++, область видимости, разрешение перегрузок

### 4. Генерация промежуточного кода (IR Generation)
- **Входные данные**: Annotated AST
- **Выходные данные**: Intermediate Representation (LLVM IR, RTL)
- **Функция**: Преобразует AST в промежуточное представление для оптимизации

### 5. Оптимизация кода (Optimization)
- **Входные данные**: Intermediate Code
- **Выходные данные**: Optimized IR
- **Функция**: Выполняет оптимизации: inline функций, устранение мертвого кода, векторизация

### 6. Генерация ассемблера (Code Generation)
- **Входные данные**: Optimized IR
- **Выходные данные**: Ассемблерный код (x86-64, ARM)
- **Функция**: Преобразует IR в ассемблер целевой архитектуры

### 7. Ассемблирование и компоновка (Assembly & Linking)
- **Входные данные**: Ассемблерный код
- **Выходные данные**: Исполняемый файл (main.exe)
- **Функция**: Ассемблер создает объектный файл, компоновщик связывает с библиотеками