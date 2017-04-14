using Leak.Common;

namespace Leak.Peer.Coordinator.Messages
{
    public class HaveIncomingMessage
    {
        private readonly byte[] data;

        public HaveIncomingMessage(byte[] data)
        {
            this.data = data;
        }

        public int Piece
        {
            get { return Bytes.ReadInt32(data, 0); }
        }
    }
}