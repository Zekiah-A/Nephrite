using NephriteRunner.Exceptions;
using NephriteRunner.Lexer;
using NephriteRunner.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace NephriteRunner.Runtime
{
    internal class Interpreter : IExpressionVisitor<object>, IStatementVisitor<object>
    {
        private NephriteEnvironment environment;

        public Interpreter()
        {
            environment = new NephriteEnvironment();
        }

        public void Run(ImmutableArray<Statement> statements)
        {
            try
            {
                foreach (Statement statement in statements)
                    Execute(statement);
            }
            catch (RuntimeErrorException)
            {
                throw;
            }
        }

        public object VisitBinaryExpression(Binary binary)
        {
            var left = Evaluate(binary.Left);
            var right = Evaluate(binary.Right);

            switch (binary.Operator.Type)
            {
                case TokenType.Plus:
                    {
                        if (left is double && right is double)
                            return (double)left + (double)right;

                        if (left is string && right is string)
                            return (string)left + (string)right;

                        throw new RuntimeErrorException("Operands must be two numbers or two strings.");

                    }

                case TokenType.Minus:
                    {
                        CheckNumberOperands(binary.Operator, left, right);
                        return (double)left - (double)right;
                    }

                case TokenType.Slash:
                    {
                        CheckNumberOperands(binary.Operator, left, right);
                        return (double)left / (double)right;
                    }

                case TokenType.Star:
                    {
                        CheckNumberOperands(binary.Operator, left, right);
                        return (double)left * (double)right;
                    }

                case TokenType.EqualEqual:
                    return IsEqual(left, right);

                case TokenType.BangEqual:
                    return !IsEqual(left, right);

                case TokenType.Greater:
                    {
                        if (left is double && right is double)
                            return (double)left > (double)right;

                        if (left is string && right is string)
                            return ((string)left).Length > ((string)right).Length;

                        throw new RuntimeErrorException("Operands must be two numbers or two strings.");
                    }

                case TokenType.GreaterEqual:
                    {
                        if (left is double && right is double)
                            return (double)left >= (double)right;

                        if (left is string && right is string)
                            return ((string)left).Length >= ((string)right).Length;

                        throw new RuntimeErrorException("Operands must be two numbers or two strings.");
                    }

                case TokenType.Less:
                    {
                        if (left is double && right is double)
                            return (double)left < (double)right;

                        if (left is string && right is string)
                            return ((string)left).Length < ((string)right).Length;

                        throw new RuntimeErrorException("Operands must be two numbers or two strings.");
                    }

                case TokenType.LessEqual:
                    {
                        if (left is double && right is double)
                            return (double)left <= (double)right;

                        if (left is string && right is string)
                            return ((string)left).Length <= ((string)right).Length;

                        throw new RuntimeErrorException("Operands must be two numbers or two strings.");
                    }
            }

            throw new RuntimeErrorException("Unknown operator.");
        }

        public object VisitGroupingExpression(Grouping grouping)
            => Evaluate(grouping.Expression);

        public object? VisitLiteralExpression(Literal literal)
            => literal.Value;

        public object VisitUnaryExpression(Unary unary)
        {
            var right = Evaluate(unary.Right);
            switch (unary.Operator.Type)
            {
                case TokenType.Bang:
                    return !IsTruthy(right);

                case TokenType.Minus:
                    CheckNumberOperands(unary.Operator, right);
                    return -(double)right;
            }

            throw new RuntimeErrorException("Unknown operator.");
        }

        public object VisitVariableExpression(Variable variable)
            => environment.Get(variable.Name);

        public object VisitAssignExpression(Assign assign)
        {
            var value = Evaluate(assign.Value);

            environment.Assign(assign.Name, value);
            return value;
        }

        public object VisitLogicalExpression(Logical logical)
        {
            var left = Evaluate(logical.Left);

            if (logical.Operator.Type == TokenType.Or)
            {
                if (IsTruthy(left))
                    return left;
            }
            else
            {
                if (!IsTruthy(left))
                    return left;
            }

            return Evaluate(logical.Right);
        }

        private object Evaluate(Expression expression)
            => expression.Accept(this);

        public object VisitBlockStatement(Block block)
        {
            ExecuteBlock(block.Statements, new NephriteEnvironment(environment));
            return block;
        }

        public object VisitIfStatement(If @if)
        {
            if (IsTruthy(Evaluate(@if.Condition)))
                Execute(@if.ThenBranch);

            else if (@if.ElseBranch != null)
                Execute(@if.ElseBranch);

            return @if;
        }

        public object VisitWriteStatement(WriteLine writeLine)
        {
            var value = Evaluate(writeLine.Expression);

            if (value is null)
                Console.WriteLine("null");

            else if (value is double)
                Console.WriteLine(value.ToString());

            else
                Console.WriteLine(value);

            return writeLine;
        }

        public object VisitStatementExpression(StatementExpression statementExpression)
        {
            Evaluate(statementExpression.Expression);
            return statementExpression;
        }

        public object VisitVarStatement(Var var)
        {
            object? value = null;
            if (var.Initializer != null)
                value = Evaluate(var.Initializer);

            environment.Define(var.Name, value);
            return var;
        }

        public object VisitWhileStatement(While @while)
        {
            while (IsTruthy(Evaluate(@while.Condition)))
                Execute(@while.Body);

            return @while;
        }

        private void ExecuteBlock(List<Statement> statements, NephriteEnvironment environment)
        {
            var previous = this.environment;
            try
            {
                this.environment = environment;

                foreach (Statement statement in statements)
                    Execute(statement);
            }
            finally
            {
                this.environment = previous;
            }
        }

        private void Execute(Statement statement)
            => statement.Accept(this);

        private bool IsTruthy(object value)
        {
            if (value == null)
                return false;

            return value is bool ? (bool)value : true;
        }

        private bool IsEqual(object left, object right)
        {
            if (left == null && left == null)
                return true;

            if (left == null)
                return false;

            return left.Equals(right);
        }

        private void CheckNumberOperands(Token @operator, params object[] operands)
        {
            foreach (var item in operands)
                if (item is not double)
                    throw new RuntimeErrorException($"Operands must be a number ({@operator.Type})");
        }
    }
}
