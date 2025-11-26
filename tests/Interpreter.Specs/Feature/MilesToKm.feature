# language: ru-RU
Функциональность: Мили в км

    Сценарий: Мили в км
        Дано я запустил программу:
            """
            monument dayzint MILES_PER_KM = 1.60934;
            
                maincraft()
                {
                    dayzint miles;
                    raid(miles);
    
                    exodusln(miles * MILES_PER_KM);
                }
            """
        И я установил входные данные:
            | Число |
            | 10    |
        Когда я выполняю программу
        Тогда я получаю результаты:
            | Результат |
            | 16.0934   |