namespace Ast.Expressions;

public sealed class ExodusExpression : Expression
{
    public ExodusExpression(Expression value)
    {
        Value = value;
    }

    public Expression Value { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}