namespace Leak.Data.Map.Components
{
    public class OmnibusPieceReady : OmnibusPiece
    {
        private readonly int blocks;

        public OmnibusPieceReady(int blocks)
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
            return false;
        }

        public bool IsComplete(int block)
        {
            return true;
        }

        public OmnibusPiece Complete()
        {
            return new OmnibusPieceCompleted(blocks);
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