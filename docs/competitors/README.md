# Сравнение языков PHP и C++: лексика, синтаксис и семантика
## 1. Синтаксис циклов, ветвлений и блоков кода
### Ветвления
#### C++

```cpp
    if (x > 10) {
        std::cout << "Больше";
    } else if (x == 10) {
        std::cout << "Равно";
    } else {
        std::cout << "Меньше";
    }
```
#### PHP

```php
    if ($x > 10) {
        echo "Больше";
    } elseif ($x == 10) {
        echo "Равно";
    } else {
        echo "Меньше";
    }
```

### Циклы
#### C++
```cpp
    for (int i = 0; i < 10; i++) {
        std::cout << i;
    }

    while (condition) {
        // код цикла
    }
```

#### PHP
```php
    for ($i = 0; $i < 10; $i++) {
        echo $i;
    }

    while ($condition) {
        // код цикла
    }

    // Дополнительно в PHP
    foreach ($array as $value) {
        echo $value;
    }
```

## 2. Объявление типов переменных и параметров
#### C++ (статическая типизация)
```cpp
    int number = 42;
    double price = 19.99;
    std::string name = "John";
    bool isValid = true;

    // Функция с типами
    int add(int a, int b) {
        return a + b;
    }
```

#### PHP (динамическая типизация)
```php
    $number = 42;           // integer
    $price = 19.99;         // float
    $name = "John";         // string
    $isValid = true;        // boolean

    // Функция с типами
    function add(int $a, int $b): int {
        return $a + $b;
    }
```

## 3. Набор типов данных

| Категория          | C++                                           | PHP                                       |
|--------------------|-----------------------------------------------|-------------------------------------------|
| **Базовые**        | `int`, `float`, `double`, `char`, `bool`      | `int`, `float`, `string`, `bool`          |
| **Составные**      | `массивы`, `структуры`, `классы`, `указатели` | `array`, `object`, `callable`, `iterable` |
| **Дополнительные** | `string`, `vector`, `map` (из STL)            | `null`, `resource`                        |

## 4. Операторы

| Категория          | C++                                         | PHP                                            |
|--------------------|---------------------------------------------|------------------------------------------------|
| **Арифметические** | `+`, `-`, `*`, `/`, `%`, `++`, `--`         | `+`, `-`, `*`, `/`, `%`, `++`, `--`            |
| **Сравнения**      | `==`, `!=`, `<`, `>`, `<=`, `>=`            | `==`, `!=`, `<`, `>`, `<=`, `>=`, `===`, `!==` |
| **Логические**     | `&&`, `\|\|`, `!`                           | `and`, `or`, `&&`, `\|\|`, `!`, `xor`          |
| **Побитовые**      | `&` `\|` `^` `~` `<<` `>>`                  | `&` `\|` `^` `~` `<<` `>>`                     |
| **Строковые**      | `+` для конкатенации (только с std::string) | `.` для конкатенации                           |

## 5. Пользовательские функции
#### C++
```cpp
    // Возвращает тип, параметры с типами
    int multiply(int a, int b) {
        return a * b;
    }

    // Перегрузка функций разрешена
    int multiply(int a, int b, int c) {
        return a * b * c;
    }
```
#### PHP
```php
    // Типы опциональны
    function multiply($a, $b) {
        return $a * $b;
    }

    // С типами (PHP 7+)
    function multiply(int $a, int $b): int {
        return $a * $b;
    }

    // Значения по умолчанию
    function greet($name = "Гость") {
        return "Привет, $name";
    }
```

## 6. Пользовательские структуры/классы
#### C++
```cpp
    class Person {
    private:
        std::string name;
        int age;
        
    public:
        Person(std::string n, int a) : name(n), age(a) {}
        
        std::string getName() { return name; }
        void setAge(int a) { age = a; }
    };
```
#### PHP
```php
    class Person {
        private $name;
        private $age;
        
        public function __construct($name, $age) {
            $this->name = $name;
            $this->age = $age;
        }
        
        public function getName() {
            return $this->name;
        }
        
        public function setAge($age) {
            $this->age = $age;
        }
    }
```

## 7. Управление памятью
#### C++
 - Ручное управление через new/delete
 - Умные указатели: unique_ptr, shared_ptr
 - Контроль времени жизни объектов
 - Возможность утечек памяти

```cpp
    int* arr = new int[100];
    delete[] arr;
```
#### PHP
 - Автоматическое управление через сборщик мусора
 - Нет прямого доступа к памяти
 - Переменные уничтожаются при выходе из области видимости
 - Нет необходимости освобождать память вручную

## 8. Обработка ошибок
#### C++ (исключения)
```cpp
    try {
        if (x == 0) {
            throw runtime_error("Деление на ноль");
        }
        result = 100 / x;
    } catch (const exception& e) {
        std::cout << "Ошибка: " << e.what() << std::endl;
    }
```
#### PHP (исключения + традиционные ошибки)
```php
    try {
        if ($x == 0) {
            throw new Exception("Деление на ноль");
        }
        $result = 100 / $x;
    } catch (Exception $e) {
        echo "Ошибка: " . $e->getMessage();
    }

    // Традиционный способ
    if (!file_exists("file.txt")) {
        trigger_error("Файл не найден", E_USER_WARNING);
    }
```

## Ключевые различия
| Аспект                  | C++                        | PHP              |
|-------------------------|----------------------------|------------------|
| **Типизация**	          | Статическая                | Динамическая     |
| **Компиляция**          | Компилируемый	           | Интерпретируемый |
| **Память**	          | Ручное управление          | Автоматическое   |
| **Основное применение** |	Системное программирование | Веб-разработка   |
| **Производительность**  | Высокая                    | Средняя          |
| **Безопасность типов**  | Строгая                    | Слабая           |
