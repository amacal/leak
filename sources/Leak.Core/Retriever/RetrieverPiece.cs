namespace Leak.Core.Retriever
{
    public class RetrieverPiece
    {
        private readonly int index;

        public RetrieverPiece(int index)
        {
            this.index = index;
        }

        public int Index
        {
            get { return index; }
        }
    }
}