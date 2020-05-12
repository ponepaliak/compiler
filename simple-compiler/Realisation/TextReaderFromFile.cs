using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using simple_compiler.Interfaces;
using simple_compiler.Properties;

namespace simple_compiler.Classes
{
    public class TextReaderFromFile : ITextReader
    {
        private IError _error;
        private char[] _ignoredCharacters = {'\n', '\t', '\r', ' '};
        private string _text;
        private int _position;
        private char _currentChar;

        public TextReaderFromFile(IError error)
        {
            _error = error;
        }
        
        public void SetPath(string path)
        {
            if (!File.Exists(path)) _error.Error("File not exist");
            StreamReader file = new StreamReader(path);
            _text = FiltrationSymbol(file.ReadToEnd());
            _position = 0;
            NextChar();
        }

        private string FiltrationSymbol(string text)
        {
            return string.Join("", text.Split(_ignoredCharacters));
        }
        
        public void NextChar()
        {
            _currentChar = _position < _text.Length ? _text[_position++] : '\0';
        }

        public void PrevChar()
        {
            _currentChar = _text[--_position];
        }
        
        public char GetChar()
        {
            return _currentChar;
        }

        public bool IsEnd()
        {
            return _position == _text.Length;
        }

        public void PrintLastNSymbols(int n)
        {
            int symbolNumber = n > _position ? 0 : _position - n;
            for (; symbolNumber <= _position && symbolNumber < _text.Length; symbolNumber++)
            {
                Console.Write(_text[symbolNumber]);
            }
        }
    }
}