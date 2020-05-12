using System;
using System.Collections.Generic;
using simple_compiler.Interfaces;
using simple_compiler.Properties;

namespace simple_compiler.Classes
{
    public class Table : ITable
    {
        private AbstractVariable _top;
        private AbstractVariable _currentVar;
        private List<AbstractVariable> _variables = new List<AbstractVariable>();
        private int _counter = 0;

        private IError _error;

        public Table(IError error)
        {
            _error = error;
        }

        public AbstractVariable SetVariable(NumberVariable variable)
        {
            Console.WriteLine("29");
            _variables.Add(variable);
            return variable;
        }
        
        public AbstractVariable SetVariable(AbstractVariable variable)
        {
            _variables.Add(variable);
            return variable;
        }

        public AbstractVariable GetVariable(string name)
        {
            foreach (AbstractVariable variable in _variables)
            {
                if (variable.IsActive && variable.Name == name)
                    return variable;
            }

            throw new SystemException("Variable " + name + " do not exist");
        }

        public AbstractVariable FirstVar()
        {
            return _variables[_counter++];
        }

        public void OpenScope()
        {
            Scope scope = new Scope("scope");
            _variables.Add(scope);
        }

        public void CloseScope()
        {
            for (int i = _variables.Count - 1; i >= 0; i--)
            {
                if (IsScope(_variables[i]))
                {
                    _variables.RemoveAt(i);
                    break;
                }
                _variables[i].IsActive = false;
            }
        }

        public void CheckIfVarExist(string name)
        {
            if (_variables.Count == 0) return;
            foreach (AbstractVariable variable in _variables)
            {
                if (variable.IsActive && variable.Name == name)
                    _error.Error("Variable already exist - " + name);
            }
        }

        public bool IsVarExist()
        {
            return _counter < _variables.Count;
        }

        public AbstractVariable NextVar()
        {
            return _variables[_counter++];
        }

        private bool IsScope(AbstractVariable variable)
        {
            return variable.GetType() == typeof(Scope);
        }
    }
    
    public class Scope : AbstractVariable
    {
        public Scope(string name) : base(name)
        {
        }
    }
}