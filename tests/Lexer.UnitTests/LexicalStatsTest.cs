using SqlLexer;

namespace SqlLexer.UnitTests;

public class LexicalStatsTest
{
	[Theory]
	[MemberData(nameof(GetLexicalStatsFromFileData))]
	public void Can_collect_lexical_stats_from_file(string sourceCode, string expectedStats)
	{
		string tempFile = Path.GetTempFileName();
		try
		{
			File.WriteAllText(tempFile, sourceCode);

			string actualStats = LexicalStats.CollectFromFile(tempFile);

			Assert.Equal(expectedStats, actualStats);
		}
		finally
		{
			if (File.Exists(tempFile))
				File.Delete(tempFile);
		}
	}

	public static TheoryData<string, string> GetLexicalStatsFromFileData()
	{
		return new TheoryData<string, string>
		{
			{
@"
funkotron fibonacci(dayzint n): dayzint {
    iffy (n <= 1) {
        returnal n;
    }
    returnal fibonacci(n - 1) + fibonacci(n - 2);
}

funkotron main(): ghost {
    exodus(""Enter Fibonacci sequence length: "");
    dayzint length;
    raid(length);
    
    iffy (length <= 0) {
        exodusln(""Invalid length"");
        returnal;
    }
    
    exodusln(""Fibonacci sequence:"");
    forza (dayzint i = 0; i < length; i = i + 1) {
        exodus(fibonacci(i) + "" "");
    }
    exodusln("""");
}",
@"keywords: 19
identifier: 19
number literals: 6
string literals: 5
operators: 10
other lexemes: 52"
			},
			{
@"
funkotron convertCurrency(fallout amount, fallout exchangeRate): fallout {
    returnal amount * exchangeRate;
}

funkotron main(): ghost {
    fallout usdAmount = 100.0;
    fallout exchangeRate = 0.85;
    
    fallout eurAmount = convertCurrency(usdAmount, exchangeRate);
    
    exodusln(""Currency Converter"");
    exodusln(""USD: "" + usdAmount);
    exodusln(""Exchange Rate: "" + exchangeRate);
    exodusln(""EUR: "" + eurAmount);
    
    iffy (usdAmount < 0 || exchangeRate <= 0) {
        exodusln(""Invalid input"");
        returnal;
    }
}",
@"keywords: 17
identifier: 17
number literals: 4
string literals: 5
operators: 10
other lexemes: 38"
			}
		};
	}
}