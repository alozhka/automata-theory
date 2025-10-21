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

class FormatRomanParameterizedTest : public ::testing::TestWithParam<std::pair<int, std::string>> {
};

TEST_P(FormatRomanParameterizedTest, FormatRoman_ValidNumbers) {
    const auto& [value, expected] = GetParam();
    EXPECT_EQ(TextUtil::FormatRoman(value), expected);
}

INSTANTIATE_TEST_SUITE_P(
        FormatRomanValidValues,
        FormatRomanParameterizedTest,
        ::testing::Values(
                std::make_pair(1, "I"),
                std::make_pair(4, "IV"),
                std::make_pair(5, "V"),
                std::make_pair(7, "VII"),
                std::make_pair(9, "IX"),
                std::make_pair(10, "X"),
                std::make_pair(14, "XIV"),
                std::make_pair(19, "XIX"),
                std::make_pair(40, "XL"),
                std::make_pair(44, "XLIV"),
                std::make_pair(49, "XLIX"),
                std::make_pair(50, "L"),
                std::make_pair(90, "XC"),
                std::make_pair(99, "XCIX"),
                std::make_pair(100, "C"),
                std::make_pair(400, "CD"),
                std::make_pair(444, "CDXLIV"),
                std::make_pair(500, "D"),
                std::make_pair(900, "CM"),
                std::make_pair(999, "CMXCIX"),
                std::make_pair(1000, "M"),
                std::make_pair(1984, "MCMLXXXIV"),
                std::make_pair(2000, "MM"),
                std::make_pair(2023, "MMXXIII"),
                std::make_pair(2025, "MMXXV"),
                std::make_pair(3000, "MMM")
        )
);

class FormatRomanInvalidTest : public ::testing::TestWithParam<int> {
};

TEST_P(FormatRomanInvalidTest, FormatRoman_InvalidNumbers) {
    EXPECT_THROW(TextUtil::FormatRoman(GetParam()), std::runtime_error);
}

INSTANTIATE_TEST_SUITE_P(
        FormatRomanInvalidValues,
        FormatRomanInvalidTest,
        ::testing::Values(-1, -100, 3001, 4000)
);

TEST(TextUtilTest, FormatRoman_BoundaryValues) {
    EXPECT_EQ(TextUtil::FormatRoman(0), "");
    EXPECT_EQ(TextUtil::FormatRoman(1), "I");
    EXPECT_EQ(TextUtil::FormatRoman(3000), "MMM");
    EXPECT_THROW(TextUtil::FormatRoman(3001), std::runtime_error);
    EXPECT_THROW(TextUtil::FormatRoman(-1), std::runtime_error);
}

TEST(TextUtilTest, FormatRoman_SpecificExamples) {
    EXPECT_EQ(TextUtil::FormatRoman(7), "VII");
    EXPECT_EQ(TextUtil::FormatRoman(2025), "MMXXV");
}