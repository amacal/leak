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

        public override void Decrypt(byte[] data, int index, int count)
        {
            key.Decrypt(data, index, count);
        }
    }
}