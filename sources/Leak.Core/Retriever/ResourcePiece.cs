namespace Leak.Core.Retriever
{
    public class ResourcePiece
    {
        private readonly int index;

        public ResourcePiece(int index)
        {
            this.index = index;
        }

        public int Index
        {
            get { return index; }
        }
    }
}