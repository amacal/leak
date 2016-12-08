using Leak.Common;
using Leak.Core.Messages;

namespace Leak.Core.Negotiator
{
    public class HandshakeKeyMessage : NetworkIncomingMessage
    {
        private readonly NetworkIncomingMessage inner;
        private readonly byte[] decrypted;

        public HandshakeKeyMessage(NetworkIncomingMessage inner, byte[] decrypted)
        {
            this.inner = inner;
            this.decrypted = decrypted;
        }

        public int Length
        {
            get { return decrypted.Length; }
        }

        public byte this[int index]
        {
            get { return decrypted[index]; }
        }

        public byte[] ToBytes()
        {
            return decrypted;
        }

        public byte[] ToBytes(int offset)
        {
            return Bytes.Copy(decrypted, offset);
        }

        public byte[] ToBytes(int offset, int count)
        {
            return Bytes.Copy(decrypted, offset, count);
        }

        public DataBlock ToBlock(DataBlockFactory factory, int offset, int count)
        {
            return factory.Create(decrypted, offset, count);
        }

        public void Acknowledge(int bytes)
        {
            inner.Acknowledge(bytes);
        }

        public void Continue(NetworkIncomingMessageHandler handler)
        {
            inner.Continue(handler);
        }
    }
}