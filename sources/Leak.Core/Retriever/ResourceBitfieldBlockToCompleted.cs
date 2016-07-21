namespace Leak.Core.Retriever
{
    public class ResourceBitfieldBlockToCompleted : ResourceBitfieldBlock
    {
        private readonly int size;

        public ResourceBitfieldBlockToCompleted(int size)
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

        public ResourceBitfieldBlock Complete()
        {
            return this;
        }

        public ResourceBitfieldBlock Complete(int block)
        {
            return this;
        }

        public ResourceBitfieldBlock Invalidate()
        {
            return new ResourceBitfieldBlockToNothing(size);
        }
    }
}