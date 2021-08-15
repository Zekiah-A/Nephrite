using NephriteRunner.Exceptions;
using NephriteRunner.Lexer;
using System.Collections.Generic;

namespace NephriteRunner.Runtime
{
    internal class NephriteEnvironment
    {
        private readonly NephriteEnvironment? enclosing;
        private readonly Dictionary<string, object?> values;

        public NephriteEnvironment(NephriteEnvironment? enclosing = null)
        {
            this.enclosing = enclosing;

            values = new Dictionary<string, object>();
        }

        // Variables can not have empty names.
        public void Define(Token token, object? value)
            => values.Add(token.Value!.ToString()!, value);

        public object Get(Token name)
        {
            if (name.Value is not null)
            {
                var value = name.Value.ToString();

                if (value is not null)
                {
                    if (values.ContainsKey(value))
                        return values[value];

                    if (enclosing != null)
                        return enclosing.Get(name);

                    throw new RuntimeErrorException($"Undefined variable '{name}'");
                }
            }

            throw new RuntimeErrorException($"Undefined variable '{name}'");
        }

        public void Assign(Token token, object value)
        {
            if (token.Value is not null)
            {
                var name = token.Value.ToString();

                if (name is not null)
                {
                    if (values.ContainsKey(name))
                    {
                        values[name] = value;
                        return;
                    }

                    if (enclosing != null)
                    {
                        enclosing.Assign(token, value);
                        return;
                    }
                }
            }

            throw new RuntimeErrorException($"Undefined variable '{token}'");
        }
    }
}
