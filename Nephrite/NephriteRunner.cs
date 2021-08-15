using NephriteRunner.Exceptions;
using NephriteRunner.Lexer;
using NephriteRunner.Runtime;
using NephriteRunner.SyntaxAnalysis;
using System;

namespace NephriteRunner
{
    public class NephriteRunner
    {
        public bool Run(string source)
        {
            try
            {
                var tokens = new Scanner(source).Run();
                var statements = new Parser(tokens).Run();

                new Interpreter().Run(statements);
            }
            catch (Exception error) when (error is ScanningErrorException || error is ParsingErrorException || error is RuntimeErrorException)
            {
                ReportError(error.StackTrace == null ? error.Message : $"{error.Message}\n{error.StackTrace}");
                return false;
            }

            return true;
        }

        public void ReportError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
