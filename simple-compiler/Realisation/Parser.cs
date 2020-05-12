using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using simple_compiler.Interfaces;
using simple_compiler.Properties;

namespace simple_compiler.Classes
{
    public class Parser
    {
        private IScanner _scanner;
        private IError _error;
        private ITable _table;
        private delegate void Action();
        private Dictionary<TokenType, Action> _constructionChooser;
        private Gen _gen;

        private TokenType[] _rationsType = new[]
            {TokenType.isEquel, TokenType.less, TokenType.lessEquel, TokenType.more, TokenType.moreEquel, TokenType.notEquel};

        public Parser(IScanner scanner, IError error, ITable table, Gen gen)
        {
            _scanner = scanner;
            _error = error;
            _table = table;
            _gen = gen;
            InitConstructionChooser();
        }

        private void InitConstructionChooser()
        {
            _constructionChooser = new Dictionary<TokenType, Action>();
            _constructionChooser.Add(TokenType.ifOper, (Action)this.IfExpression);
            _constructionChooser.Add(TokenType.whileOper, (Action)this.WhileExpression);
            _constructionChooser.Add(TokenType.varName, (Action)this.Variable);
            _constructionChooser.Add(TokenType.print, (Action) this.Print);
        }

        public void Program()
        {
            while (!IsType(TokenType.eof))
            {
                Operator();
            }

            CompileFinish();
        }
        
        private void Operator(bool isBlock = false)
        {
            TokenType currentType = _scanner.GetToken().TokenType;
            if (_constructionChooser.ContainsKey(currentType)) 
                _constructionChooser[currentType]();
            else if (!isBlock)  
                _error.ErrorWithShowingCode("Unknown construction " + _scanner.GetToken().Value);
        }

        private void CompileFinish()
        {
            _gen.Cmd(0);
            _gen.Cmd(VMCommands.cmStop);
            _gen.AllocateVariables();
        }

        private void IfExpression()
        {
            int condPc, lastGoto;
            _scanner.NextToken();
            lastGoto = 0;
            ConditionExpression();
            condPc = _gen.Pc;
            BlockExpression();
            
            while (IsType(TokenType.ifelOper))
            {
                _gen.Cmd(lastGoto);
                _gen.Cmd(VMCommands.cmGOTO);
                lastGoto = _gen.Pc;
                _scanner.NextToken();
                _gen.Fixup(condPc);
                ConditionExpression();
                condPc = _gen.Pc;
                BlockExpression();
            }

            if (IsType(TokenType.elseOper))
            {
                _gen.Cmd(lastGoto);
                _gen.Cmd(VMCommands.cmGOTO);
                lastGoto = _gen.Pc;
                _scanner.NextToken();
                _gen.Fixup(condPc);
                BlockExpression(); 
            }
            else
            {
                _gen.Fixup(condPc);
            }
            
            _gen.Fixup(lastGoto);
        }

        private void ConditionExpression()
        {
            Check(TokenType.leftBracket, 85);
            BollExpression();
            Check(TokenType.rightBracket, 87);
        }

        private void BlockExpression()
        {
            Check(TokenType.openBlock, 92);
            _table.OpenScope();
            BlocksOperators();
            Check(TokenType.closeBlock, 95);
            _table.CloseScope();
        }

        private void BlocksOperators()
        {
            while (!IsType(TokenType.closeBlock))
            {
                Operator(true);
            }
        }
        private void WhileExpression()
        {
            int whilePc = _gen.Pc;
            _scanner.NextToken();
            ConditionExpression();
            int condPc = _gen.Pc;
            BlockExpression();
            _gen.Cmd(whilePc);
            _gen.Cmd(VMCommands.cmGOTO);
            _gen.Fixup(condPc);
        }
        
        private void Variable()
        {
            string varName = _scanner.GetToken().Value;
            _scanner.NextToken();
            
            if (IsType(TokenType.colon))
            {
                _table.CheckIfVarExist(varName);
                _scanner.NextToken();
                Check(TokenType.intType, 111);
                _table.SetVariable(new NumberVariable(varName));
            }

            if (IsType(TokenType.equel))
            {
                _scanner.NextToken();
                AbstractVariable variable = _table.GetVariable(varName);
                _gen.Addr(variable);
                SimpleExpression();
                _gen.Cmd(VMCommands.cmSave);
                Check(TokenType.semicolon, 121);
            }
        }

