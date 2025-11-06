using Antlr4.Runtime;

using Grammar.Listener;

using MysticGameScript;

namespace Grammar;

public static class MysticGameScript
{
    public static void ValidateQuery(string statement)
    {
        AntlrInputStream stream = new(statement);
        MysticGameScriptLexer lexer = new(stream);
        CommonTokenStream tokenStream = new(lexer);
        MysticGameScriptParser parser = new(tokenStream);

        parser.RemoveErrorListeners();
        parser.AddErrorListener(new ThrowingErrorListener());

        parser.ErrorHandler = new BailErrorStrategy();
        parser.program();
    }
}