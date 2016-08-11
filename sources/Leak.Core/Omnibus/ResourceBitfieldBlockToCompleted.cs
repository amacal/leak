namespace Leak.Core.Omnibus
{
    public class OmnibusBitfieldBlockToCompleted : OmnibusBitfieldBlock
    {
        private readonly int size;

        public OmnibusBitfieldBlockToCompleted(int size)
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

        public OmnibusBitfieldBlock Complete()
        {
            return this;
        }

        public OmnibusBitfieldBlock Complete(int block)
        {
            return this;
        }

        public OmnibusBitfieldBlock Invalidate()
        {
            return new OmnibusBitfieldBlockToNothing(size);
        }
    }
}