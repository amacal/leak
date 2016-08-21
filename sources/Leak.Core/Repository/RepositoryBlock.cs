namespace Leak.Core.Repository
{
    public class RepositoryBlock
    {
        private readonly int piece;
        private readonly int offset;
        private readonly int size;

        public RepositoryBlock(int piece, int offset, int size)
        {
            this.piece = piece;
            this.offset = offset;
            this.size = size;
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
            get { return size; }
        }
    }
}