namespace simple_compiler.Interfaces
{
    public class AbstractVariable
    {
        public AbstractVariable PreviousVariable;

        public string Name;
        public int Value;
        public bool IsActive = true;

        public AbstractVariable(string name)
        {
            Name = name;
        }
    }
}