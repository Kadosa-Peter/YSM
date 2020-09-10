namespace Ysm.Core
{
    public static class DebugMode
    {
        public static bool IsDebugMode { get; set; }

        static DebugMode()
        {
#if DEBUG
            IsDebugMode = true;
#endif
        }
    }
}
