namespace Leak.Core.Retriever
{
    public class ResourceBitfieldBlockToNothing : ResourceBitfieldBlock
    {
        private readonly int size;

        public ResourceBitfieldBlockToNothing(int size)
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

        public ResourceBitfieldBlock Complete()
        {
            return new ResourceBitfieldBlockToCompleted(size);
        }

        public ResourceBitfieldBlock Complete(int block)
        {
            return new ResourceBitfieldBlockToPartial(size, block);
        }

        public ResourceBitfieldBlock Invalidate()
        {
            return this;
        }
    }
}