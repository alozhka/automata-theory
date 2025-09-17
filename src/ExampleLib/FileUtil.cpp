#include "FileUtil.h"
#include <vector>
#include <istream>
#include <fstream>

void FileUtil::AddLineNumbers(const std::string& path)
{
    std::ifstream inputFile(path);
    if (!inputFile.is_open()) {
        throw std::runtime_error("Error open file");
    }

    std::vector<std::string> lines;
    std::string line;
    while (std::getline(inputFile, line)) {
        if (inputFile.fail() && !inputFile.eof()) {
            throw std::runtime_error("Error read line");
        }
        lines.push_back(line);
    }

    std::ofstream outputFile(path, std::ios::trunc);
    for (int i = 1; i <= lines.size(); i++)
    {
        outputFile << i << " " << lines[i - 1] << std::endl;
    }
    outputFile.close();
}