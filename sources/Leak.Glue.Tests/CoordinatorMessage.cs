using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Peer.Coordinator.Tests
{
    public class CoordinatorMessage : NetworkIncomingMessage
    {
        private readonly byte[] data;

        public CoordinatorMessage(params byte[] data)
        {
            this.data = data;
        }

        public int Length
        {
            get { return data.Length; }
        }

        public byte this[int index]
        {
            get { return data[index]; }
        }

        public byte[] ToBytes()
        {
            return data;
        }

        public byte[] ToBytes(int offset)
        {
            return Bytes.Copy(data, offset);
        }

        public byte[] ToBytes(int offset, int count)
        {
            return Bytes.Copy(data, offset, count);
        }

        public DataBlock ToBlock(DataBlockFactory factory, int offset, int count)
        {
            return factory.Transcient(data, offset, count);
        }

        public void Acknowledge(int bytes)
        {
        }

        public void Continue(NetworkIncomingMessageHandler handler)
        {
        }
    }
}