using System;

namespace NephriteRunner.Exceptions
{
    internal class RuntimeErrorException : Exception
    {
        public RuntimeErrorException(string message) : base(message)
        {
        }
    }
}
