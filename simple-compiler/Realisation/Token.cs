using simple_compiler.Interfaces;

namespace simple_compiler.Classes
{
    public class Token : IToken
    {
        private string _value;
        private TokenType _tokenType;

        public TokenType TokenType
        {
            get => _tokenType;
            set => _tokenType = value;
        }

        public string Value
        {
            get => _value;
            set => _value = value;
        }
    }
}