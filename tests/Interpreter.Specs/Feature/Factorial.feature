# language: ru-RU
Функциональность: факториал

    Сценарий: факториал
        Дано я запустил программу:
            """
            funkotron factorial(fallout n): fallout
            {
                iffy (n < 0.0) {
                    returnal -1.0;
                }
            
                fallout factorial = 1.0;
                forza (fallout i = 1.0; i <= n; i = i + 1.0) {
                    factorial = factorial * i;
                }
            
                returnal factorial;
            }
            
            maincraft()
            {
                dayzint n;
                raid(n);
                
                exodus(factorial(n));
            }
            """
        И я установил входные данные:
            | Число  |
            | 5      |
        Когда я выполняю программу
        Тогда я получаю результаты:
            | Результат |
            | 120.0     |