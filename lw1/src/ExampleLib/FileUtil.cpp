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
        lines.push_back(line);
    }
    if (inputFile.bad()) {
        throw std::runtime_error("Error reading file");
    }
    inputFile.close();

    std::ofstream outputFile(path, std::ios::trunc);
    for (int i = 1; i <= lines.size(); i++)
    {
        outputFile << i << ". " << lines[i - 1] << '\n';
    }
    outputFile.close();
}