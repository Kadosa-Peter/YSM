using Ysm.Assets;
using Ysm.Assets.Transfer;

namespace Ysm.Actions
{
    public class ExportAction : IAction
    {
        public string Name { get; } = nameof(ExportAction);

        public void Execute(object obj)
        {
            Export export = new Export();
            export.Run();
        }
    }
}
