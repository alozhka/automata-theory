using Ast.Declarations;
using Ast.Expressions;
using Semantics.Exceptions;
using Semantics.Symbols;

namespace Semantics.Passes;

/// <summary>
/// Проход по AST, устанавливающий соответствие имён и символов (объявлений).
/// </summary>
public sealed class ResolveNamesPass : AbstractPass
{
    /// <summary>
    /// В таблицу символов складываются объявления.
    /// </summary>
    private SymbolsTable _symbols;

    public ResolveNamesPass(SymbolsTable globalSymbols)
    {
        _symbols = globalSymbols;
    }

    public override void Visit(RaidExpression e)
    {
        base.Visit(e);

        e.Variable = ResolveVariable(e.Name);
    }

    public override void Visit(AssignmentExpression e)
    {
        base.Visit(e);

        AbstractVariableDeclaration variable = ResolveVariable(e.Name);
        e.Variable = variable;

        if (variable is ConstantDeclaration)
        {
            throw new InvalidAssignmentException(
                $"Cannot assign to constant '{e.Name}'. Constants are immutable.");
        }
    }

    public override void Visit(FunctionCall e)
    {
        base.Visit(e);

        e.Function = ResolveFunction(e.FunctionName);
    }

    public override void Visit(ConstantDeclaration d)
    {
        base.Visit(d);

        d.DeclaredType = ResolveType(d.TypeName);

        d.ResultType = ConvertTypeNameToValueType(d.TypeName);

        _symbols.DefineSymbol(d.Name, d);
    }

    public override void Visit(VariableDeclaration d)
    {
        base.Visit(d);

        d.DeclaredType = ResolveType(d.DeclaredTypeName);

        d.ResultType = ConvertTypeNameToValueType(d.DeclaredTypeName);

        _symbols.DefineSymbol(d.Name, d);
    }

    public override void Visit(VariableExpression e)
    {
        base.Visit(e);

        e.Variable = ResolveVariable(e.Name);
    }

    public override void Visit(ParameterDeclaration d)
    {
        base.Visit(d);

        d.Type = ResolveType(d.TypeName);
        d.ResultType = ConvertTypeNameToValueType(d.TypeName);

        _symbols.DefineSymbol(d.Name, d);
    }

    public override void Visit(ForLoopExpression e)
    {
        _symbols = new SymbolsTable(_symbols);
        try
        {
            VariableDeclaration iteratorDecl = new(
                e.IteratorName,
                e.TypeName,
                e.StartValue
            );
            iteratorDecl.ResultType = ConvertTypeNameToValueType(e.TypeName);
            _symbols.DefineSymbol(e.IteratorName, iteratorDecl);

            base.Visit(e);
        }
        finally
        {
            _symbols = _symbols.Parent!;
        }
    }

    public override void Visit(FunctionDeclaration d)
    {
        _symbols.DefineSymbol(d.Name, d);

        if (d.DeclaredTypeName != null)
        {
            d.DeclaredType = ResolveType(d.DeclaredTypeName);
            d.ResultType = ConvertTypeNameToValueType(d.DeclaredTypeName);
        }

        _symbols = new SymbolsTable(_symbols);
        try
        {
            foreach (AbstractParameterDeclaration param in d.Parameters)
            {
                param.Accept(this);
            }

            base.Visit(d);
        }
        finally
        {
            _symbols = _symbols.Parent!;
        }
    }

    private AbstractFunctionDeclaration ResolveFunction(string name)
    {
        Declaration symbol = _symbols.GetSymbol(name);
        if (symbol is AbstractFunctionDeclaration function)
        {
            return function;
        }

        throw new InvalidSymbolException(
            $"Name {name} does not refer to a function"
        );
    }

    private AbstractVariableDeclaration ResolveVariable(string name)
    {
        Declaration symbol = _symbols.GetSymbol(name);
        if (symbol is AbstractVariableDeclaration variable)
        {
            return variable;
        }

        throw new InvalidSymbolException(
            $"Name {name} does not refer to a variable"
        );
    }

    private AbstractTypeDeclaration ResolveType(string name)
    {
        Declaration symbol = _symbols.GetSymbol(name);
        if (symbol is AbstractTypeDeclaration type)
        {
            return type;
        }

        throw new InvalidSymbolException(
            $"Name {name} does not refer to a type"
        );
    }

    private Runtime.ValueType ConvertTypeNameToValueType(string typeName)
    {
        return typeName.ToLower() switch
        {
            "dayzint" or "int" => Runtime.ValueType.Int,
            "fallout" or "double" => Runtime.ValueType.Double,
            "strike" or "string" => Runtime.ValueType.String,
            _ => throw new Exception($"Unknown type: {typeName}"),
        };
    }
}