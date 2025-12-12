using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

namespace Ast;

public interface IAstVisitor
{
    public void Visit(BinaryOperationExpression e);

    public void Visit(UnaryOperationExpression e);

    public void Visit(LiteralExpression e);

    public void Visit(VariableExpression e);

    public void Visit(AssignmentStatement e);

    public void Visit(ContinueStatement e);

    public void Visit(BreakStatement e);

    public void Visit(VariableScopeExpression e);

    public void Visit(IfElseStatement e);

    public void Visit(IfStatement e);

    public void Visit(ForLoopStatement e);

    public void Visit(VariableDeclaration d);

    public void Visit(ConstantDeclaration d);

    public void Visit(FunctionDeclaration d);

    public void Visit(ExodusStatement e);

    public void Visit(RaidStatement e);

    public void Visit(FunctionCall e);

    public void Visit(ReturnStatement e);

    public void Visit(WhileLoopStatement e);

    public void Visit(ParameterDeclaration d);
}