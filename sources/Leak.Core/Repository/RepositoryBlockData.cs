namespace Leak.Core.Repository
{
    public class RepositoryBlockData
    {
        private readonly int piece;
        private readonly int offset;
        private readonly byte[] bytes;

        public RepositoryBlockData(int piece, int offset, byte[] bytes)
        {
            this.piece = piece;
            this.offset = offset;
            this.bytes = bytes;
        }

        public int Piece
        {
            get { return piece; }
        }

        public int Offset
        {
            get { return offset; }
        }

        public byte[] Bytes
        {
            get { return bytes; }
        }
    }
}