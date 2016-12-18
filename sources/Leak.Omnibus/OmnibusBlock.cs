namespace Leak.Omnibus
{
    public struct OmnibusBlock
    {
        private readonly int piece;
        private readonly int offset;
        private readonly int size;

        public OmnibusBlock(int piece, int offset, int size)
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

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        public override bool Equals(object obj)
        {
            return Equals(this, (OmnibusBlock)obj);
        }

        public static int GetHashCode(OmnibusBlock block)
        {
            return block.piece + block.offset;
        }

        public static bool Equals(OmnibusBlock left, OmnibusBlock right)
        {
            return left.piece == right.piece &&
                   left.offset == right.offset;
        }
    }
}