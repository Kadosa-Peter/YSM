using System.Threading.Tasks;

namespace Ysm.Core.Helpers
{
    /// <summary>
    /// Helper class used when an async method is required,
    /// but the context is synchronous.
    /// </summary>
    public static class Empty
    {
        /// <summary>
        /// Gets the empty task.
        /// </summary>
        public static Task Task { get; } = new Task(
            () =>
            {
            });
    }
}