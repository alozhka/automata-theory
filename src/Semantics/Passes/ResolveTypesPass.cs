using Ast;
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
    private ValueType? _expectedReturnType;
    private bool _hasGuaranteedReturn;

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

    public override void Visit(IfElseExpression e)
    {
        base.Visit(e);

        bool thenHasReturn = HasGuaranteedReturnInBlock(e.ThenBranch);
        bool elseHasReturn = HasGuaranteedReturnInBlock(e.ElseBranch);

        if (thenHasReturn && elseHasReturn)
        {
            _hasGuaranteedReturn = true;
        }
    }

    public override void Visit(FunctionDeclaration d)
    {
        _hasGuaranteedReturn = false;
        string functionName = d.Name;
        ValueType expectedReturnType;

        try
        {
            expectedReturnType = d.ResultType;
        }
        catch (InvalidOperationException)
        {
            expectedReturnType = ValueType.Void;
        }

        _expectedReturnType = expectedReturnType;

        base.Visit(d);

        if (expectedReturnType != ValueType.Void)
        {
            if (!_hasGuaranteedReturn && !HasGuaranteedReturnInBlock(d.Body))
            {
                throw new TypeErrorException(
                    $"Function '{functionName}' with return type {expectedReturnType} " +
                    $"must guarantee a return statement in all execution paths"
                );
            }
        }

        _expectedReturnType = null;
    }

    public override void Visit(ReturnExpression e)
    {
        e.Value.Accept(this);

        if (_expectedReturnType == null)
        {
            throw new TypeErrorException("return statement outside of function");
        }

        if (!AreTypesCompatible(_expectedReturnType.Value, e.Value.ResultType))
        {
            throw new TypeErrorException(
                $"Cannot return {e.Value.ResultType} " +
                $"from function returning {_expectedReturnType.Value}"
            );
        }

        e.ResultType = ValueType.Void;
    }

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

    /// <summary>
    /// Проверяет, гарантирует ли блок кода выполнение return statement.
    /// </summary>
    private bool HasGuaranteedReturnInBlock(List<AstNode> block)
    {
        foreach (AstNode node in block)
        {
            if (node is ReturnExpression)
            {
                return true;
            }

            if (node is IfElseExpression ifElse)
            {
                bool thenHasReturn = HasGuaranteedReturnInBlock(ifElse.ThenBranch);
                bool elseHasReturn = HasGuaranteedReturnInBlock(ifElse.ElseBranch);

                if (thenHasReturn && elseHasReturn)
                {
                    return true;
                }
            }
        }

        return false;
    }
}