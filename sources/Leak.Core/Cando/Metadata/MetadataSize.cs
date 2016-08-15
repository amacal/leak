namespace Leak.Core.Cando.Metadata
{
    public class MetadataSize
    {
        private readonly int bytes;

        public MetadataSize(int bytes)
        {
            this.bytes = bytes;
        }

        public int Bytes
        {
            get { return bytes; }
        }
    }
}