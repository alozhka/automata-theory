namespace Lexer.UnitTests;

public class LexerStatsTest
{
    [Theory]
    [MemberData(nameof(GetLexicalStatsFromFileData))]
    public void Can_collect_lexical_stats_from_file(string sourceCode, string expectedStats)
    {
        string tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, sourceCode);

            string actualStats = LexerStats.CollectFromFile(tempFile);

            Assert.Equal(expectedStats, actualStats);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    public static TheoryData<string, string> GetLexicalStatsFromFileData()
    {
        return new TheoryData<string, string>
        {
            {
                """
                funkotron fibonacci(dayzint n): dayzint {
                    iffy (n <= 1) {
                        returnal n;
                    }
                    returnal fibonacci(n - 1) + fibonacci(n - 2);
                }

                funkotron main(): ghost {
                    exodus("Enter Fibonacci sequence length: ");
                    dayzint length;
                    raid(length);
                    
                    iffy (length <= 0) {
                        exodus("Invalid length");
                        returnal;
                    }
                    
                    exodus("Fibonacci sequence:");
                    forza (dayzint i = 0; i < length; i = i + 1) {
                        exodus(fibonacci(i) + " ");
                    }
                    exodus("");
                }
                """,
                """
                keywords: 18
                identifier: 20
                number literals: 6
                string literals: 5
                operators: 10
                other lexemes: 52
                """
            },
            {
                """
                funkotron convertCurrency(fallout amount, fallout exchangeRate): fallout {
                    returnal amount * exchangeRate;
                }

                funkotron main(): ghost {
                    fallout usdAmount = 100.0;
                    fallout exchangeRate = 0.85;
                    
                    fallout eurAmount = convertCurrency(usdAmount, exchangeRate);
                    
                    exodus("Currency Converter");
                    exodus("USD: " + usdAmount);
                    exodus("Exchange Rate: " + exchangeRate);
                    exodus("EUR: " + eurAmount);
                    
                    iffy (usdAmount < 0 || exchangeRate <= 0) {
                        exodus("Invalid input");
                        returnal;
                    }
                }
                """,
                @"keywords: 16
identifier: 18
number literals: 4
string literals: 5
operators: 10
other lexemes: 38"
            },
        };
    }
}