using Ast.Declarations;
using Ast.Expressions;
using Semantics.Exceptions;

using ValueType = Runtime.ValueType;

namespace Semantics.Passes;

/// <summary>
/// Проход по AST выполняет две задачи:
///  1. Вычислить типы данных.
///  2. Проверить корректность программы с точки зрения совместимости типов данных.
/// </summary>
/// <exception cref="TypeErrorException">Бросается при несоответствии типов данных.</exception>
public sealed class ResolveTypesPass : AbstractPass
{
    /// <summary>
    /// Литерал всегда имеет определённый тип.
    /// </summary>
    public override void Visit(LiteralExpression e)
    {
        base.Visit(e);
        e.ResultType = e.Value.GetValueType();
    }

    /// <summary>
    /// Выполняет проверки типов для бинарных операций:
    /// 1. Арифметические и логические операции выполняются над целыми числами и возвращают число.
    /// 2. Операции сравнения выполняются над двумя числами либо двумя строками и возвращают тот же тип.
    /// </summary>
    public override void Visit(BinaryOperationExpression e)
    {
        base.Visit(e);

        ValueType? resultType = GetBinaryOperationResultType(e.Operation, e.Left.ResultType, e.Right.ResultType);
        if (resultType == null)
        {
            throw new TypeErrorException(
                $"Binary operation {e.Operation} is not allowed for types {e.Left.ResultType} and {e.Right.ResultType}"
            );
        }

        e.ResultType = resultType.Value;
    }

    /// <summary>
    /// Выполняет проверки типов для унарного минуса.
    /// Унарный минус применяется только к числам и возвращает число.
    /// </summary>
    public override void Visit(UnaryOperationExpression e)
    {
        base.Visit(e);

        ValueType operandType = e.Operand.ResultType;
        if (e.Operation == UnaryOperation.Minus)
        {
            if (operandType == ValueType.String)
            {
                throw new TypeErrorException($"Unary minus operation is not allowed for type {operandType}");
            }
        }

        e.ResultType = operandType;
    }

    /// <summary>
    /// Проверяет соответствие типов параметров функции и аргументов при вызове этой функции.
    /// </summary>
    public override void Visit(FunctionCall e)
    {
        base.Visit(e);

        CheckFunctionArgumentTypes(e, e.Function);
        e.ResultType = e.Function.ResultType;
    }

    public override void Visit(VariableExpression e)
    {
        base.Visit(e);

        e.ResultType = e.Variable.ResultType;
    }

    /// <summary>
    /// Проверяет тип переменной и тип выражения, которым она инициализируется.
    /// </summary>
    public override void Visit(VariableDeclaration d)
    {
        base.Visit(d);

        if (d.Value != null)
        {
            ValueType valueType = d.Value.ResultType;
            ValueType declaredType = d.DeclaredType?.ResultType ?? valueType;

            if (!AreTypesCompatible(declaredType, valueType))
            {
                throw new TypeErrorException(
                    $"Cannot initialize variable of type {declaredType} with value of type {valueType}"
                );
            }
        }
    }

    public override void Visit(AssignmentExpression e)
    {
        base.Visit(e);

        ValueType variableType = e.Variable.ResultType;

        ValueType valueType = e.Value.ResultType;

        if (!AreTypesCompatible(variableType, valueType))
        {
            throw new TypeErrorException(
                $"Cannot assign value of type {valueType} to variable of type {variableType}"
            );
        }

        e.ResultType = ValueType.Void;
    }

