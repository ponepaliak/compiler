namespace simple_compiler.Interfaces
{
    public interface ITable
    {
        AbstractVariable SetVariable(AbstractVariable variable);

        AbstractVariable GetVariable(string name);

        void OpenScope();

        void CloseScope();

        void CheckIfVarExist(string name);

        AbstractVariable NextVar();
    }
}