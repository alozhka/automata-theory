using Ast.Attributes;
using Ast.Expressions;

namespace Ast.Declarations;

public sealed class ConstantDeclaration : AbstractVariableDeclaration
{
    private AstAttribute<AbstractTypeDeclaration?> _declaredType;

    public ConstantDeclaration(string name, string typeName, Expression value)
        : base(name)
    {
        TypeName = typeName;
        Value = value;
    }

    public string TypeName { get; }

    public Expression Value { get; }

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