using System;

namespace simple_compiler.Properties
{
    public interface IError
    {
        void Warning(string message);

        void Error(string message);

        void ErrorWithShowingCode(string message);
    }
}