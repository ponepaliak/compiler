namespace simple_compiler.Interfaces
{
    public interface IToken
    {
        TokenType TokenType { get; set; }
        string Value { get; set; }
    }
}