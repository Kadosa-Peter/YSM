using System.Reflection;

namespace Ysm.Core.Helpers
{
    /// <summary>
    /// Helper class for platform and feature detection.
    /// </summary>
    internal static class FeatureDetection
    {
        private static bool? _isPrivateReflectionSupported;

        public static bool IsPrivateReflectionSupported
        {
            get
            {
                if (_isPrivateReflectionSupported == null)
                    _isPrivateReflectionSupported = ResolveIsPrivateReflectionSupported();

                return _isPrivateReflectionSupported.Value;
            }
        }

        private static bool ResolveIsPrivateReflectionSupported()
        {
            ReflectionDetectionClass inst = new ReflectionDetectionClass();

            try
            {
                MethodInfo method = typeof(ReflectionDetectionClass).GetTypeInfo().GetDeclaredMethod("Action");
                method.Invoke(inst, null);
            }
            catch // If we get here, then our platform doesn't support private reflection
            {
                return false;
            }

            return true;
        }

        private class ReflectionDetectionClass
        {
// ReSharper disable UnusedMember.Local
            private void Method()
// ReSharper restore UnusedMember.Local
            {

            }
        }
    }
}
