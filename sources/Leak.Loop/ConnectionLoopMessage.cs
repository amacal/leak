using Leak.Common;

namespace Leak.Loop
{
    public class ConnectionLoopMessage : NetworkIncomingMessage
    {
        private readonly NetworkIncomingMessage inner;
        private readonly int size;

        public ConnectionLoopMessage(NetworkIncomingMessage inner)
        {
            this.inner = inner;
            this.size = inner.GetSize() + 4;
        }

        public int Length
        {
            get { return size; }
        }

        public byte this[int index]
        {
            get { return inner[index]; }
        }

        public byte[] ToBytes()
        {
            return inner.ToBytes(0, size);
        }

        public byte[] ToBytes(int offset)
        {
            return inner.ToBytes(offset, size - offset);
        }

        public byte[] ToBytes(int offset, int count)
        {
            return inner.ToBytes(offset, count);
        }

        public DataBlock ToBlock(DataBlockFactory factory, int offset, int count)
        {
            return inner.ToBlock(factory, offset, count);
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