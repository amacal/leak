namespace Leak.Common
{
    public class PieceInfo
    {
        private readonly int index;

        public PieceInfo(int index)
        {
            this.index = index;
        }

        public int Index
        {
            get { return index; }
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        public override bool Equals(object obj)
        {
            return Equals(this, (PieceInfo)obj);
        }

        public static int GetHashCode(PieceInfo piece)
        {
            return piece.index;
        }

        public static bool Equals(PieceInfo left, PieceInfo right)
        {
            return left.index == right.index;
        }

        public override string ToString()
        {
            return index.ToString();
        }
    }
}