using Ast.Attributes;
using Ast.Declarations;

namespace Ast.Expressions;

public sealed class RaidExpression : Expression
{
    private AstAttribute<AbstractVariableDeclaration> _variable;

    public RaidExpression(string name)
    {
        Name = name;
    }

    public string Name { get; }

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