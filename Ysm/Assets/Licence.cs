using Ysm.Core;

namespace Ysm.Assets
{
    public static class Licence
    {
        public static bool IsValid()
        {
            if (Settings.Default.Validated.IsNull() ||
                StringEncryption.Decrypt(Settings.Default.Validated, Kernel.Default.EncryptionKey.ConvertToString()) !=
                "Validated")
            {
                return false;
            }

            return true;
        }
    }
}
