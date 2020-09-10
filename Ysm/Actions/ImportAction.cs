using Ysm.Assets;
using Ysm.Assets.Transfer;

namespace Ysm.Actions
{
    public class ImportAction : IAction
    {
        public string Name { get; } = nameof(ImportAction);

        public void Execute(object obj)
        {
            Import import = new Import();
            import.Run();
        }
    }
}
