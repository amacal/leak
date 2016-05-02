namespace Leak.Core.Net
{
    public class PeerPiece
    {
        private readonly int piece;
        private readonly int offset;
        private readonly byte[] data;

        public PeerPiece(PeerMessage message)
        {
            int length = message[3] + message[2] * 256 + message[1] * 256 * 256;

            this.piece = message[8] + message[7] * 256 + message[6] * 256 * 256;
            this.offset = message[12] + message[11] * 256 + message[10] * 256 * 256;
            this.data = message.ToBytes(13, length - 9);
        }

        public int Piece
        {
            get { return piece; }
        }

        public int Offset
        {
            get { return offset; }
        }

        public int Size
        {
            get { return data.Length; }
        }

        public byte[] Data
        {
            get { return data; }
        }
    }
}