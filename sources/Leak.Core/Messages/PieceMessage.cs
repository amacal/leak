namespace Leak.Core.Messages
{
    public class PieceMessage
    {
        private readonly int piece;
        private readonly int offset;
        private readonly byte[] data;

        public PieceMessage(byte[] data)
        {
            this.piece = data[3] + data[2] * 256 + data[1] * 256 * 256;
            this.offset = data[7] + data[6] * 256 + data[5] * 256 * 256;
            this.data = Bytes.Copy(data, 8);
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