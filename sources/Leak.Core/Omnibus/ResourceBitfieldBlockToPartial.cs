namespace Leak.Core.Omnibus
{
    public class OmnibusBitfieldBlockToPartial : OmnibusBitfieldBlock
    {
        private readonly bool[] blocks;
        private int completed;

        public OmnibusBitfieldBlockToPartial(int size, int block)
        {
            this.blocks = new bool[size];
            this.blocks[block] = true;
            this.completed = 1;
        }

        public int Size
        {
            get { return blocks.Length; }
        }

        public bool IsComplete()
        {
            return completed == blocks.Length;
        }

        public bool IsComplete(int block)
        {
            return blocks[block];
        }

        public OmnibusBitfieldBlock Complete()
        {
            return new OmnibusBitfieldBlockToCompleted(blocks.Length);
        }

        public OmnibusBitfieldBlock Complete(int block)
        {
            if (blocks[block] == false)
            {
                blocks[block] = true;
                completed++;
            }

            if (completed == blocks.Length)
            {
                return new OmnibusBitfieldBlockToCompleted(blocks.Length);
            }

            return this;
        }

        public OmnibusBitfieldBlock Invalidate()
        {
            return new OmnibusBitfieldBlockToNothing(blocks.Length);
        }
    }
}