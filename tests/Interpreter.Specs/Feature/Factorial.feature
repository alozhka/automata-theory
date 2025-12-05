# language: ru-RU
Функциональность: факториал

    Сценарий: факториал
        Дано я запустил программу:
            """
            funkotron factorial(dayzint n)
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
            """
        И я установил входные данные:
            | Число  |
            | -5     |
        Когда я выполняю программу
        Тогда я получаю результаты:
            | Результат |
            | -1         |