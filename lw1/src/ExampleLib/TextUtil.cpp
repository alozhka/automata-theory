#include "TextUtil.h"

#include <map>
#include <istream>

int TextUtil::ParseRoman(const std::string &text)
{
    if (text.empty()) {
        throw std::runtime_error("Empty string");
    }

    std::map<char, int> roman = {
            {'M', 1000}, {'D', 500}, {'C', 100},
            {'L', 50}, {'X', 10}, {'V', 5}, {'I', 1}
    };

    for (char c : text) {
        if (roman.find(c) == roman.end()) {
            throw std::runtime_error("Undefined symbol: " + std::string(1, c));
        }
    }

    int res = 0;
    for (size_t i = 0; i < text.size() - 1; ++i) {
        int current = roman[text[i]];
        int next = roman[text[i + 1]];

        if (current < next) {
            res -= current;
        } else {
            res += current;
        }
    }

    res += roman[text[text.size() - 1]];

    if (res <= 0 || res > 3000) {
        throw std::runtime_error("Number must be from 1 to 3000");
    }

    return res;
}