using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Ysm.Core;

namespace Ysm.Assets.Transfer
{
    public abstract class TransferBase
    {
        public event EventHandler Started;
        public event EventHandler Finished;

        // tramsfer is allowed
        protected bool Allowed = true;

        public void Run()
        {
            BeforeStart();
            
            if(Allowed == false) return;

            Started?.Invoke(this, EventArgs.Empty);

            Task task = Task.Factory.StartNew(DoWork, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            task.ContinueWith(t =>
            {
                Finished?.Invoke(this, EventArgs.Empty); AfterFinish();

            }, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void Cleanup()
        {
            try
            {
                foreach (FileInfo file in new DirectoryInfo(FileSystem.Backup).GetFiles())
                {
                    file.Delete();
                }
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = GetType().Assembly.FullName,
                    ClassName = GetType().FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace,
                };

                Logger.Log(error);

                #endregion
            }
        }

        protected virtual void BeforeStart()
        {
            
        }

        protected virtual void DoWork()
        {

        }

        protected virtual void AfterFinish()
        {
            
        }
    }
}
