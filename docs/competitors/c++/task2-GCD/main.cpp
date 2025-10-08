#include <iostream>
#include <cmath>

int GetGCD(int a, int b) 
{
    a = abs(a);
    b = abs(b);
    
    while (b != 0) 
    {
        int temp = b;
        b = a % b;
        a = temp;
    }
    return a;
}

int main() 
{
    int a = 0;
    int b = 0;
    std::cout << "Введите первое число: ";
    std::cin >> a;
    std::cout << "Введите второе число: ";
    std::cin >> b;

    std::cout << "НОД: " << GetGCD(a, b) << std::endl;
    return 0;
}