using Ast.Expressions;

namespace Ast.Statements;

public class BreakStatement : Expression
{
    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}