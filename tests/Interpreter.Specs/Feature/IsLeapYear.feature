# language: ru-RU
Функциональность: Год

    Сценарий: Год
        Дано я запустил программу:
            """
            funkotron checkStrToDayzint(strike str): dayzint
            {
                dayzint strLength = length(str);
                iffy (strLength == 0) {
                    returnal 0;
                }
                dayzint startIndex;
                iffy (str_at(str, 0) == "-") {
                    iffy (strLength <= 1) {
                        returnal 0;
                    }
                    startIndex = 1;
                }
                forza (dayzint i = startIndex; i < strLength; i = i + 1) {
                    strike char = str_at(str, i);
                    iffy (char < "0" || char > "9") {
                        returnal 0;
                    }
                }
            
                returnal 1;
            }
            
            funkotron charToDayzint(strike char): dayzint
            {
                iffy (char == "0") {returnal 0;}
                iffy (char == "1") {returnal 1;}
                iffy (char == "2") {returnal 2;}
                iffy (char == "3") {returnal 3;}
                iffy (char == "4") {returnal 4;}
                iffy (char == "5") {returnal 5;}
                iffy (char == "6") {returnal 6;}
                iffy (char == "7") {returnal 7;}
                iffy (char == "8") {returnal 8;}
                iffy (char == "9") {returnal 9;}
                returnal -1;
            }
            
            funkotron strToDayzint(strike str): dayzint
            {
                dayzint startIndex;
                iffy (str_at(str, 0) == "-") {
                    startIndex = 1;
                }
            
                dayzint strLength = length(str);
                dayzint number;
                forza (dayzint i = startIndex; i < strLength; i = i + 1) {
                    strike char = str_at(str, i);
                    number = number * 10 + charToDayzint(char);
                }
            
                iffy (startIndex) {
                    number = number * -1;
                }
            
                returnal number;
            }
            
            funkotron isLeapYear(dayzint year): dayzint
            {
                iffy (year % 400 == 0) {
                    returnal 1;
                }
                iffy (year % 100 == 0) {
                    returnal 0; 
                }
                iffy (year % 4 == 0) {
                    returnal 1;
                }
                returnal 0;                           
            }
            
            maincraft()
            {
                strike str;
                raid(str);
                
                iffy (checkStrToDayzint(str)) {
                    dayzint year = strToDayzint(str);
                    iffy (isLeapYear(year)) {
                        exodus("yes");
                    } 
                    elysian {
                        exodus("no");
                    }
                }
                elysian {
                    exodus("The entered string is not a year");
                }    
            }
            """
        И я установил входные данные:
            | Число |
            | 2024    |
        Когда я выполняю программу
        Тогда я получаю результаты:
            | Результат |
            | yes       |