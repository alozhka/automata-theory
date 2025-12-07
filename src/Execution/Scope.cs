using System.Globalization;

using Runtime;

namespace Execution;

public class Scope
{
    private readonly Dictionary<string, Value> _variables = [];

    /// <summary>
    /// Читает переменную из этой области видимости.
    /// Возвращает false, если переменная не объявлена в этой области видимости.
    /// </summary>
    public bool TryGetVariable(string name, out Value value)
    {
        if (_variables.TryGetValue(name, out Value? v))
        {
            value = v;
            return true;
        }

        value = Value.Void;
        return false;
    }

    /// <summary>
    /// Присваивает переменную в этой области видимости.
    /// Возвращает false, если переменная не объявлена в этой области видимости.
    /// </summary>
    public bool TryAssignVariable(string name, Value newValue)
    {
        if (_variables.ContainsKey(name))
        {
            Value currentValue = _variables[name];

            if (currentValue.GetValueType() != newValue.GetValueType())
            {
                Value convertedValue = ConvertToType(newValue, currentValue.GetValueType());
                _variables[name] = convertedValue;
            }
            else
            {
                _variables[name] = newValue;
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Объявляет переменную в этой области видимости.
    /// Возвращает false, если переменная уже объявлена в этой области видимости.
    /// </summary>
    public bool TryDefineVariable(string name, Value value)
    {
        return _variables.TryAdd(name, value);
    }

    private Value ConvertToType(Value value, Runtime.ValueType targetType)
    {
        if (value.GetValueType() == targetType)
        {
            return value;
        }

        return (value.GetValueType(), targetType) switch
        {
            (Runtime.ValueType.Int, Runtime.ValueType.Double) => new Value((double)value.AsInt()),
            (Runtime.ValueType.Double, Runtime.ValueType.Int) => new Value((int)value.AsDouble()),

            (Runtime.ValueType.Int, Runtime.ValueType.String) => new Value(value.AsInt().ToString()),
            (Runtime.ValueType.Double, Runtime.ValueType.String) => new Value(value.AsDouble().ToString(CultureInfo.InvariantCulture)),

            _ => throw new InvalidOperationException(
                $"Cannot convert {value.GetValueType()} to {targetType}"),
        };
    }
}