#include <iostream>
#include <cmath>

float GetAreaCircleByRadius(float radius) 
{
    return radius * radius * M_PI;
}

int main() 
{
    float radius = 0;
    std::cout << "Введите радиус: ";
    std::cin >> radius;

    std::cout << "Площадь круга: " << GetAreaCircleByRadius(radius) << std::endl;
    return 0;
}