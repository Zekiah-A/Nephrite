namespace NephriteRunner.Lexer
{
    internal record Token(TokenType Type, object? Value, int Line);
}
