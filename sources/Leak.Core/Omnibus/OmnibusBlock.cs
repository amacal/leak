namespace Leak.Core.Omnibus
{
    public class OmnibusBlock
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
            return piece;
        }

        public override bool Equals(object obj)
        {
            OmnibusBlock other = obj as OmnibusBlock;

            return other.piece == piece && other.offset == offset;
        }
    }
}