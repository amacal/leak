using Leak.Networking.Core;

namespace Leak.Peer.Negotiator
{
    public class HandshakeConnectionEncryptedMessage : NetworkOutgoingMessage
    {
        private readonly NetworkOutgoingMessage inner;
        private readonly HandshakeKey key;

        public HandshakeConnectionEncryptedMessage(NetworkOutgoingMessage inner, HandshakeKey key)
        {
            this.inner = inner;
            this.key = key;
        }

        public int Length
        {
            get { return inner.Length; }
        }

        public void ToBytes(DataBlock block)
        {
            inner.ToBytes(block);
            key.Encrypt(block);
        }

        public void Release()
        {
            inner.Release();
        }
    }
}