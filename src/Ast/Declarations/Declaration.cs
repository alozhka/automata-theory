using Ast.Attributes;

namespace Ast.Declarations;

/// <summary>
/// Абстрактный класс всех объявлений (declarations).
/// </summary>
public abstract class Declaration : AstNode
{
    private AstAttribute<Runtime.ValueType> _resultType;

    /// <summary>
    /// Тип результата объявления.
    /// </summary>
    public Runtime.ValueType ResultType
    {
        get => _resultType.Get();

        set => _resultType.Set(value);
    }
}