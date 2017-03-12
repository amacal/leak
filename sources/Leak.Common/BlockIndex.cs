namespace Leak.Common
{
    public struct BlockIndex
    {
        private readonly PieceInfo piece;
        private readonly int offset;
        private readonly int size;

        public BlockIndex(int piece, int offset, int size)
        {
            this.piece = new PieceInfo(piece);
            this.offset = offset;
            this.size = size;
        }

        public PieceInfo Piece
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

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        public override bool Equals(object obj)
        {
            return Equals(this, (BlockIndex)obj);
        }

        public static int GetHashCode(BlockIndex block)
        {
            return block.piece.GetHashCode() + block.offset;
        }

        public static bool Equals(BlockIndex left, BlockIndex right)
        {
            return left.piece.Index == right.piece.Index &&
                   left.offset == right.offset;
        }

        public override string ToString()
        {
            return $"{piece.Index:D5}.{offset:D8}";
        }
    }
}