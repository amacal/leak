using Leak.Common;

namespace Leak.Networking
{
    public class NetworkEncryptorToNothing : NetworkEncryptor
    {
        public override void Encrypt(DataBlock block)
        {
        }
    }
}