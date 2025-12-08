using Ast.Expressions;

namespace Ast.Declarations;

public sealed class VariableDeclaration : Declaration
{
    public VariableDeclaration(string name, Runtime.ValueType type, Expression? value)
    {
        Name = name;
        Value = value;
        Type = type;
    }

    public string Name { get; }

    public Expression? Value { get; }

    public Runtime.ValueType Type { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}