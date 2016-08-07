namespace Leak.Core.Omnibus
{
    public class OmnibusPieceNothing : OmnibusPiece
    {
        private readonly int size;

        public OmnibusPieceNothing(int size)
        {
            this.size = size;
        }

        public int Size
        {
            get { return size; }
        }

        public bool IsComplete()
        {
            return false;
        }

        public bool IsComplete(int block)
        {
            return false;
        }

        public OmnibusPiece Complete()
        {
            return new OmnibusPieceCompleted(size);
        }

        public OmnibusPiece Complete(int block)
        {
            return new OmnibusPiecePartial(size, block);
        }

        public OmnibusPiece Invalidate()
        {
            return this;
        }
    }
}