<?php
declare(strict_types=1);

function getGCD(int $a, int $b): int 
{
    $a = abs($a);
    $b = abs($b);
    
    while ($b != 0) {
        $temp = $b;
        $b = $a % $b;
        $a = $temp;
    }
    
    return $a;
}

echo "Введите первое число: ";
$firstNumber = trim(fgets(STDIN));

if (!is_numeric($firstNumber)) {
    echo "Введено не числовое значение" . PHP_EOL;
    exit(1);
}

echo "Введите второе число: ";
$secondNumber = trim(fgets(STDIN));

if (!is_numeric($secondNumber)) {
    echo "Введено не числовое значение" . PHP_EOL;
    exit(1);
}
$firstNumber = (int)$firstNumber;
$secondNumber = (int)$secondNumber;

echo "НОД: " . getGCD($firstNumber, $secondNumber) . PHP_EOL;