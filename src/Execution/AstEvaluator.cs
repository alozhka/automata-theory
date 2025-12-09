using Ast;
using Ast.Declarations;
using Ast.Expressions;
using Execution.Exceptions;
using Lexer;
using Runtime;

using ValueType = Runtime.ValueType;

namespace Execution;

public class AstEvaluator(Context context, IEnvironment environment) : IAstVisitor
{
    private readonly Stack<Value> _values = [];

    private readonly int _trueToDouble = 1;

    private readonly int _falseToDouble = 0;

    public Value Evaluate(AstNode node)
    {
        if (_values.Count > 0)
        {
            throw new InvalidOperationException(
                $"Evaluation stack must be empty, but contains {_values.Count} values: {string.Join(", ", _values)}"
            );
        }

        node.Accept(this);

        if (_values.Count == 0)
        {
            return Value.Void;
        }

        return _values.Count switch
        {
            > 1 => throw new InvalidOperationException(
                $"Evaluator logical error: expected 1 value, got {_values.Count} values: {string.Join(", ", _values)}"
            ),
            _ => _values.Pop(),
        };
    }

    public void Visit(ParameterDeclaration d)
    {
    }

    public void Visit(ForLoopExpression e)
    {
        context.PushScope(new Scope());
        try
        {
            e.StartValue.Accept(this);
            Value iteratorValue = _values.Pop();
            context.DefineVariable(e.IteratorName, iteratorValue);

            bool isNotBreaked = true;
            while (isNotBreaked)
            {
                e.EndCondition.Accept(this);
                Value conditionResult = _values.Pop();

                if (conditionResult.GetValueType() != ValueType.Int)
                {
                    throw new Exception($"Condition expression {e.IteratorName} does not support type {conditionResult.GetValueType()}");
                }

                if (Numbers.AreEqual(0.0, conditionResult.AsInt()))
                {
                    break;
                }

                context.PushScope(new Scope());
                try
                {
                    foreach (AstNode statement in e.Body)
                    {
                        try
                        {
                            statement.Accept(this);
                            if (_values.Count > 0)
                            {
                                _values.Pop();
                            }
                        }
                        catch (ContinueException)
                        {
                            if (_values.Count > 0)
                            {
                                _values.Pop();
                            }

                            break;
                        }
                        catch (BreakException)
                        {
                            if (_values.Count > 0)
                            {
                                _values.Pop();
                            }

                            isNotBreaked = false;
                            break;
                        }
                    }
                }
                finally
                {
                    context.PopScope();
                }

                if (e.StepExpression != null)
                {
                    e.StepExpression.Accept(this);
                    if (_values.Count > 0)
                    {
                        _values.Pop();
                    }
                }
            }

            _values.Push(Value.Void);
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
        Value right = _values.Pop();
        Value left = _values.Pop();
        if (left.GetValueType() == ValueType.Int && right.GetValueType() == ValueType.Double)
        {
            left = new Value(Convert.ToDouble(left.AsInt()));
        }
        else if (left.GetValueType() == ValueType.Double && right.GetValueType() == ValueType.Int)
        {
            right = new Value(Convert.ToDouble(right.AsInt()));
        }

        switch (e.Operation)
        {
            case BinaryOperation.Plus:
                if (left.GetValueType() != right.GetValueType())
                {
                    throw new InvalidOperationException(
                        $"Types must be equals! Got {left.GetValueType()} and {right.GetValueType()}"
                    );
                }

                switch (left.GetValueType())
                {
                    case ValueType.Int:
                        _values.Push(new Value(left.AsInt() + right.AsInt()));
                        break;
                    case ValueType.Double:
                        _values.Push(new Value(left.AsDouble() + right.AsDouble()));
                        break;
                    case ValueType.String:
                        _values.Push(new Value(left.AsString() + right.AsString()));
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"Plus operation not supported for type {left.GetValueType()}"
                        );
                }

                break;
            case BinaryOperation.Minus:
                if (left.GetValueType() != right.GetValueType())
                {
                    throw new InvalidOperationException(
                        $"Types must be equals! Got {left.GetValueType()} and {right.GetValueType()}"
                    );
                }

                switch (left.GetValueType())
                {
                    case ValueType.Int:
                        _values.Push(new Value(left.AsInt() - right.AsInt()));
                        break;
                    case ValueType.Double:
                        _values.Push(new Value(left.AsDouble() - right.AsDouble()));
                        break;
                    case ValueType.String:
                    default:
                        throw new InvalidOperationException(
                            $"Minus operation not supported for type {left.GetValueType()}"
                        );
                }

                break;
            case BinaryOperation.Multiply:
                if (left.GetValueType() != right.GetValueType())
                {
                    throw new InvalidOperationException(
                        $"Types must be equals! Got {left.GetValueType()} and {right.GetValueType()}"
                    );
                }

                switch (left.GetValueType())
                {
                    case ValueType.Int:
                        _values.Push(new Value(left.AsInt() * right.AsInt()));
                        break;
                    case ValueType.Double:
                        _values.Push(new Value(left.AsDouble() * right.AsDouble()));
                        break;
                    case ValueType.String:
                    default:
                        throw new InvalidOperationException(
                            $"Umnozhenie operation not supported for type {left.GetValueType()}"
                        );
                }

                break;
            case BinaryOperation.Divide:
                if (left.GetValueType() != right.GetValueType())
                {
                    throw new InvalidOperationException(
                        $"Types must be equals! Got {left.GetValueType()} and {right.GetValueType()}"
                    );
                }

                switch (left.GetValueType())
                {
                    case ValueType.Int:
                        _values.Push(new Value(left.AsInt() / right.AsInt()));
                        break;
                    case ValueType.Double:
                        _values.Push(new Value(left.AsDouble() / right.AsDouble()));
                        break;
                    case ValueType.String:
                    default:
                        throw new InvalidOperationException(
                            $"Delenie operation not supported for type {left.GetValueType()}"
                        );
                }

                break;
            case BinaryOperation.Modulo:
                if (left.GetValueType() != right.GetValueType())
                {
                    throw new InvalidOperationException(
                        $"Types must be equals! Got {left.GetValueType()} and {right.GetValueType()}"
                    );
                }

                switch (left.GetValueType())
                {
                    case ValueType.Int:
                        _values.Push(new Value(left.AsInt() % right.AsInt()));
                        break;
                    case ValueType.Double:
                        _values.Push(new Value(left.AsDouble() % right.AsDouble()));
                        break;
                    case ValueType.String:
                    default:
                        throw new InvalidOperationException(
                            $"Procent operation not supported for type {left.GetValueType()}"
                        );
                }

                break;
            case BinaryOperation.LessThan:
                if (left.GetValueType() != right.GetValueType())
                {
                    throw new InvalidOperationException(
                        $"Types must be equals! Got {left.GetValueType()} and {right.GetValueType()}"
                    );
                }

                switch (left.GetValueType())
                {
                    case ValueType.Int:
                        _values.Push(new Value(left.AsInt() < right.AsInt() ? _trueToDouble : _falseToDouble));
                        break;
                    case ValueType.Double:
                        _values.Push(new Value(left.AsDouble() < right.AsDouble() ? _trueToDouble : _falseToDouble));
                        break;
                    case ValueType.String:
                        _values.Push(new Value(string.CompareOrdinal(left.AsString(), right.AsString()) < 0 ? 1 : 0));
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"Less than operation not supported for type {left.GetValueType()}"
                        );
                }

                break;
            case BinaryOperation.GreaterThan:
                if (left.GetValueType() != right.GetValueType())
                {
                    throw new InvalidOperationException(
                        $"Types must be equals! Got {left.GetValueType()} and {right.GetValueType()}"
                    );
                }

                switch (left.GetValueType())
                {
                    case ValueType.Int:
                        _values.Push(new Value(left.AsInt() > right.AsInt() ? _trueToDouble : _falseToDouble));
                        break;
                    case ValueType.Double:
                        _values.Push(new Value(left.AsDouble() > right.AsDouble() ? _trueToDouble : _falseToDouble));
                        break;
                    case ValueType.String:
                        _values.Push(new Value(string.CompareOrdinal(left.AsString(), right.AsString()) > 0 ? 1 : 0));
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"Greater than operation not supported for type {left.GetValueType()}"
                        );
                }

