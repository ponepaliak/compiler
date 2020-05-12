using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace simple_compiler.Classes
{
    public class SVM
    {
        private const int MEMSIZE = 8 * 1024;
        private int[] _memory = new int[MEMSIZE];
        private int _pc = 0;
        private int _sp = MEMSIZE;
        private int _cmd;
        private int _buf;

        private delegate void Action();

        private Dictionary<int, Action> _procesorsCommandDict;

        public SVM()
        {
            _procesorsCommandDict = new Dictionary<int, Action>
            {
                {VMCommands.cmAdd, CmAdd},
                {VMCommands.cmSub, CmSub},
                {VMCommands.cmMult, CmMult},
                {VMCommands.cmDiv, CmDiv},
                {VMCommands.cmMod, CmMod},
                {VMCommands.cmNeg, CmNeg},
                {VMCommands.cmLoad, CmLoad},
                {VMCommands.cmSave, CmSave},
                {VMCommands.cmDup, CmDup},
                {VMCommands.cmDrop, CmDrop},
                {VMCommands.cmSwap, CmSwap},
                {VMCommands.cmOver, CmOver},
                {VMCommands.cmGOTO, CmGOTO},
                {VMCommands.cmIfEQ, CmIfEQ},
                {VMCommands.cmIfNE, CmIfNE},
                {VMCommands.cmIfLE, CmIfLE},
                {VMCommands.cmIfLT, CmIfLT},
                {VMCommands.cmIfGE, CmIfGE},
                {VMCommands.cmIfGT, CmIfGT},
                {VMCommands.cmIn, CmIn},
                {VMCommands.cmOut, CmOut},
                {VMCommands.cmOutLn, CmOutLn}
            };
        }

        public void InMemory(int pc, int cmd)
        {
            _memory[pc] = cmd;
        }

        public int FromMemory(int addr)
        {
            return _memory[addr];
        }

        public void Run()
        {
            while ((_cmd = _memory[_pc++]) != VMCommands.cmStop)
            {
                Executing();
            }
            Finishing();
        }

        private void Executing()
        {
            if (_cmd >= 0)
            {
                _memory[--_sp] = _cmd;
            }
            else
            {
                if (_procesorsCommandDict.ContainsKey(_cmd))
                    _procesorsCommandDict[_cmd]();
                else
                    Another();
            }
        }

        private void Finishing()
        {
            Console.WriteLine();
            if( _sp < MEMSIZE )
                Console.WriteLine("Код повернення " + _memory[_sp]);
        }

        private void CmAdd()
        {
            _sp++;
            _memory[_sp] += _memory[_sp - 1];
        }

        private void CmSub()
        {
            _sp++;
            _memory[_sp] -= _memory[_sp - 1];
        }

        private void CmMult()
        {
            _sp++;
            _memory[_sp] *= _memory[_sp - 1];
        }

        private void CmDiv()
        {
            _sp++;
            _memory[_sp] /= _memory[_sp - 1];
        }

        private void CmMod()
        {
            _sp++;
            _memory[_sp] %= _memory[_sp - 1];
        }

        private void CmNeg()
        {
            _memory[_sp] = -_memory[_sp];
        }

        private void CmLoad()
        {
            _memory[_sp] = _memory[_memory[_sp]];
        }

        private void CmSave()
        {
            _memory[_memory[_sp + 1]] = _memory[_sp];
            _sp += 2;
        }

        private void CmDup()
        {
            _sp--;
            _memory[_sp] = _memory[_sp + 1];
        }

        private void CmDrop()
        {
            _sp++;
        }

        private void CmSwap()
        {
            _buf = _memory[_sp];
            _memory[_sp] = _memory[_sp + 1];
            _memory[_sp + 1] = _buf;
        }

        private void CmOver()
        {
            _sp--;
            _memory[_sp] = _memory[_sp + 2];
        }

        private void CmGOTO()
        {
            _pc = _memory[_sp++];
        }

        private void CmIfEQ()
        {
            if (_memory[_sp + 2] == _memory[_sp + 1])
                _pc = _memory[_sp];
            _sp += 3;
        }

        private void CmIfNE()
        {
            if (_memory[_sp + 2] != _memory[_sp + 1])
                _pc = _memory[_sp];
            _sp += 3;
        }

        private void CmIfLE()
        {
            if (_memory[_sp + 2] <= _memory[_sp + 1])
                _pc = _memory[_sp];
            _sp += 3;
        }

        private void CmIfLT()
        {
            if (_memory[_sp + 2] < _memory[_sp + 1])
                _pc = _memory[_sp];
            _sp += 3;
        }

        private void CmIfGE()
        {
            if (_memory[_sp + 2] >= _memory[_sp + 1])
                _pc = _memory[_sp];
            _sp += 3;
        }

        private void CmIfGT()
        {
            if (_memory[_sp + 2] > _memory[_sp + 1])
                _pc = _memory[_sp];
            _sp += 3;
        }

        private void CmIn()
        {
            Console.Write('?');
            _memory[--_sp] = int.Parse(Console.ReadLine());
        }

        private void CmOut()
        {
            Console.WriteLine("Printed: " + _memory[_sp]);
            _sp += 1;
        }

        private void CmOutLn()
        {
            Console.WriteLine();
        }

        private void Another()
        {
            Console.WriteLine("Неприпустимий код операції");
            _memory[_pc] = VMCommands.cmStop;
        }
    }
}