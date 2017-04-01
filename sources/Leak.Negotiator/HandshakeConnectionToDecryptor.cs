using Leak.Networking;

namespace Leak.Negotiator
{
    public class HandshakeConnectionToDecryptor : NetworkIncomingDecryptor
    {
        private readonly HandshakeKey key;

        public HandshakeConnectionToDecryptor(HandshakeKey key)
        {
            this.key = key;
        }

        public void Decrypt(byte[] data, int index, int count)
        {
            key.Decrypt(data, index, count);
        }
    }
}