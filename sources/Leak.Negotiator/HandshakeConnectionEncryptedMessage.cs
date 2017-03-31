using Leak.Common;

namespace Leak.Negotiator
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

        public DataBlock ToBytes(DataBlockFactory factory)
        {
            DataBlock block = inner.ToBytes(factory);

            key.Encrypt(block);
            return block;
        }
    }
}