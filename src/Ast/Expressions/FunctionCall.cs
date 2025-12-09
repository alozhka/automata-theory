using Ast.Attributes;
using Ast.Declarations;

namespace Ast.Expressions;

public class FunctionCall : Expression
{
    private AstAttribute<AbstractFunctionDeclaration> _function;

    public FunctionCall(string functionName, List<Expression> arguments)
    {
        FunctionName = functionName;
        Arguments = arguments;
    }

    public string FunctionName { get; }

    public List<Expression> Arguments { get; }

    public AbstractFunctionDeclaration Function
    {
        get => _function.Get();
        set => _function.Set(value);
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}