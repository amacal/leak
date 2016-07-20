namespace Leak.Core.Retriever
{
    public class ResourcePieceRequest
    {
        private readonly int index;
        private readonly int offset;

        public ResourcePieceRequest(int index, int offset)
        {
            this.index = index;
            this.offset = offset;
        }

        public int Index
        {
            get { return index; }
        }

        public int Offset
        {
            get { return offset; }
        }

        public override int GetHashCode()
        {
            return index;
        }

        public override bool Equals(object obj)
        {
            ResourcePieceRequest other = obj as ResourcePieceRequest;

            return other.index == index && other.offset == offset;
        }
    }
}