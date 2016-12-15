using Leak.Networking;

namespace Leak.Negotiator
{
    public class HandshakeConnectionToDecryptor : NetworkDecryptor
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