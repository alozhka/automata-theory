using Antlr4.Runtime;

namespace Grammar.Grammars;

public static class MysticGameScriptGrammar
{
    public static void ValidateQuery(string statement)
    {
        AntlrInputStream stream = new(statement);
        
    }
}