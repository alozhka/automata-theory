using Ast.Attributes;
using Ast.Declarations;
using Ast.Expressions;

namespace Ast.Statements;

public sealed class RaidStatement : Expression
{
    private AstAttribute<AbstractVariableDeclaration> _variable;

    public RaidStatement(string name)
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