namespace Leak.Common
{
    public class MetainfoProperties
    {
        private readonly long totalSize;
        private readonly int pieces;
        private readonly int pieceSize;
        private readonly int blockSize;

        public MetainfoProperties(long totalSize, int pieces, int pieceSize, int blockSize)
        {
            this.totalSize = totalSize;
            this.pieces = pieces;
            this.pieceSize = pieceSize;
            this.blockSize = blockSize;
        }

        public long TotalSize
        {
            get { return totalSize; }
        }

        public int Pieces
        {
            get { return pieces; }
        }

        public int PieceSize
        {
            get { return pieceSize; }
        }

        public int BlockSize
        {
            get { return blockSize; }
        }
    }
}