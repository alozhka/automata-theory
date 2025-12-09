using Ast.Attributes;

namespace Ast.Declarations;

public sealed class FunctionDeclaration : AbstractFunctionDeclaration
{
    private AstAttribute<AbstractTypeDeclaration?> _declaredType;

    public FunctionDeclaration(
        string name,
        IReadOnlyList<ParameterDeclaration> parameters,
        string? declaredTypeName,
        List<AstNode> body
    )
        : base(name, parameters)
    {
        DeclaredTypeName = declaredTypeName;
        Body = body;
    }

    public AbstractTypeDeclaration? DeclaredType
    {
        get => _declaredType.Get();
        set => _declaredType.Set(value);
    }

    public string? DeclaredTypeName { get; }

    public List<AstNode> Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}