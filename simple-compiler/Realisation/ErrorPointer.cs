using System;
using simple_compiler.Properties;

namespace simple_compiler.Classes
{
    public class ErrorPointer : IError
    {
        private TextReaderFromFile _textReader;

        public void SetTextReader(TextReaderFromFile textReader)
        {
            _textReader = textReader;
        }
        
        public void Warning(string message)
        {
            Console.WriteLine(message);
        }

        public void Error(string message)
        {
            Console.WriteLine(message);
            Environment.Exit(1);
        }

        public void ErrorWithShowingCode(string message)
        {
            Console.WriteLine();
            _textReader.PrintLastNSymbols(30);
            Console.WriteLine("...");
            Error(message);
        }
    }
}