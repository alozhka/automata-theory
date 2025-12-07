namespace Ast.Declarations;

public class Parameter
{
    public Parameter(string name, Runtime.ValueType type)
    {
        Name = name;
        Type = type;
    }

    public string Name { get; }

    public Runtime.ValueType Type { get; }
}