using NephriteRunner.Exceptions;
using NephriteRunner.Lexer;
using NephriteRunner.Runtime;
using NephriteRunner.SyntaxAnalysis;
using System;
using System.IO;

namespace NephriteRunner
{
    public class NephriteRunner
    {
        public void Run(string[] args)
        {
            if (args.Length == 1)
            {
                try
                {
                    Execute(File.ReadAllText(args[0]));
                }
                catch (IOException)
                {
                    ReportError($"Couldn't open the file. ({args[0]})");
                }
            }

            else if (args.Length == 0)
            {
                Console.Write($"You have entered the Nephrite REPL. Enter a command to run it.\nUse ");
                WriteConsoleColour(ConsoleColor.DarkCyan, "ctrl + c");
                Console.Write(" or ");
                WriteConsoleColour(ConsoleColor.DarkCyan, "exit 0;");
                Console.Write(" to quit the repl.\n\n");

                while (true)
                {
                    Console.Write(">> ");
                    var input = Console.ReadLine();

                    if (!string.IsNullOrEmpty(input))
                        Execute(input);

                    else
                        break;

                    Console.WriteLine();
                }

            }

            else
                ReportError("Usage: Press enter to start the REPL. Or write the file path to run it.");

        }

        private void Execute(string source)
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
            }
        }

        private void ReportError(string message)
            => WriteConsoleColour(ConsoleColor.Red, message);

        private static void WriteConsoleColour(ConsoleColor colour, string text)
        {
            Console.ForegroundColor = colour;
            Console.Write(text);
            Console.ResetColor();
        }
    }
}
