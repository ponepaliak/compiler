using System;
using System.Collections.Generic;
using System.Linq;
using simple_compiler.Interfaces;
using simple_compiler.Properties;

namespace simple_compiler.Classes
{
    public class Scanner : IScanner
    {
        private ITextReader _textReader;
        private IError _error;
        private IToken _token = new Token();
        private string _tokenValue;

        private string[] _comparisonOperations = {">", "<", "="};

        private Dictionary<string, TokenType> _keyWordDictionary = new Dictionary<string, TokenType>
        {
            {"if", TokenType.ifOper},
            {"elif", TokenType.ifelOper},
            {"else", TokenType.elseOper},
            {"while", TokenType.whileOper},
            {"int", TokenType.intType}
        };

        private Dictionary<string, TokenType> _notAlphabeticSymbolDictionary = new Dictionary<string, TokenType>
        {
            {";", TokenType.semicolon},
            {":", TokenType.colon},
            {"(", TokenType.leftBracket},
            {")", TokenType.rightBracket},
            {"\r", TokenType.colon},
            {"+", TokenType.plus},
            {"-", TokenType.minus},
            {"*", TokenType.multiply},
            {"/", TokenType.divide},
            {">", TokenType.more},
            {">=", TokenType.moreEquel},
            {"<", TokenType.less},
            {"<=", TokenType.lessEquel},
            {"=", TokenType.equel},
            {"==", TokenType.isEquel},
            {"#", TokenType.notEquel},
            {"%", TokenType.mod},
            {"{", TokenType.openBlock},
            {"}", TokenType.closeBlock},
            {"\0", TokenType.eof},
            {"$", TokenType.print}
        };

        public Scanner(ITextReader textReader, IError error)
        {
            _textReader = textReader;
            _error = error;
        }

        public IToken GetToken()
        {
            return _token;
        }

        public void NextToken()
        {
            PurificationOfInternalValues();
            TokenDefinition();
            SetValueToToken();
        }

        private void PurificationOfInternalValues()
        {
            _tokenValue = "";
            _token = new Token();
        }

        private void TokenDefinition()
        {
            char symbol = _textReader.GetChar();

            if (Char.IsLetter(symbol)) 
                Ident();
            else if (Char.IsDigit(symbol)) 
                Number();
            else 
                NotAlphabeticCharacters();
        }

        private void Ident()
        {
            while (Char.IsLetterOrDigit(_textReader.GetChar()))
            {
                AddSymbolToTokenValue();
            }
            
            _token.TokenType = GetIdentType();
        }

        private void Number()
        {
            while (Char.IsDigit(_textReader.GetChar()))
            {
                AddSymbolToTokenValue();
            }

            _token.TokenType = TokenType.number;
        }

        private void NotAlphabeticCharacters()
        {
            _tokenValue += _textReader.GetChar();
            _textReader.NextChar();

            if (_comparisonOperations.Contains(_tokenValue) && _textReader.GetChar() == '=' && !_textReader.IsEnd())
            {
                _tokenValue += _textReader.GetChar();
                _textReader.NextChar();
            }

            _token.TokenType = GetNotAlphabeticCharacterType();
        }

        private void SetValueToToken()
        {
            _token.Value = _tokenValue;
        }

        private void AddSymbolToTokenValue()
        {
            _tokenValue += _textReader.GetChar();
            _textReader.NextChar();
        }

        private TokenType GetIdentType()
        {
            if (_keyWordDictionary.ContainsKey(_tokenValue))
                return _keyWordDictionary[_tokenValue];
            return TokenType.varName;
        }

        private TokenType GetNotAlphabeticCharacterType()
        {
            if (_notAlphabeticSymbolDictionary.ContainsKey(_tokenValue))
                return _notAlphabeticSymbolDictionary[_tokenValue];
            Console.WriteLine($"An unspecified character is present {_tokenValue}");
            _error.Error($"An unspecified character is present {_tokenValue}");
            return TokenType.error;
        }
    }
}