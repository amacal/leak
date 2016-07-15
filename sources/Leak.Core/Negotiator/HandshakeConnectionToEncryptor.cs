using Leak.Core.Network;

namespace Leak.Core.Negotiator
{
    public class HandshakeConnectionToEncryptor : NetworkConnectionEncryptor
    {
        private readonly HandshakeKey key;

        public HandshakeConnectionToEncryptor(HandshakeKey key)
        {
            this.key = key;
        }

        public override byte[] Encrypt(byte[] data)
        {
            return key.Encrypt(data);
        }
    }
}