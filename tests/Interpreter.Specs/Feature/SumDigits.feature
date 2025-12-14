# language: ru-RU
Функциональность: сумма цифр

    Сценарий: сумма цифр
        Дано я запустил программу:
            """
            funkotron sumDigits(dayzint number): dayzint
            {
                monument dayzint RADIX = 10;
            
                dayzint sum = 0;
                valorant (number != 0) {
                    sum = sum + number % RADIX;
                    number = number / RADIX;
                }   
            
                returnal sum;
            }
            
            maincraft()
            {
                dayzint number;
                raid(number);
                
                exodus(sumDigits(number));
            }
            """
        И я установил входные данные:
            | Число    |
            | 235      |
        Когда я выполняю программу
        Тогда я получаю результаты:
            | Результат |
            | 10        |