#include <iostream>
#include <string>
#include <algorithm>

std::string ReverseString(const std::string& str) 
{
    return std::string(str.rbegin(), str.rend());
}

int main() 
{
    std::string str;
    std::cout << "Введите строку: ";
    getline(std::cin, str);

    std::cout << "Перевернутая строка: " << ReverseString(str) << std::endl;
    return 0;
}