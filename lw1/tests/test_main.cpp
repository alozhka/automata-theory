#include <gtest/gtest.h>
#include <fstream>
#include <sstream>
#include <filesystem>
#include "../src/ExampleLib/FileUtil.h"
#include "../src/ExampleLib/TextUtil.h"

class FileUtilTest : public ::testing::Test {
protected:
    void SetUp() override {
        testFileName = "test_file.txt";
    }

    void TearDown() override {
        if (std::filesystem::exists(testFileName)) {
            std::filesystem::remove(testFileName);
        }
    }

    std::string testFileName;

    static void AssertFileContent(const std::string& filename, const std::vector<std::string>& expectedLines) {
        std::ifstream file(filename);
        ASSERT_TRUE(file.is_open()) << "Cannot open file: " << filename;

        std::string line;
        for (size_t i = 0; i < expectedLines.size(); ++i) {
            ASSERT_TRUE(std::getline(file, line)) << "Expected more lines in file";
            EXPECT_EQ(line, expectedLines[i]) << "Line " << (i + 1) << " mismatch";
        }

        EXPECT_FALSE(std::getline(file, line)) << "Unexpected extra lines in file";
        file.close();
    }

    void CreateTestFile(const std::vector<std::string>& lines) {
        std::ofstream file(testFileName);
        ASSERT_TRUE(file.is_open()) << "Cannot create test file: " << testFileName;

        for (size_t i = 0; i < lines.size(); ++i) {
            file << lines[i];
            if (i < lines.size() - 1) {
                file << "\n";
            }
        }
        file.close();
    }
};

TEST_F(FileUtilTest, AddLineNumbers_ValidFile) {
    CreateTestFile({"first line", "second line", "third line"});

    ASSERT_NO_THROW(FileUtil::AddLineNumbers(testFileName));

    AssertFileContent(testFileName, {
            "1. first line",
            "2. second line",
            "3. third line"
    });
}

TEST_F(FileUtilTest, AddLineNumbers_NonExistentFile) {
    EXPECT_THROW(FileUtil::AddLineNumbers("non_existent_file.txt"), std::runtime_error);
}

TEST_F(FileUtilTest, AddLineNumbers_EmptyFile) {
    CreateTestFile({});

    ASSERT_NO_THROW(FileUtil::AddLineNumbers(testFileName));

    std::ifstream resultFile(testFileName);
    std::string content((std::istreambuf_iterator<char>(resultFile)),
                        std::istreambuf_iterator<char>());
    resultFile.close();

    EXPECT_TRUE(content.empty());
}

TEST_F(FileUtilTest, AddLineNumbers_SingleLine) {
    CreateTestFile({"single line"});

    ASSERT_NO_THROW(FileUtil::AddLineNumbers(testFileName));

    AssertFileContent(testFileName, {"1. single line"});
}

class TextUtilParameterizedTest : public ::testing::TestWithParam<std::pair<std::string, int>> {
};

TEST_P(TextUtilParameterizedTest, ParseRoman_ValidNumbers) {
    const auto& [roman, expected] = GetParam();
    EXPECT_EQ(TextUtil::ParseRoman(roman), expected);
}

INSTANTIATE_TEST_SUITE_P(
        ValidRomanNumerals,
        TextUtilParameterizedTest,
        ::testing::Values(
                std::make_pair("I", 1),
                std::make_pair("V", 5),
                std::make_pair("X", 10),
                std::make_pair("L", 50),
                std::make_pair("C", 100),
                std::make_pair("D", 500),
                std::make_pair("M", 1000),
                std::make_pair("IV", 4),
                std::make_pair("IX", 9),
                std::make_pair("XL", 40),
                std::make_pair("XC", 90),
                std::make_pair("CD", 400),
                std::make_pair("CM", 900),
                std::make_pair("MMM", 3000)
        )
);

class TextUtilInvalidTest : public ::testing::TestWithParam<std::string> {
};

TEST_P(TextUtilInvalidTest, ParseRoman_InvalidNumbers) {
    EXPECT_THROW(TextUtil::ParseRoman(GetParam()), std::runtime_error);
}

INSTANTIATE_TEST_SUITE_P(
        InvalidRomanNumerals,
        TextUtilInvalidTest,
        ::testing::Values(
                "",
                "-",
                "MMMI",
                "ABC"
        )
);

TEST(TextUtilTest, ParseRoman_BoundaryValues) {
    EXPECT_EQ(TextUtil::ParseRoman("I"), 1);
    EXPECT_EQ(TextUtil::ParseRoman("MMM"), 3000);
    EXPECT_THROW(TextUtil::ParseRoman("MMMI"), std::runtime_error);
}