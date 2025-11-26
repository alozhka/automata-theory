namespace Ast.Expressions;

public class BreakExpression : Expression
{
    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}