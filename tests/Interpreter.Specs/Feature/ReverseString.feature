# language: ru-RU
Функциональность: Реверс строки

    Сценарий: Реверс строки
        Дано я запустил программу:
            """
            funkotron getReverseStrike(strike str): strike
            {
                dayzint strLength = length(str);
                strike reverseStrike;
                forza (dayzint i = 0; i < strLength; i = i + 1) {
                    reverseStrike = str_at(str, i) + reverseStrike;
                }
            
                returnal reverseStrike;
            }
            
            maincraft()
            {
                strike str;
                raid(str);
                
                exodus(getReverseStrike(str));
            }
            """
        И я установил входные данные:
            | Число |
            | 12345    |
        Когда я выполняю программу
        Тогда я получаю результаты:
            | Результат |
            | 54321       |