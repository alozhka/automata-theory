<?php
declare(strict_types=1);

function calculateCircleArea(float $radius): float {
    return M_PI * $radius * $radius;
}

echo "Введите радиус круга: ";
$input = trim(fgets(STDIN));

if (!is_numeric($input)) {
    echo "Введено не числовое значение" . PHP_EOL;
    exit(1);
}

$radius = (float)$input;

if ($radius < 0) {
    echo "Радиус не может быть меньше 0" . PHP_EOL;
    exit(1);
}
$area = calculateCircleArea($radius);
echo "Площадь круга: " . $area . PHP_EOL;