namespace simple_compiler.Interfaces
{
    public interface ITextReader
    {
        void NextChar();

        void PrevChar();
        char GetChar();

        bool IsEnd();
    }
}