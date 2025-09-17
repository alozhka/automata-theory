#include <gtest/gtest.h>
#include <fstream>
#include <sstream>
#include "../src/ExampleLib/FileUtil.h"
#include "../src/ExampleLib/TextUtil.h"

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
    EXPECT_EQ(line, "1. first line");

    std::getline(resultFile, line);
    EXPECT_EQ(line, "2. second line");

    std::getline(resultFile, line);
    EXPECT_EQ(line, "3. third line");

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

TEST(TextUtilTest, ParseRoman_ValidParse)
{
    EXPECT_EQ(TextUtil::ParseRoman("VII"), 7);
}

TEST(TextUtilTest, ParseRoman_InvalidNumber_NotRimsk)
{
    EXPECT_THROW(TextUtil::ParseRoman("-"), std::runtime_error);
}

TEST(TextUtilTest, ParseRoman_InvalidNumber_EmptyString)
{
    EXPECT_THROW(TextUtil::ParseRoman(""), std::runtime_error);
}

TEST(TextUtilTest, ParseRoman_InvalidNumber_Bigger3000)
{
    EXPECT_THROW(TextUtil::ParseRoman("MMMI"), std::runtime_error);
}

TEST(TextUtilTest, ParseRoman_VvalidNumber_Equal3000)
{
    EXPECT_EQ(TextUtil::ParseRoman("MMM"), 3000);
}

TEST(TextUtilTest, ParseRoman_VvalidNumber_Equal1)
{
    EXPECT_EQ(TextUtil::ParseRoman("I"), 1);
}