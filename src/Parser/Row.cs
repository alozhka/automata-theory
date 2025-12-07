using Runtime;

namespace Parser;

public class Row
{
    private readonly object[] _values;

    public Row(params object[] values)
    {
        _values = values;
    }

    public int ColumnCount => _values.Length;

    public object this[int index] => _values[index];

    public Value GetValue(int index)
    {
        object obj = _values[index];

        return obj switch
        {
            Value v => v,
            string s => new Value(s),
            int i => new Value(i),
            double d => new Value(d),
            _ => throw new InvalidOperationException($"Cannot convert {obj.GetType()} to Value"),
        };
    }
}