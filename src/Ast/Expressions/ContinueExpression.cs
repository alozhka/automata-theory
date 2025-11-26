namespace Ast.Expressions;

public class ContinueExpression : Expression
{
    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}