using Leak.Core.Common;

namespace Leak.Core.Metadata
{
    public class Metainfo
    {
        private readonly FileHash hash;
        private readonly MetainfoEntry[] entries;

        public Metainfo(FileHash hash, MetainfoEntry[] entries)
        {
            this.hash = hash;
            this.entries = entries;
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public MetainfoEntry[] Entries
        {
            get { return entries; }
        }

        public MetainfoHash[] Pieces
        {
            get { return null; }
        }
    }
}