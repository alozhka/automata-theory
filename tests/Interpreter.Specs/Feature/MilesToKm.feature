# language: ru-RU
Функциональность: Мили в км

    Сценарий: Мили в км
        Дано я запустил программу:
            """
            monument fallout MILES_PER_KM = 1.60934;
            
                maincraft()
                {
                    fallout miles;
                    raid(miles);
    
                    exodusln(miles * MILES_PER_KM);
                }
            """
        И я установил входные данные:
            | Число |
            | 10.0    |
        Когда я выполняю программу
        Тогда я получаю результаты:
            | Результат |
            | 16.0934   |