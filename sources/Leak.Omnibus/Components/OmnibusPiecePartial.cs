namespace Leak.Data.Map.Components
{
    public class OmnibusPiecePartial : OmnibusPiece
    {
        private readonly bool[] blocks;
        private int completed;

        public OmnibusPiecePartial(int size, int block)
        {
            this.blocks = new bool[size];
            this.blocks[block] = true;
            this.completed = 1;
        }

        public int Blocks
        {
            get { return blocks.Length; }
        }

        public bool IsReady()
        {
            return completed == blocks.Length;
        }

        public bool IsComplete()
        {
            return false;
        }

        public bool IsComplete(int block)
        {
            return blocks[block];
        }

        public OmnibusPiece Complete()
        {
            return new OmnibusPieceCompleted(blocks.Length);
        }

        public OmnibusPiece Complete(int block)
        {
            if (blocks[block] == false)
            {
                blocks[block] = true;
                completed++;
            }

            if (completed == blocks.Length)
            {
                return new OmnibusPieceReady(blocks.Length);
            }

            return this;
        }

        public OmnibusPiece Invalidate()
        {
            return new OmnibusPieceNothing(blocks.Length);
        }
    }
}