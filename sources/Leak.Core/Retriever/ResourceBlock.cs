namespace Leak.Core.Retriever
{
    public class ResourceBlock
    {
        private readonly int index;
        private readonly int offset;
        private readonly int size;

        public ResourceBlock(int index, int offset, int size)
        {
            this.index = index;
            this.offset = offset;
            this.size = size;
        }

        public int Index
        {
            get { return index; }
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
            return index;
        }

        public override bool Equals(object obj)
        {
            ResourceBlock other = obj as ResourceBlock;

            return other.index == index && other.offset == offset;
        }
    }
}