using Ast.Attributes;
using Ast.Declarations;
using Ast.Expressions;

namespace Ast.Statements;

public sealed class AssignmentStatement : Expression
{
    private AstAttribute<AbstractVariableDeclaration> _variable;

    public AssignmentStatement(string name, Expression value)
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