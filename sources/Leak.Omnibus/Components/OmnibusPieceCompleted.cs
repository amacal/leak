namespace Leak.Data.Map.Components
{
    public class OmnibusPieceCompleted : OmnibusPiece
    {
        private readonly int blocks;

        public OmnibusPieceCompleted(int blocks)
        {
            this.blocks = blocks;
        }

        public int Blocks
        {
            get { return blocks; }
        }

        public bool IsReady()
        {
            return true;
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
            return new OmnibusPieceNothing(blocks);
        }
    }
}