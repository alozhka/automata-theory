using System.Globalization;

namespace Lexer;

public class TokenValue
{
    private readonly object _value;

    public TokenValue(int value)
    {
        _value = value;
    }

    public TokenValue(string value)
    {
        _value = value;
    }

    public TokenValue(double value)
    {
        _value = value;
    }

    /// <summary>
    /// Возвращает значение токена в виде строки.
    /// </summary>
    /// <remarks>
    /// Имя метода пересекается со стандартным методом ToString(), поэтому добавлен override.
    /// </remarks>
    public override string ToString()
    {
        return _value switch
        {
            string s => s,
            int i => i.ToString(),
            double d => d.ToString(CultureInfo.InvariantCulture),
            _ => throw new NotImplementedException(),
        };
    }

    /// <summary>
    /// Возвращает значение токена в виде числа.
    /// </summary>
    public double ToDouble()
    {
        return _value switch
        {
            string s => double.Parse(s, CultureInfo.InvariantCulture),
            int i => i,
            double d => d,
            _ => throw new NotImplementedException(),
        };
    }

    /// <summary>
    /// Проверяет равенство значений токенов. Значения разных типов всегда считаются разными.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is TokenValue other)
        {
            if (_value is string str1 && other._value is string str2)
            {
                return str1 == str2;
            }

            if (IsNumeric(_value) && IsNumeric(other._value))
            {
                double d1 = ToDouble();
                double d2 = other.ToDouble();
                return Math.Abs(d1 - d2) < 0.0000001;
            }

            return _value.Equals(other._value);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    private bool IsNumeric(object value)
    {
        return value is int or double or float or decimal;
    }
}