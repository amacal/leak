using Leak.Core.Network;

namespace Leak.Core.Messages
{
    public class RequestMessage : NetworkOutgoingMessage
    {
        private readonly int piece;
        private readonly int offset;
        private readonly int length;

        public RequestMessage(int piece, int offset, int length)
        {
            this.piece = piece;
            this.offset = offset;
            this.length = length;
        }

        public int Length
        {
            get { return 17; }
        }

        public byte[] ToBytes()
        {
            byte[] data = new byte[17];

            data[3] = 13;
            data[4] = 6;
            data[5] = (byte)((piece >> 24) & 255);
            data[6] = (byte)((piece >> 16) & 255);
            data[7] = (byte)((piece >> 8) & 255);
            data[8] = (byte)piece;
            data[9] = (byte)((offset >> 24) & 255);
            data[10] = (byte)((offset >> 16) & 255);
            data[11] = (byte)((offset >> 8) & 255);
            data[12] = (byte)(offset & 255);
            data[13] = (byte)((length >> 24) & 255);
            data[14] = (byte)((length >> 16) & 255);
            data[15] = (byte)((length >> 8) & 255);
            data[16] = (byte)(length & 255);

            return data;
        }
    }
}