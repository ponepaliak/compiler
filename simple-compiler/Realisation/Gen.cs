using System;
using System.Runtime.InteropServices;
using simple_compiler.Interfaces;

namespace simple_compiler.Classes
{
    public class Gen
    {
        private SVM _svm;
        private Table _table;
        private ErrorPointer _errorPointer;
        private int _pc = 0;
        
        public Gen(SVM svm, Table table, ErrorPointer errorPointer)
        {
            _svm = svm;
            _table = table;
            _errorPointer = errorPointer;
        }

        public int Pc
        {
            get => _pc;
        }

        public void Cmd(int cmd)
        {
            _svm.InMemory(_pc++, cmd);
        }

        public void Fixup(int a)
        {
            while (a > 0)
            {
                int index = a - 2;
                int temp = _svm.FromMemory(index);
                _svm.InMemory(index, _pc);
                a = temp;
            }
        }
        
        public void Odd() {
            Cmd(2);
            Cmd(VMCommands.cmMod);
            Cmd(0);
            Cmd(0);
            Cmd(VMCommands.cmIfEQ);
        }
        
        public void Comp(TokenType Lex) {
            Cmd(0);
            switch(Lex) {
                case TokenType.isEquel : Cmd(VMCommands.cmIfNE); break;
                case TokenType.notEquel : Cmd(VMCommands.cmIfEQ); break;
                case TokenType.lessEquel : Cmd(VMCommands.cmIfGT); break;
                case TokenType.less : Cmd(VMCommands.cmIfGE); break;
                case TokenType.moreEquel : Cmd(VMCommands.cmIfLT); break;
                case TokenType.more : Cmd(VMCommands.cmIfLE); break;
            }
        }
        
        public void Const(string strNum) {
            int num = Int32.Parse(strNum);
            Cmd(Math.Abs(num));
            if ( num < 0 )
                Cmd(VMCommands.cmNeg);
        }
        
        public void Addr(AbstractVariable variable)
        {
            NumberVariable nVar = (NumberVariable) variable;
            Cmd(nVar.Value);
            nVar.Value = _pc + 1;
        }
        
        public void AllocateVariables() {
            
            while( _table.IsVarExist() )
            {
                NumberVariable vRef = (NumberVariable) _table.NextVar();
                if ( vRef.Value == 0 )
                    _errorPointer.Warning(
                        "Змінна " + vRef.Name + " не використовується"
                    );
                else {
                    Fixup(vRef.Value);
                    _pc++;
                }
            }
        }
    }
}