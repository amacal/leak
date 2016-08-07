namespace Leak.Core.Omnibus
{
    public class OmnibusPieceCompleted : OmnibusPiece
    {
        private readonly int size;

        public OmnibusPieceCompleted(int size)
        {
            this.size = size;
        }

        public int Size
        {
            get { return size; }
        }

        public bool IsComplete()
        {
            return true;
        }

        public bool IsComplete(int block)
        {
            return true;
        }

        public OmnibusPiece Complete()
        {
            return this;
        }

        public OmnibusPiece Complete(int block)
        {
            return this;
        }

        public OmnibusPiece Invalidate()
        {
            return new OmnibusPieceNothing(size);
        }
    }
}