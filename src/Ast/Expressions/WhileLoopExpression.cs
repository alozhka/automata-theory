namespace Ast.Expressions;

public sealed class WhileLoopExpression : Expression
{
    public WhileLoopExpression(Expression condition, List<AstNode> thenBranch)
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