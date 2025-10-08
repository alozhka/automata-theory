<?php
declare(strict_types=1);

echo "Введите строку: ";
$input = trim(fgets(STDIN));
$reversed = strrev($input);
echo "Перевернутая строка: " . $reversed . PHP_EOL;