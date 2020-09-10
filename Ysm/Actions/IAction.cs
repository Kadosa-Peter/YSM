namespace Ysm.Assets
{
    public interface IAction
    {
        string Name { get; }

        void Execute(object obj);
    }
}
