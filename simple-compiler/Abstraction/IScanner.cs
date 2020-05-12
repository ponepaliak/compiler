namespace simple_compiler.Interfaces
{
    public interface IScanner
    {
        void NextToken();

        IToken GetToken();
    }
}