#include <gtest/gtest.h>
#include <fstream>
#include <sstream>
#include "FileUtil.h"

class FileUtilTest : public ::testing::Test {
protected:
    void SetUp() override {
        testFileName = "test_file.txt";
    }

    void TearDown() override {
        std::remove(testFileName.c_str());
    }

    std::string testFileName;
};

TEST_F(FileUtilTest, AddLineNumbers_ValidFile) {
    std::ofstream testFile(testFileName);
    testFile << "first line\n";
    testFile << "second line\n";
    testFile << "third line";
    testFile.close();

    ASSERT_NO_THROW(FileUtil::AddLineNumbers(testFileName));

    std::ifstream resultFile(testFileName);
    std::string line;

    std::getline(resultFile, line);
    EXPECT_EQ(line, "1 first line");

    std::getline(resultFile, line);
    EXPECT_EQ(line, "2 second line");

    std::getline(resultFile, line);
    EXPECT_EQ(line, "3 third line");

    resultFile.close();
}

TEST_F(FileUtilTest, AddLineNumbers_NonExistentFile) {
    EXPECT_THROW(FileUtil::AddLineNumbers("non_existent_file.txt"), std::runtime_error);
}

TEST_F(FileUtilTest, AddLineNumbers_EmptyFile) {
    std::ofstream testFile(testFileName);
    testFile.close();

    ASSERT_NO_THROW(FileUtil::AddLineNumbers(testFileName));

    std::ifstream resultFile(testFileName);
    std::string content((std::istreambuf_iterator<char>(resultFile)),
                        std::istreambuf_iterator<char>());
    resultFile.close();

    EXPECT_TRUE(content.empty());
}