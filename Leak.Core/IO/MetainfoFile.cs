using Leak.Core.Encoding;

namespace Leak.Core.IO
{
    public class MetainfoFile
    {
        private readonly BencodedValue data;

        public MetainfoFile(byte[] data)
        {
            this.data = Bencoder.Decode(data);
        }

        public string[] Trackers
        {
            get
            {
                return new[] { data.Find("announce", node => node.ToText()) };
            }
        }

        public MetainfoFileEntryCollection Entries
        {
            get { return new MetainfoFileEntryCollection(data.Find("info", x => x)); }
        }
    }
}