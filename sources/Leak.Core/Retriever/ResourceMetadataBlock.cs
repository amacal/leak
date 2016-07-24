namespace Leak.Core.Retriever
{
    public class ResourceMetadataBlock
    {
        private readonly int index;

        public ResourceMetadataBlock(int index)
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
            ResourceMetadataBlock other = obj as ResourceMetadataBlock;

            return other.index == index;
        }
    }
}