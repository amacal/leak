using Leak.Core.Network;

namespace Leak.Core.Messages
{
    public class HaveOutgoingMessage : NetworkOutgoingMessage
    {
        private readonly int piece;

        public HaveOutgoingMessage(int piece)
        {
            this.piece = piece;
        }

        public int Length
        {
            get { return 9; }
        }

        public byte[] ToBytes()
        {
            byte[] data = new byte[9];

            data[4] = 0x04;

            Bytes.Write(5, data, 0);
            Bytes.Write(piece, data, 5);

            return data;
        }
    }
}