using Ast.Expressions;

namespace Ast.Declarations;

public sealed class ConstantDeclaration : Declaration
{
    public ConstantDeclaration(string name, Runtime.ValueType type, Expression value)
    {
        Name = name;
        Type = type;
        Value = value;
    }

    public string Name { get; }

    public Runtime.ValueType Type { get; }

    public Expression Value { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}