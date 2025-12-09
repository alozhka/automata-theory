namespace Ast.Expressions;

public sealed class ForLoopExpression : Expression
{
    public ForLoopExpression(
        string typeName,
        string iteratorName,
        Expression startValue,
        Expression endCondition,
        Expression? stepValue,
        List<AstNode> body
    )
    {
        TypeName = typeName;
        IteratorName = iteratorName;
        StartValue = startValue;
        EndCondition = endCondition;
        StepExpression = stepValue;
        Body = body;
    }

    public string TypeName { get; }

    public string IteratorName { get; }

    public Expression StartValue { get; }

    public Expression EndCondition { get; }

    public Expression? StepExpression { get; }

    public List<AstNode> Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}