using Ast.Expressions;

namespace Ast.Statements;

public sealed class WhileLoopStatement : Expression
{
    public WhileLoopStatement(Expression condition, List<AstNode> thenBranch)
    {
        Condition = condition;
        ThenBranch = thenBranch;
    }

    public Expression Condition { get; }

    public List<AstNode> ThenBranch { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}