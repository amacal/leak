namespace Leak.Core.Omnibus
{
    public class OmnibusPieceNothing : OmnibusPiece
    {
        private readonly int blocks;

        public OmnibusPieceNothing(int blocks)
        {
            this.blocks = blocks;
        }

        public int Blocks
        {
            get { return blocks; }
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
            return new OmnibusPieceCompleted(blocks);
        }

        public OmnibusPiece Complete(int block)
        {
            return new OmnibusPiecePartial(blocks, block);
        }

        public OmnibusPiece Invalidate()
        {
            return this;
        }
    }
}