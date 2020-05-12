using System;
using simple_compiler.Classes;
using simple_compiler.Interfaces;

namespace simple_compiler
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            ErrorPointer errorPointer = new ErrorPointer();
            TextReaderFromFile textReaderFromFile = new TextReaderFromFile(errorPointer);
            errorPointer.SetTextReader(textReaderFromFile);
            textReaderFromFile.SetPath("..\\..\\code-example.txt");
            IScanner scanner = new Scanner(textReaderFromFile, errorPointer);
            scanner.NextToken();
            Table table = new Table(errorPointer);
            SVM svm = new SVM();
            Gen gen = new Gen(svm, table, errorPointer);
            Parser parser = new Parser(scanner, errorPointer, table, gen);
            parser.Program();
            svm.Run();
            int a;
        }
    }
}