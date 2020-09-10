using System.Diagnostics;
using Ysm.Assets;

namespace Ysm.Actions
{
    public class OpenFeedback : IAction
    {
        public string Name { get; } = "OpenFeedback";

        public void Execute(object obj)
        {
            try
            {
                Process.Start("Ysm.Feedback.exe");
            }
            catch 
            {
                //  ignore
            }
        }
    }
}
