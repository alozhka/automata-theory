# language: ru-RU
Функциональность: Гласные буквы

    Сценарий: Гласные буквы
        Дано я запустил программу:
            """
            funkotron isVowelChar(strike char): dayzint
            {
                returnal char == "a" || char == "A"
                    || char == "e" || char == "E"
                    || char == "i" || char == "I"
                    || char == "o" || char == "O"
                    || char == "u" || char == "U"
                    || char == "y" || char == "Y";
            }
            
            funkotron countVowels(strike str): dayzint
            {
                dayzint strLength = length(str);
                dayzint countVowels;
                forza (dayzint i = 0; i < strLength; i = i + 1) {
                    countVowels = countVowels + isVowelChar(str_at(str, i));
                }
            
                returnal countVowels;
            }
            
            maincraft()
            {
                strike str;
                raid(str);
                
                exodus(countVowels(str));
            }
            """
        И я установил входные данные:
            | Число |
            | abcde    |
        Когда я выполняю программу
        Тогда я получаю результаты:
            | Результат |
            | 2       |