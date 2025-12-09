using Ast;
using Ast.Declarations;
using Ast.Expressions;

namespace Semantics.Passes;

/// <summary>
/// Базовый класс для проходов по AST с целью вычисления атрибутов и семантических проверок.
/// </summary>
public abstract class AbstractPass : IAstVisitor
{
    public virtual void Visit(LiteralExpression e)
    {
    }

    public virtual void Visit(UnaryOperationExpression e)
    {
        e.Operand.Accept(this);
    }

    public virtual void Visit(VariableExpression e)
    {
    }

    public virtual void Visit(ContinueExpression e)
    {
    }

    public virtual void Visit(BreakExpression e)
    {
    }

    public virtual void Visit(VariableScopeExpression e)
    {
        foreach (VariableDeclaration variable in e.Variables)
        {
            variable.Accept(this);
        }

        e.Expression.Accept(this);
    }

    public virtual void Visit(IfExpression e)
    {
        e.Condition.Accept(this);

        foreach (AstNode node in e.ThenBranch)
        {
            node.Accept(this);
        }
    }

    public virtual void Visit(ReturnExpression e)
    {
        e.Value.Accept(this);
    }

    public virtual void Visit(ConstantDeclaration d)
    {
        d.Value.Accept(this);
    }

    public virtual void Visit(RaidExpression e)
    {
    }

    public virtual void Visit(ExodusExpression e)
    {
        e.Value.Accept(this);
    }

    public virtual void Visit(BinaryOperationExpression e)
    {
        e.Left.Accept(this);
        e.Right.Accept(this);
    }

    public virtual void Visit(FunctionCall e)
    {
        foreach (Expression argument in e.Arguments)
        {
            argument.Accept(this);
        }
    }

    public virtual void Visit(AssignmentExpression e)
    {
        e.Value.Accept(this);
    }

    public virtual void Visit(IfElseExpression e)
    {
        e.Condition.Accept(this);

        foreach (AstNode node in e.ThenBranch)
        {
            node.Accept(this);
        }

        foreach (AstNode node in e.ElseBranch)
        {
            node.Accept(this);
        }
    }

    public virtual void Visit(WhileLoopExpression e)
    {
        e.Condition.Accept(this);
        foreach (AstNode node in e.ThenBranch)
        {
            node.Accept(this);
        }
    }

    public virtual void Visit(ForLoopExpression e)
    {
        e.StartValue.Accept(this);
        e.EndCondition.Accept(this);

        if (e.StepExpression != null)
        {
            e.StepExpression.Accept(this);
        }

        foreach (AstNode node in e.Body)
        {
            node.Accept(this);
        }
    }

    public virtual void Visit(VariableDeclaration d)
    {
        if (d.Value != null)
        {
            d.Value.Accept(this);
        }
    }

    public virtual void Visit(FunctionDeclaration d)
    {
        foreach (AstNode node in d.Body)
        {
            node.Accept(this);
        }
    }

    public virtual void Visit(ParameterDeclaration d)
    {
    }
}