namespace Ysm.Core
{
    public static partial class ExtensionMethods
    {
        public static bool IsExactly<T>(this object obj) where T : class
        {
            return obj != null && obj.GetType() == typeof(T);
        }

        public static bool IsNot<T>(this object obj) where T : class
        {
            return obj != null && obj.GetType() != typeof(T);
        }
    }
}
