# language: ru-RU
Функциональность: НОД

    Сценарий: НОД
        Дано я запустил программу:
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
                
                exodus(getGCD(a, b));
            }
            """
        И я установил входные данные:
            | Число  |
            | 5    |
            | 15    |
        Когда я выполняю программу
        Тогда я получаю результаты:
            | Результат |
            | 5     |