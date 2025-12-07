namespace Ast.Expressions;

public sealed class ForLoopExpression : Expression
{
    public ForLoopExpression(
        Runtime.ValueType iteratorType,
        string iteratorName,
        Expression startValue,
        Expression endCondition,
        Expression? stepValue,
        List<AstNode> body
    )
    {
        IteratorType = iteratorType;
        IteratorName = iteratorName;
        StartValue = startValue;
        EndCondition = endCondition;
        StepExpression = stepValue;
        Body = body;
    }

    public Runtime.ValueType IteratorType { get; }

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