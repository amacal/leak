using Leak.Common;

namespace Leak.Core.Metadata
{
    public class MetainfoFile
    {
        private readonly Metainfo data;
        private readonly string[] trackers;

        public MetainfoFile(Metainfo data, string[] trackers)
        {
            this.data = data;
            this.trackers = trackers;
        }

        public Metainfo Data
        {
            get { return data; }
        }

        public string[] Trackers
        {
            get { return trackers; }
        }
    }
}