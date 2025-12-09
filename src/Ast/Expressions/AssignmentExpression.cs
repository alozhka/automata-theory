using Ast.Attributes;
using Ast.Declarations;

namespace Ast.Expressions;

public sealed class AssignmentExpression : Expression
{
    private AstAttribute<AbstractVariableDeclaration> _variable;

    public AssignmentExpression(string name, Expression value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; }

    public Expression Value { get; }

    public AbstractVariableDeclaration Variable
    {
        get => _variable.Get();
        set => _variable.Set(value);
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}