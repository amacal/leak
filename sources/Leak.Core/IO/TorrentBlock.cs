namespace Leak.Core.IO
{
    public class TorrentBlock
    {
        private readonly long offset;
        private readonly int size;

        public TorrentBlock(long offset, int size)
        {
            this.offset = offset;
            this.size = size;
        }

        public long Offset
        {
            get { return offset; }
        }

        public int Size
        {
            get { return size; }
        }
    }
}