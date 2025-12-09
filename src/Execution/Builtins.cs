using Ast.Declarations;
using Runtime;

using ValueType = Runtime.ValueType;

namespace Execution;

/// <summary>
/// Объект, предоставляющий доступ к встроенным символам языка.
/// </summary>
public class Builtins
{
    public Builtins()
    {
        Initialize();
    }

    public Dictionary<string, BuiltinFunction> Functions { get; } = new();

    public Dictionary<string, BuiltinType> Types { get; } = new();

    public void Initialize()
    {
        Types["dayzint"] = new BuiltinType("dayzint", ValueType.Int);
        Types["fallout"] = new BuiltinType("fallout", ValueType.Double);
        Types["strike"] = new BuiltinType("strike", ValueType.String);

        Functions["floor"] = new BuiltinFunction(
            "floor",
            [new BuiltinFunctionParameter("value", ValueType.Double)],
            ValueType.Double,
            args => new Value(Math.Floor(args[0].AsDouble()))
        );

        Functions["ceil"] = new BuiltinFunction(
            "ceil",
            [new BuiltinFunctionParameter("value", ValueType.Double)],
            ValueType.Double,
            args => new Value(Math.Ceiling(args[0].AsDouble()))
        );

        Functions["round"] = new BuiltinFunction(
            "round",
            [new BuiltinFunctionParameter("value", ValueType.Double)],
            ValueType.Double,
            args => new Value(Math.Round(args[0].AsDouble()))
        );

        Functions["length"] = new BuiltinFunction(
            "length",
            [new BuiltinFunctionParameter("str", ValueType.String)],
            ValueType.Int,
            args => new Value(args[0].AsString().Length)
        );

        Functions["str_at"] = new BuiltinFunction(
            "str_at",
            [
                new BuiltinFunctionParameter("str", ValueType.String),
                new BuiltinFunctionParameter("index", ValueType.Int)
            ],
            ValueType.String,
            args =>
            {
                string str = args[0].AsString();
                int index = args[1].AsInt();
                if (index < 0 || index >= str.Length)
                {
                    throw new IndexOutOfRangeException();
                }

                return new Value(str[index].ToString());
            }
        );

        Functions["abs"] = new BuiltinFunction(
            "abs",
            [new BuiltinFunctionParameter("value", ValueType.Double)],
            ValueType.Double,
            args => new Value(Math.Abs(args[0].AsDouble()))
        );

        Functions["max"] = new BuiltinFunction(
            "max",
            [
                new BuiltinFunctionParameter("a", ValueType.Double),
                new BuiltinFunctionParameter("b", ValueType.Double)
            ],
            ValueType.Double,
            args => new Value(Math.Max(args[0].AsDouble(), args[1].AsDouble()))
        );

        Functions["min"] = new BuiltinFunction(
            "min",
            [
                new BuiltinFunctionParameter("a", ValueType.Double),
                new BuiltinFunctionParameter("b", ValueType.Double),
            ],
            ValueType.Double,
            args => new Value(Math.Min(args[0].AsDouble(), args[1].AsDouble()))
        );
    }
}