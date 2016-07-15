using Leak.Core.Network;

namespace Leak.Core.Negotiator
{
    public class HandshakeConnectionToDecryptor : NetworkConnectionDecryptor
    {
        private readonly HandshakeKey key;

        public HandshakeConnectionToDecryptor(HandshakeKey key)
        {
            this.key = key;
        }

        public override byte[] Decrypt(byte[] data)
        {
            return key.Decrypt(data);
        }
    }
}