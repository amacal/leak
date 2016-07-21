namespace Leak.Core.Retriever
{
    public class ResourceBitfieldBlockToPartial : ResourceBitfieldBlock
    {
        private readonly bool[] blocks;
        private int completed;

        public ResourceBitfieldBlockToPartial(int size, int block)
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

        public ResourceBitfieldBlock Complete()
        {
            return new ResourceBitfieldBlockToCompleted(blocks.Length);
        }

        public ResourceBitfieldBlock Complete(int block)
        {
            if (blocks[block] == false)
            {
                blocks[block] = true;
                completed++;
            }

            if (completed == blocks.Length)
            {
                return new ResourceBitfieldBlockToCompleted(blocks.Length);
            }

            return this;
        }

        public ResourceBitfieldBlock Invalidate()
        {
            return new ResourceBitfieldBlockToNothing(blocks.Length);
        }
    }
}