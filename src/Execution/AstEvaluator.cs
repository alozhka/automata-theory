using Ast;
using Ast.Declarations;
using Ast.Expressions;
using Lexer;

namespace Execution;

public class AstEvaluator(Context context, IEnvironment environment) : IAstVisitor
{
    private readonly Stack<double> _values = [];

    private readonly double _trueToDouble = 1.0;

    private readonly double _falseToDouble = 0.0;

    public double Evaluate(AstNode node)
    {
        if (_values.Count > 0)
        {
            throw new InvalidOperationException(
                $"Evaluation stack must be empty, but contains {_values.Count} values: {string.Join(", ", _values)}"
            );
        }

        node.Accept(this);

        return _values.Count switch
        {
            0 => throw new InvalidOperationException(
                "Evaluator logical error: the stack has no evaluation result"
            ),
            > 1 => throw new InvalidOperationException(
                $"Evaluator logical error: expected 1 value, got {_values.Count} values: {string.Join(", ", _values)}"
            ),
            _ => _values.Pop(),
        };
    }

    public void Visit(ForLoopExpression e)
    {
        context.PushScope(new Scope());
        try
        {
            e.StartValue.Accept(this);
            double iteratorValue = _values.Pop();
            context.DefineVariable(e.IteratorName, iteratorValue);

            while (true)
            {
                e.EndCondition.Accept(this);
                double conditionResult = _values.Pop();

                if (Numbers.AreEqual(0.0, conditionResult))
                {
                    break;
                }

                foreach (AstNode statement in e.Body)
                {
                    statement.Accept(this);
                    if (_values.Count > 0 && statement is AssignmentExpression)
                    {
                        _values.Pop();
                    }
                }

                if (e.StepValue != null)
                {
                    e.StepValue.Accept(this);
                }

                if (_values.Count > 0)
                {
                    _values.Pop();
                }
            }

            _values.Push(0.0);
        }
        finally
        {
            context.PopScope();
        }
    }

    public void Visit(BinaryOperationExpression e)
    {
        e.Left.Accept(this);
        e.Right.Accept(this);
        double right = _values.Pop();
        double left = _values.Pop();

        switch (e.Operation)
        {
            case BinaryOperation.Plus:
                _values.Push(left + right);
                break;
            case BinaryOperation.Minus:
                _values.Push(left - right);
                break;
            case BinaryOperation.Multiply:
                _values.Push(left * right);
                break;
            case BinaryOperation.Divide:
                _values.Push(left / right);
                break;
            case BinaryOperation.Modulo:
                _values.Push(left % right);
                break;
            case BinaryOperation.LessThan:
                _values.Push(left < right ? _trueToDouble : _falseToDouble);
                break;
            case BinaryOperation.GreaterThan:
                _values.Push(left > right ? _trueToDouble : _falseToDouble);
                break;
            case BinaryOperation.LessThanOrEqual:
                _values.Push(left <= right ? _trueToDouble : _falseToDouble);
                break;
            case BinaryOperation.GreaterThanOrEqual:
                _values.Push(left >= right ? _trueToDouble : _falseToDouble);
                break;
            case BinaryOperation.Equal:
                _values.Push(Numbers.AreEqual(left, right) ? _trueToDouble : _falseToDouble);
                break;
            case BinaryOperation.NotEqual:
                _values.Push(Numbers.AreNotEqual(left, right) ? _trueToDouble : _falseToDouble);
                break;
            case BinaryOperation.LogicalAnd:
                _values.Push((Numbers.AreEqual(left, _trueToDouble) && Numbers.AreEqual(right, _trueToDouble)) ? _trueToDouble : _falseToDouble);
                break;
            case BinaryOperation.LogicalOr:
                _values.Push((Numbers.AreEqual(left, _trueToDouble) || Numbers.AreEqual(right, _trueToDouble)) ? _trueToDouble : _falseToDouble);
                break;
            default:
                throw new NotImplementedException($"Unknown binary operation {e.Operation}");
        }
    }

    public void Visit(UnaryOperationExpression e)
    {
        e.Operand.Accept(this);
        switch (e.Operation)
        {
            case UnaryOperation.Minus:
                _values.Push(-_values.Pop());
                break;
            case UnaryOperation.LogicalNot:
                double value = _values.Pop();
                _values.Push(value == 0.0 ? 1.0 : 0.0);
                break;
            default:
                throw new NotImplementedException($"Unknown unary operation {e.Operation}");
        }
    }

    public void Visit(LiteralExpression e)
    {
        _values.Push(e.Value);
    }

    public void Visit(VariableExpression e)
    {
        _values.Push(context.GetValue(e.Name));
    }

    public void Visit(AssignmentExpression e)
    {
        e.Value.Accept(this);
        double value = _values.Pop();
        context.AssignVariable(e.Name, value);
        _values.Push(value);
    }

    public void Visit(VariableScopeExpression e)
    {
        context.PushScope(new Scope());
        try
        {
            foreach (VariableDeclaration variable in e.Variables)
            {
                variable.Accept(this);
                _values.Pop();
            }

            e.Expression.Accept(this);
        }
        finally
        {
            context.PopScope();
        }
    }

