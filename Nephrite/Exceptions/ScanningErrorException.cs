using System;

namespace NephriteRunner.Exceptions
{
    internal class ScanningErrorException : Exception
    {
        public ScanningErrorException(string message) : base(message)
        {
        }
    }
}
