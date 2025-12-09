using Ast.Declarations;
using Ast.Expressions;
using Semantics.Exceptions;

namespace Semantics.Passes;

/// <summary>
/// Проверяет соблюдение контекстно-зависимых правил языка.
/// </summary>
/// <remarks>
/// Контекстно-зависимые правила не могли быть проверены при синтаксическом анализе, поскольку синтаксический анализатор
///  разбирает контекстно-свободную грамматику.
/// </remarks>
public sealed class CheckContextSensitiveRulesPass : AbstractPass
{
    // Стек контекстов выражений используется для проверки контекстно-зависимых правил.
    private readonly Stack<ExpressionContext> _expressionContextStack;

    public CheckContextSensitiveRulesPass()
    {
        _expressionContextStack = [];
        _expressionContextStack.Push(ExpressionContext.Default);
    }

    private enum ExpressionContext
    {
        Default,
        InsideLoop,
    }

    /// <summary>
    /// Проверяет корректность программы с точки зрения использования функций.
    /// </summary>
    /// <exception cref="InvalidFunctionCallException">Бросается при неправильном вызове функций.</exception>
    public override void Visit(FunctionCall e)
    {
        base.Visit(e);

        if (e.Arguments.Count != e.Function.Parameters.Count)
        {
            throw new InvalidFunctionCallException(
                $"Function {e.FunctionName} requires {e.Function.Parameters.Count} arguments, got {e.Arguments.Count}"
            );
        }
    }

    public override void Visit(FunctionDeclaration d)
    {
        _expressionContextStack.Push(ExpressionContext.Default);
        try
        {
            base.Visit(d);
        }
        finally
        {
            _expressionContextStack.Pop();
        }
    }

    public override void Visit(WhileLoopExpression e)
    {
        _expressionContextStack.Push(ExpressionContext.InsideLoop);
        try
        {
            base.Visit(e);
        }
        finally
        {
            _expressionContextStack.Pop();
        }
    }

    public override void Visit(ForLoopExpression e)
    {
        _expressionContextStack.Push(ExpressionContext.InsideLoop);
        try
        {
            base.Visit(e);
        }
        finally
        {
            _expressionContextStack.Pop();
        }
    }

    public override void Visit(BreakExpression e)
    {
        base.Visit(e);

        if (_expressionContextStack.Peek() != ExpressionContext.InsideLoop)
        {
            throw new InvalidExpressionException("The \"breakout\" expression is allowed only inside the loop");
        }
    }

    public override void Visit(ContinueExpression e)
    {
        base.Visit(e);

        if (_expressionContextStack.Peek() != ExpressionContext.InsideLoop)
        {
            throw new InvalidExpressionException("The \"contra\" expression is allowed only inside the loop");
        }
    }
}