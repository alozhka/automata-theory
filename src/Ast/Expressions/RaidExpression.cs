namespace Ast.Expressions;

public sealed class RaidExpression : Expression
{
    public RaidExpression(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}