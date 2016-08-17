namespace Leak.Core.Cando.Metadata
{
    public class MetadataData
    {
        private readonly int block;
        private readonly int size;
        private readonly byte[] payload;

        public MetadataData(int block, int size, byte[] payload)
        {
            this.block = block;
            this.size = size;
            this.payload = payload;
        }

        public int Block
        {
            get { return block; }
        }

        public int Size
        {
            get { return size; }
        }

        public byte[] Payload
        {
            get { return payload; }
        }
    }
}