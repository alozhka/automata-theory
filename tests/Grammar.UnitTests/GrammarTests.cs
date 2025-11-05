using Antlr4.Runtime.Misc;

namespace Grammar.UnitTests;

public class GrammarTests
{
    [Theory]
    [MemberData(nameof(ListValidStatements))]
    public void Accepts_valid_statements(string stmt)
    {
        MysticGameScript.ValidateQuery(stmt);
    }

    [Theory]
    [MemberData(nameof(ListInvalidStatements))]
    public void Rejects_invalid_statements(string stmt)
    {
        Assert.Throws<ParseCanceledException>(() => MysticGameScript.ValidateQuery(stmt));
    }

    public static TheoryData<string> ListValidStatements()
    {
        return new TheoryData<string>
        {
            // целочисленная переменная
            "dayzint x;",
        };
    }

    public static TheoryData<string> ListInvalidStatements()
    {
        return new TheoryData<string>
        {
            // Отсутствует точка с запятой
            "dayzint x",
        };
    }
}