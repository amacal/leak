using Leak.Networking;

namespace Leak.Peer.Negotiator
{
    public class HandshakeConnectionToDecryptor : NetworkIncomingDecryptor
    {
        private readonly HandshakeKey key;

        public HandshakeConnectionToDecryptor(HandshakeKey key)
        {
            this.key = key;
        }

        public void Decrypt(byte[] data, int offset, int count)
        {
            key.Decrypt(data, offset, count);
        }
    }
}