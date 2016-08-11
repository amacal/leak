namespace Leak.Core.Omnibus
{
    public class OmnibusBitfieldBlockToNothing : OmnibusBitfieldBlock
    {
        private readonly int size;

        public OmnibusBitfieldBlockToNothing(int size)
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

        public OmnibusBitfieldBlock Complete()
        {
            return new OmnibusBitfieldBlockToCompleted(size);
        }

        public OmnibusBitfieldBlock Complete(int block)
        {
            return new OmnibusBitfieldBlockToPartial(size, block);
        }

        public OmnibusBitfieldBlock Invalidate()
        {
            return this;
        }
    }
}