using Ast.Attributes;
using Ast.Expressions;

namespace Ast.Declarations;

public sealed class VariableDeclaration : AbstractVariableDeclaration
{
    private AstAttribute<AbstractTypeDeclaration?> _declaredType;

    public VariableDeclaration(string name, string declaredTypeName, Expression? value)
        : base(name)
    {
        Value = value;
        DeclaredTypeName = declaredTypeName;
    }

    public Expression? Value { get; }

    public string DeclaredTypeName { get; }

    public AbstractTypeDeclaration? DeclaredType
    {
        get => _declaredType.Get();
        set => _declaredType.Set(value);
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}