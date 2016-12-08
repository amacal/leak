namespace Leak.Common
{
    public class Metainfo
    {
        private readonly FileHash hash;
        private readonly MetainfoEntry[] entries;
        private readonly MetainfoHash[] pieces;
        private readonly MetainfoProperties properties;

        public Metainfo(FileHash hash, MetainfoEntry[] entries, MetainfoHash[] pieces, MetainfoProperties properties)
        {
            this.hash = hash;
            this.entries = entries;
            this.pieces = pieces;
            this.properties = properties;
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
            get { return pieces; }
        }

        public MetainfoProperties Properties
        {
            get { return properties; }
        }
    }
}