using Leak.Common;
using Leak.Networking;

namespace Leak.Negotiator
{
    public class HandshakeConnectionToEncryptor : NetworkEncryptor
    {
        private readonly HandshakeKey key;

        public HandshakeConnectionToEncryptor(HandshakeKey key)
        {
            this.key = key;
        }

        public override void Encrypt(DataBlock block)
        {
            key.Encrypt(block);
        }
    }
}