        private void Print()
        {
            _scanner.NextToken();
            if (IsType(TokenType.varName))
            {
                _gen.Addr(_table.GetVariable(_scanner.GetToken().Value));
                _gen.Cmd(VMCommands.cmLoad);
                
            } 
            else if (IsType(TokenType.number))
            {
                _gen.Const(_scanner.GetToken().Value);
            }
            else
            {
                _error.Error("After $ must be variable");
            }
            _gen.Cmd(VMCommands.cmOut);
            _scanner.NextToken();
            Check(TokenType.semicolon, 170);
        }

        private void BollExpression()
        {
            SimpleExpression();
            TokenType tokenType = Ration();
            SimpleExpression();
            _gen.Comp(tokenType);
        }

        private void SimpleExpression()
        {
            TokenType tokenType = NumberSign();
            Addition();
            if (tokenType == TokenType.minus) _gen.Cmd(VMCommands.cmNeg);
            while (IsAdditionOperation())
            {
                tokenType = _scanner.GetToken().TokenType;
                _scanner.NextToken();
                Addition();
                switch(tokenType) {
                    case TokenType.plus: _gen.Cmd(VMCommands.cmAdd); break;
                    case TokenType.minus: _gen.Cmd(VMCommands.cmSub); break;
                }
            }
        }

        private TokenType NumberSign()
        {
            TokenType tokenType = TokenType.undefined;
            if (IsType(TokenType.minus) || IsType(TokenType.plus))
            {
                tokenType = _scanner.GetToken().TokenType;
                _scanner.NextToken();
            }
            return tokenType;
        }

        private bool IsAdditionOperation()
        {
            return IsType(TokenType.plus) || IsType(TokenType.minus);
        }

        private void Addition()
        {
            Multiplier();
            while (IsMultiplication())
            {
                TokenType tokenType = _scanner.GetToken().TokenType;
                _scanner.NextToken();
                Multiplier();
                switch( tokenType ) {
                    case TokenType.multiply: _gen.Cmd(VMCommands.cmMult); break;
                    case TokenType.divide: _gen.Cmd(VMCommands.cmDiv); break;
                    case TokenType.mod: _gen.Cmd(VMCommands.cmMod); break;
                }
            }
        }
        
        private void Multiplier()
        {
            if (IsType(TokenType.varName))
            {
                AbstractVariable aVar = _table.GetVariable(_scanner.GetToken().Value);
                _gen.Addr(aVar);
                _gen.Cmd(VMCommands.cmLoad);
                _scanner.NextToken();
            }
            else if (IsType(TokenType.number))
            {
                _gen.Const(_scanner.GetToken().Value);
                Number();
            }
            else
            {
                Check(TokenType.leftBracket, 170);
                SimpleExpression();
                Check(TokenType.rightBracket, 172);
            }
        }

        private TokenType Ration()
        {
            if (!_rationsType.Contains(_scanner.GetToken().TokenType))
                _error.ErrorWithShowingCode("Ration operations is absent");
            TokenType tokenType = _scanner.GetToken().TokenType;
            _scanner.NextToken();
            return tokenType;
        }
        
        private void AdditionAction()
        {
            
        }

        private bool IsMultiplication()
        {
            return IsType(TokenType.multiply) || IsType(TokenType.divide) || IsType(TokenType.mod);
        }
        
        private void Multiplication()
        {
            
        }

        private void Number()
        {
            Check(TokenType.number);
        }

        private void Check(TokenType tokenType, int ln = 0)
        {
            if (IsType(tokenType))
            {
                _scanner.NextToken();
            }
            else
            {
                _error.ErrorWithShowingCode("Unexpected construction " + _scanner.GetToken().Value + ", method number " + ln.ToString());
            }
        }

        private bool IsType(TokenType tokenType)
        {
            return tokenType == _scanner.GetToken().TokenType;
        }
    }
}