    public void Visit(VariableDeclaration d)
    {
        double value = 0;
        if (d.Value != null)
        {
            d.Value.Accept(this);
            value = _values.Pop();
        }

        context.DefineVariable(d.Name, value);
        _values.Push(0.0);
    }

    public void Visit(ConstantDeclaration d)
    {
        d.Value.Accept(this);
        double value = _values.Pop();
        context.DefineConstant(d.Name, value);
        _values.Push(0.0);
    }

    public void Visit(FunctionDeclaration d)
    {
        context.DefineFunction(d);

        _values.Push(_falseToDouble);
    }

    public void Visit(ExodusExpression e)
    {
        e.Value.Accept(this);
        double value = _values.Pop();
        environment.AddResult(value);
        _values.Push(0.0);
    }

    public void Visit(RaidExpression e)
    {
        double value = environment.ReadInput();

        context.AssignVariable(e.Name, value);
        _values.Push(0.0);
    }

    public void Visit(FunctionCall call)
    {
        List<double> argValues = new();
        foreach (Expression arg in call.Arguments)
        {
            arg.Accept(this);
            argValues.Add(_values.Pop());
        }

        if (IsBuiltInFunction(call.FunctionName))
        {
            double result = call.FunctionName.ToLower() switch
            {
                "floor" => Math.Floor(argValues[0]),
                "ceil" => Math.Ceiling(argValues[0]),
                "round" => Math.Round(argValues[0]),
                "abs" => Math.Abs(argValues[0]),
                "max" => argValues.Max(),
                "min" => argValues.Min(),
                _ => throw new Exception($"Unknown built-in function: {call.FunctionName}"),
            };
            _values.Push(result);
        }
        else
        {
            FunctionDeclaration function = context.GetFunction(call.FunctionName);

            foreach (double argValue in Enumerable.Reverse(argValues))
            {
                _values.Push(argValue);
            }

            context.PushScope(new Scope());
            try
            {
                foreach (string parameterName in Enumerable.Reverse(function.Parameters))
                {
                    double value = _values.Pop();
                    context.DefineVariable(parameterName, value);
                }

                double returnValue;

                try
                {
                    foreach (AstNode statement in function.Body)
                    {
                        statement.Accept(this);

                        if (!(statement is ReturnExpression) && _values.Count > 0)
                        {
                            _values.Pop();
                        }
                    }

                    returnValue = 0.0;
                }
                catch (ReturnException ret)
                {
                    returnValue = ret.ReturnValue;
                }

                while (_values.Count > 0)
                {
                    _values.Pop();
                }

                _values.Push(returnValue);
            }
            finally
            {
                context.PopScope();
            }
        }
    }

    public void Visit(IfExpression e)
    {
        e.Condition.Accept(this);

        double conditionValue = _values.Pop();
        bool isTrueCondition = Numbers.AreEqual(_trueToDouble, conditionValue);

        if (isTrueCondition)
        {
            foreach (AstNode statement in e.ThenBranch)
            {
                statement.Accept(this);
                if (_values.Count > 0 && !(statement is ReturnExpression))
                {
                    _values.Pop();
                }
            }
        }
    }

    public void Visit(WhileLoopExpression e)
    {
        context.PushScope(new Scope());
        try
        {
            while (true)
            {
                e.Condition.Accept(this);
                double conditionValue = _values.Pop();

                if (Numbers.AreEqual(0.0, conditionValue))
                {
                    break;
                }

                foreach (AstNode statement in e.ThenBranch)
                {
                    statement.Accept(this);

                    if (_values.Count > 0 && !(statement is ReturnExpression))
                    {
                        _values.Pop();
                    }
                }
            }

            _values.Push(0.0);
        }
        finally
        {
            context.PopScope();
        }
    }

    public void Visit(ReturnExpression e)
    {
        e.Value.Accept(this);
        double returnValue = _values.Pop();
        throw new ReturnException(returnValue);
    }

    public void Visit(IfElseExpression e)
    {
        e.Condition.Accept(this);

        double conditionValue = _values.Pop();
        bool isTrueCondition = Numbers.AreEqual(_trueToDouble, conditionValue);
        if (isTrueCondition)
        {
            foreach (AstNode statement in e.ThenBranch)
            {
                statement.Accept(this);
                if (_values.Count > 0 && !(statement is ReturnExpression))
                {
                    _values.Pop();
                }
            }
        }
        else
        {
            foreach (AstNode statement in e.ElseBranch)
            {
                statement.Accept(this);
                if (_values.Count > 0 && !(statement is ReturnExpression))
                {
                    _values.Pop();
                }
            }
        }
    }

    private bool IsBuiltInFunction(string functionName)
    {
        string lowerName = functionName.ToLower();
        return lowerName == "floor" || lowerName == "ceil" || lowerName == "round" ||
               lowerName == "abs" || lowerName == "max" || lowerName == "min";
    }
}