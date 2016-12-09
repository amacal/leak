using Leak.Networking;

namespace Leak.Core.Negotiator
{
    public class HandshakeConnectionToEncryptor : NetworkEncryptor
    {
        private readonly HandshakeKey key;

        public HandshakeConnectionToEncryptor(HandshakeKey key)
        {
            this.key = key;
        }

        public override byte[] Encrypt(byte[] data)
        {
            return key.Encrypt(data, 0, data.Length);
        }
    }
}