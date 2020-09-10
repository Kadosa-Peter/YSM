using System;
using Ysm.Actions;

namespace Ysm.Assets
{
    public static class Update
    {
        public static void Run()
        {
            if (Settings.Default.Update.AddHours(23) < DateTime.Now)
            {
                IAction action = ActionRepository.Find("Update");
                action?.Execute(false);

                Settings.Default.Update = DateTime.Now;
                Settings.Default.Save();
            }
        }
    }
}