                break;
            case BinaryOperation.LessThanOrEqual:
                if (left.GetValueType() != right.GetValueType())
                {
                    throw new InvalidOperationException(
                        $"Types must be equals! Got {left.GetValueType()} and {right.GetValueType()}"
                    );
                }

                switch (left.GetValueType())
                {
                    case ValueType.Int:
                        _values.Push(new Value(left.AsInt() <= right.AsInt() ? _trueToDouble : _falseToDouble));
                        break;
                    case ValueType.Double:
                        _values.Push(new Value(left.AsDouble() <= right.AsDouble() ? _trueToDouble : _falseToDouble));
                        break;
                    case ValueType.String:
                        _values.Push(new Value(string.CompareOrdinal(left.AsString(), right.AsString()) <= 0 ? 1 : 0));
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"Less than or Equal operation not supported for type {left.GetValueType()}"
                        );
                }

                break;
            case BinaryOperation.GreaterThanOrEqual:
                if (left.GetValueType() != right.GetValueType())
                {
                    throw new InvalidOperationException(
                        $"Types must be equals! Got {left.GetValueType()} and {right.GetValueType()}"
                    );
                }

                switch (left.GetValueType())
                {
                    case ValueType.Int:
                        _values.Push(new Value(left.AsInt() >= right.AsInt() ? _trueToDouble : _falseToDouble));
                        break;
                    case ValueType.Double:
                        _values.Push(new Value(left.AsDouble() >= right.AsDouble() ? _trueToDouble : _falseToDouble));
                        break;
                    case ValueType.String:
                        _values.Push(new Value(string.CompareOrdinal(left.AsString(), right.AsString()) >= 0 ? 1 : 0));
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"Greater than or Equal operation not supported for type {left.GetValueType()}"
                        );
                }

                break;
            case BinaryOperation.Equal:
                if (left.GetValueType() != right.GetValueType())
                {
                    throw new InvalidOperationException(
                        $"Types must be equals! Got {left.GetValueType()} and {right.GetValueType()}"
                    );
                }

                switch (left.GetValueType())
                {
                    case ValueType.Int:
                        _values.Push(new Value(Numbers.AreEqual(left.AsInt(), right.AsInt()) ? _trueToDouble : _falseToDouble));
                        break;
                    case ValueType.Double:
                        _values.Push(new Value(Numbers.AreEqual(left.AsDouble(), right.AsDouble()) ? _trueToDouble : _falseToDouble));
                        break;
                    case ValueType.String:
                        _values.Push(new Value(left.AsString() == right.AsString() ? _trueToDouble : _falseToDouble));
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"Equal than or Equal operation not supported for type {left.GetValueType()}"
                        );
                }

                break;
            case BinaryOperation.NotEqual:
                if (left.GetValueType() != right.GetValueType())
                {
                    throw new InvalidOperationException(
                        $"Types must be equals! Got {left.GetValueType()} and {right.GetValueType()}"
                    );
                }

                switch (left.GetValueType())
                {
                    case ValueType.Int:
                        _values.Push(new Value(Numbers.AreNotEqual(left.AsInt(), right.AsInt()) ? _trueToDouble : _falseToDouble));
                        break;
                    case ValueType.Double:
                        _values.Push(new Value(Numbers.AreNotEqual(left.AsDouble(), right.AsDouble()) ? _trueToDouble : _falseToDouble));
                        break;
                    case ValueType.String:
                        _values.Push(new Value(left.AsString() != right.AsString() ? _trueToDouble : _falseToDouble));
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"Equal than or Equal operation not supported for type {left.GetValueType()}"
                        );
                }

                break;
            case BinaryOperation.LogicalAnd:
                _values.Push(new Value(IsTruthy(left) && IsTruthy(right) ? 1 : 0));
                break;

            case BinaryOperation.LogicalOr:
                _values.Push(new Value(IsTruthy(left) || IsTruthy(right) ? 1 : 0));
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
                Value unaryValue = _values.Pop();

                switch (unaryValue.GetValueType())
                {
                    case ValueType.Int:
                        _values.Push(new Value(-unaryValue.AsInt()));
                        break;
                    case ValueType.Double:
                        _values.Push(new Value(-unaryValue.AsDouble()));
                        break;

                    default:
                        throw new Exception($"Unary minus not supported for type {unaryValue.GetValueType()}");
                }

                break;
            case UnaryOperation.LogicalNot:
                Value value = _values.Pop();

                bool isTrue = IsTruthy(value);

                _values.Push(isTrue ? new Value(0) : new Value(1));
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
        Value value = _values.Pop();
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
        Value value;
        if (d.Value != null)
        {
            d.Value.Accept(this);
            value = _values.Pop();
        }
        else
        {
            if (d.ResultType == default)
            {
                value = new Value(0);
            }
            else
            {
                value = d.ResultType switch
                {
                    ValueType.Int => new Value(0),
                    ValueType.Double => new Value(0.0),
                    ValueType.String => new Value(""),
                    _ => Value.Void,
                };
            }
        }

        context.DefineVariable(d.Name, value);
        _values.Push(Value.Void);
    }

    public void Visit(ConstantDeclaration d)
    {
        d.Value.Accept(this);
        Value value = _values.Pop();

        context.DefineConstant(d.Name, value);
        _values.Push(Value.Void);
    }

    public void Visit(FunctionDeclaration d)
    {
        context.DefineFunction(d);

        _values.Push(Value.Void);
    }

    public void Visit(ExodusExpression e)
    {
        e.Value.Accept(this);
        Value value = _values.Pop();
        environment.AddResult(value);
        _values.Push(Value.Void);
    }

    public void Visit(RaidExpression e)
    {
        Value value = environment.ReadInput();

        context.AssignVariable(e.Name, value);
        _values.Push(Value.Void);
    }

    public void Visit(FunctionCall call)
    {
        List<Value> argValues = new();
        foreach (Expression arg in call.Arguments)
        {
            arg.Accept(this);
            argValues.Add(_values.Pop());
        }

        if (IsBuiltInFunction(call.FunctionName))
        {
            Value result = call.FunctionName.ToLower() switch
            {
                "floor" => argValues[0].GetValueType() switch
                {
                    ValueType.Int => new Value(Math.Floor((double)argValues[0].AsInt())),
                    ValueType.Double => new Value(Math.Floor(argValues[0].AsDouble())),
                    _ => throw new Exception($"Function floor does not support type {argValues[0].GetValueType()}"),
                },
                "ceil" => argValues[0].GetValueType() switch
                {
                    ValueType.Int => new Value(Math.Ceiling((double)argValues[0].AsInt())),
                    ValueType.Double => new Value(Math.Ceiling(argValues[0].AsDouble())),
                    _ => throw new Exception($"Function ceil does not support type {argValues[0].GetValueType()}"),
                },
                "round" => argValues[0].GetValueType() switch
                {
                    ValueType.Int => new Value(Math.Round((double)argValues[0].AsInt())),
                    ValueType.Double => new Value(Math.Round(argValues[0].AsDouble())),
                    _ => throw new Exception($"Function round does not support type {argValues[0].GetValueType()}"),
                },
                "length" => argValues[0].GetValueType() switch
                {
                    ValueType.String => new Value(argValues[0].AsString().Length),
                    _ => throw new Exception($"Function length does not support type {argValues[0].GetValueType()}"),
                },
                "str_at" => HandleStrAt(argValues),
                "abs" => argValues[0].GetValueType() switch
                {
                    ValueType.Int => new Value(Math.Abs(argValues[0].AsInt())),
                    ValueType.Double => new Value(Math.Abs(argValues[0].AsDouble())),
                    _ => throw new Exception($"Function abs does not support type {argValues[0].GetValueType()}"),
                },
                "max" => new Value(argValues.Max(v => v.GetValueType() switch
                {
                    ValueType.Int => v.AsInt(),
                    ValueType.Double => v.AsDouble(),
                    _ => throw new Exception($"Function max does not support type {v.GetValueType()}"),
                })),
                "min" => new Value(argValues.Min(v => v.GetValueType() switch
                {
                    ValueType.Int => v.AsInt(),
                    ValueType.Double => v.AsDouble(),
                    _ => throw new Exception($"Function min does not support type {v.GetValueType()}"),
                })),
                _ => throw new Exception($"Unknown built-in function: {call.FunctionName}"),
            };
            _values.Push(result);
        }
        else
        {
            FunctionDeclaration function = context.GetFunction(call.FunctionName);

            if (argValues.Count != function.Parameters.Count)
            {
                throw new InvalidOperationException(
                    $"Function '{call.FunctionName}' expects {function.Parameters.Count} arguments, " +
                    $"but got {argValues.Count}");
            }

            for (int i = 0; i < argValues.Count; i++)
            {
                Value argValue = argValues[i];
                ValueType paramType = function.Parameters[i].ResultType;

                if (argValue.GetValueType() != paramType)
                {
                    if (argValue.GetValueType() == ValueType.Int && paramType == ValueType.Double)
                    {
                        argValues[i] = new Value(Convert.ToDouble(argValue.AsInt()));
                    }
                    else if (argValue.GetValueType() == ValueType.Double && paramType == ValueType.Int)
                    {
                        argValues[i] = new Value((int)argValue.AsDouble());
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            $"Argument {i + 1} of function '{call.FunctionName}' " +
                            $"must be of type {paramType}, but got {argValue.GetValueType()}");
                    }
                }
            }

            context.PushScope(new Scope());
            try
            {
                for (int i = 0; i < function.Parameters.Count; i++)
                {
                    context.DefineVariable(function.Parameters[i].Name, argValues[i]);
                }

                Value returnValue;
                int stackDepthBeforeBody = _values.Count;

                try
                {
                    foreach (AstNode statement in function.Body)
                    {
                        statement.Accept(this);

                        if (_values.Count > stackDepthBeforeBody && !(statement is ReturnExpression))
                        {
                            _values.Pop();
                        }
                    }

                    returnValue = Value.Void;
                }
                catch (ReturnException ret)
                {
                    if (function.DeclaredType != null)
                    {
                        if (ret.ReturnValue.Equals(Value.Void))
                        {
                            throw new InvalidOperationException(
                                $"Function '{function.Name}' expects return value of type {function.DeclaredType.ResultType}, but got void");
                        }

                        ValueType returnValueType = ret.ReturnValue.GetValueType();
                        ValueType expectedType = function.DeclaredType.ResultType;

                        if (returnValueType != expectedType)
                        {
                            if (returnValueType == ValueType.Double && expectedType == ValueType.Int)
                            {
                                returnValue = new Value((int)ret.ReturnValue.AsDouble());
                            }
                            else if (returnValueType == ValueType.Int && expectedType == ValueType.Double)
                            {
                                returnValue = new Value((double)ret.ReturnValue.AsInt());
                            }
                            else
                            {
                                throw new InvalidOperationException(
                                    $"Function '{function.Name}' expects return type {expectedType}, " +
                                    $"but got {returnValueType}");
                            }
                        }
                        else
                        {
                            returnValue = ret.ReturnValue;
                        }
                    }
                    else
                    {
                        if (!ret.ReturnValue.Equals(Value.Void))
                        {
                            throw new InvalidOperationException(
                                $"Function '{function.Name}' should not return a value, but got {ret.ReturnValue}");
                        }

                        returnValue = ret.ReturnValue;
                    }
                }

                while (_values.Count > stackDepthBeforeBody)
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

        Value conditionValue = _values.Pop();
        if (conditionValue.GetValueType() != ValueType.Int)
        {
            throw new Exception($"Condition expression {e.Condition} does not support type {conditionValue.GetValueType()}");
        }

        bool isTrueCondition = Numbers.AreEqual(_trueToDouble, conditionValue.AsInt());

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
        bool isNotBreaked = true;
        try
        {
            while (isNotBreaked)
            {
                e.Condition.Accept(this);
                Value conditionValue = _values.Pop();

                if (conditionValue.GetValueType() != ValueType.Int)
                {
                    throw new Exception($"Condition expression {e.Condition} does not support type {conditionValue.GetValueType()}");
                }

                if (Numbers.AreEqual(0.0, conditionValue.AsInt()))
                {
                    break;
                }

                foreach (AstNode statement in e.ThenBranch)
                {
                    try
                    {
                        statement.Accept(this);

                        if (_values.Count > 0 && !(statement is ReturnExpression))
                        {
                            _values.Pop();
                        }
                    }
                    catch (ContinueException)
                    {
                        break;
                    }
                    catch (BreakException)
                    {
                        isNotBreaked = false;
                        break;
                    }
                }
            }

            _values.Push(Value.Void);
        }
        finally
        {
            context.PopScope();
        }
    }

    public void Visit(ReturnExpression e)
    {
        e.Value.Accept(this);
        Value returnValue = _values.Pop();
        throw new ReturnException(returnValue);
    }

    public void Visit(ContinueExpression e)
    {
        throw new ContinueException();
    }

    public void Visit(BreakExpression e)
    {
        throw new BreakException();
    }

    public void Visit(IfElseExpression e)
    {
        e.Condition.Accept(this);

        Value conditionValue = _values.Pop();

        if (conditionValue.GetValueType() != ValueType.Int)
        {
            throw new Exception($"Condition expression {e.Condition} does not support type {conditionValue.GetValueType()}");
        }

        bool isTrueCondition = Numbers.AreEqual(_trueToDouble, conditionValue.AsInt());
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

    private bool IsTruthy(Value value)
    {
        return value.GetValueType() switch
        {
            ValueType.Int => value.AsInt() != 0,
            ValueType.Double => value.AsDouble() != 0,
            ValueType.String => !string.IsNullOrEmpty(value.AsString()),
            _ => false,
        };
    }

    private bool IsBuiltInFunction(string functionName)
    {
        string lowerName = functionName.ToLower();
        return lowerName == "floor" || lowerName == "ceil" || lowerName == "round" ||
               lowerName == "abs" || lowerName == "max" || lowerName == "min" ||
               lowerName == "length" || lowerName == "str_at";
    }

    private Value HandleStrAt(List<Value> argValues)
    {
        if (argValues.Count != 2)
        {
            throw new Exception($"str_at requires 2 arguments, got {argValues.Count}");
        }

        if (argValues[0].GetValueType() != ValueType.String)
        {
            throw new Exception($"str_at first argument must be string, got {argValues[0].GetValueType()}");
        }

        if (argValues[1].GetValueType() != ValueType.Int)
        {
            throw new Exception($"str_at second argument must be int, got {argValues[1].GetValueType()}");
        }

        string str = argValues[0].AsString();
        int index = argValues[1].AsInt();

        if (index < 0 || index >= str.Length)
        {
            throw new IndexOutOfRangeException($"Index {index} out of range for string length {str.Length}");
        }

        return new Value(str[index].ToString());
    }
}