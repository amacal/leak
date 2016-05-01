namespace Leak.Core.Net
{
    public class PeerRequest : PeerMessageFactory
    {
        private readonly int piece;
        private readonly int offset;
        private readonly int length;

        public PeerRequest(int piece, int offset, int length)
        {
            this.piece = piece;
            this.offset = offset;
            this.length = length;
        }

        public int Piece
        {
            get { return piece; }
        }

        public int Offset
        {
            get { return offset; }
        }

        public override PeerMessage GetMessage()
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
            data[15] = 0x40;

            return new PeerMessage(data);
        }
    }
}