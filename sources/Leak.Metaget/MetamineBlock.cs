namespace Leak.Meta.Get
{
    public class MetamineBlock
    {
        private readonly int index;
        private readonly int size;

        public MetamineBlock(int index, int size)
        {
            this.index = index;
            this.size = size;
        }

        public int Index
        {
            get { return index; }
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
            MetamineBlock other = obj as MetamineBlock;

            return other != null && other.index == index;
        }
    }
}