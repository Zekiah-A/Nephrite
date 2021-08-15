using System;

namespace NephriteRunner.Exceptions
{
    internal class ParsingErrorException : Exception
    {
        public ParsingErrorException(string message) : base(message)
        {
        }
    }
}
