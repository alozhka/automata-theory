using ValueType = Runtime.ValueType;

namespace Ast.Declarations;

public class FunctionDeclaration : AstNode
{
    public FunctionDeclaration(string name, List<Parameter> parameters, ValueType? returnType, List<AstNode> body)
    {
        Name = name;
        Parameters = parameters;
        ReturnType = returnType;
        Body = body;
    }

    public string Name { get; }

    public ValueType? ReturnType { get; }

    public List<Parameter> Parameters { get; }

    public List<AstNode> Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}