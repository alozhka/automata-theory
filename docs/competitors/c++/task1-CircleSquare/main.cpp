#include <iostream> //почему с #
#include <cmath> // к чему отностится include

float GetAreaCircleByRadius(float radius) 
{
    return radius * radius * M_PI;
}

int main() 
{
    float radius = 0;
    std::cout << "Введите радиус: ";
    std::cin >> radius;
    if (radius < 0) 
    {
        std::cout << "Радиус не может быть меньше нуля" << std::endl;
        return 0;
    }

    std::cout << "Площадь круга: " << GetAreaCircleByRadius(radius) << std::endl;
    return 0;
}