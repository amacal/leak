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
            return index;
        }

        public override bool Equals(object obj)
        {
            PieceInfo other = obj as PieceInfo;

            return other != null && other.index == index;
        }
    }
}