    // public override void Visit(IfElseExpression e)
    // {
    //     base.Visit(e);
    //
    //     CheckResultType("if-else condition", e.Condition, ValueType.Int);
    //
    //     ValueType thenType = e.ThenBranch.ResultType;
    //
    //     if (e.ElseBranch != null)
    //     {
    //         CheckResultType("else branch", e.ElseBranch, thenType);
    //     }
    //     else if (thenType != ValueType.Void)
    //     {
    //         throw new TypeErrorException("The \"if...then\" expression without \"else\" branch may not return value");
    //     }
    //
    //     e.ResultType = thenType;
    // }
    // public override void Visit(FunctionDeclaration d)
    // {
    //     base.Visit(d);
    //
    //     CheckResultType("function body", d.Body, d.ResultType);
    // }
    // public override void Visit(WhileLoopExpression e)
    // {
    //     base.Visit(e);
    //
    //     CheckResultType("while loop condition", e.Condition, ValueType.Int);
    //     CheckResultType("while loop body", e.LoopBody, ValueType.Void);
    //     e.ResultType = ValueType.Void;
    // }
    // public override void Visit(ForLoopExpression e)
    // {
    //     base.Visit(e);
    //
    //     CheckResultType("for loop start value", e.StartValue, ValueType.Int);
    //     CheckResultType("for loop end value", e.EndValue, ValueType.Int);
    //     CheckResultType("for loop body", e.LoopBody, ValueType.Void);
    //     e.ResultType = ValueType.Void;
    // }
    // public override void Visit(ForIteratorDeclaration d)
    // {
    //     base.Visit(d);
    //     d.ResultType = ValueType.Int;
    // }
    // public override void Visit(BreakLoopExpression e)
    // {
    //     base.Visit(e);
    //     e.ResultType = ValueType.Void;
    // }

    /// <summary>
    /// Вычисляет тип результата бинарной операции.
    /// Возвращает null, если бинарная операция не может быть выполнена с указанными типами.
    /// </summary>
    private static ValueType? GetBinaryOperationResultType(BinaryOperation operation, ValueType left, ValueType right)
{
    switch (operation)
    {
        case BinaryOperation.Plus:
            if ((left == ValueType.Int || left == ValueType.Double) &&
                (right == ValueType.Int || right == ValueType.Double))
            {
                if (left == ValueType.Double || right == ValueType.Double)
                {
                    return ValueType.Double;
                }

                return ValueType.Int;
            }

            if (left == ValueType.String && right == ValueType.String)
            {
                return ValueType.String;
            }

            return null;

        case BinaryOperation.Minus:
        case BinaryOperation.Multiply:
        case BinaryOperation.Divide:
        case BinaryOperation.Modulo:
            if ((left == ValueType.Int || left == ValueType.Double) &&
                (right == ValueType.Int || right == ValueType.Double))
            {
                if (left == ValueType.Double || right == ValueType.Double)
                {
                    return ValueType.Double;
                }

                return ValueType.Int;
            }

            return null;

        case BinaryOperation.LessThan:
        case BinaryOperation.GreaterThan:
        case BinaryOperation.LessThanOrEqual:
        case BinaryOperation.GreaterThanOrEqual:
        case BinaryOperation.Equal:
        case BinaryOperation.NotEqual:
            if ((left == ValueType.Int || left == ValueType.Double) &&
                (right == ValueType.Int || right == ValueType.Double))
            {
                return ValueType.Int;
            }

            if (left == ValueType.String && right == ValueType.String)
            {
                return ValueType.Int;
            }

            return null;

        case BinaryOperation.LogicalOr:
        case BinaryOperation.LogicalAnd:
            return ValueType.Int;

        default:
            throw new ArgumentException($"Unknown binary operation {operation}");
    }
}

    /// <summary>
    /// Проверяет соответствие типов формальных параметров и фактических параметров (аргументов) при вызове функции.
    /// </summary>
    private static void CheckFunctionArgumentTypes(FunctionCall e, AbstractFunctionDeclaration function)
    {
        for (int i = 0, iMax = e.Arguments.Count; i < iMax; ++i)
        {
            Expression argument = e.Arguments[i];
            AbstractParameterDeclaration parameter = function.Parameters[i];
            if (!AreTypesCompatible(parameter.ResultType, argument.ResultType))
            {
                throw new TypeErrorException(
                    $"Cannot apply argument #{i} of type {argument.ResultType} to function {e.FunctionName} parameter {parameter.Name} which has type {parameter.ResultType}"
                );
            }
        }
    }

    private static bool AreTypesCompatible(ValueType targetType, ValueType sourceType)
    {
        if (targetType == sourceType)
        {
            return true;
        }

        if (targetType == ValueType.Double && sourceType == ValueType.Int)
        {
            return true;
        }

        if (targetType == ValueType.Int && sourceType == ValueType.Double)
        {
            return true;
        }

        return false;
    }
}