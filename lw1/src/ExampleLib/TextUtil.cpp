#include "TextUtil.h"

#include <map>
#include <stdexcept>
#include <vector>

std::string TextUtil::FormatRoman(int value)
{
    if (value < 0 || value > 3000) {
        throw std::runtime_error("Number must be from 0 to 3000");
    }

    if (value == 0) {
        return "";
    }

    std::vector<std::pair<int, std::string>> values = {
            {1000, "M"},
            {900, "CM"},
            {500, "D"},
            {400, "CD"},
            {100, "C"},
            {90, "XC"},
            {50, "L"},
            {40, "XL"},
            {10, "X"},
            {9, "IX"},
            {5, "V"},
            {4, "IV"},
            {1, "I"}
    };

    std::string result;
    int remaining = value;

    for (const auto& [num, roman] : values) {
        while (remaining >= num) {
            result += roman;
            remaining -= num;
        }
    }

    return result;
}