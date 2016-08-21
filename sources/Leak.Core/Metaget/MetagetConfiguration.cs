using Leak.Core.Collector;
using Leak.Core.Common;

namespace Leak.Core.Metaget
{
    public class MetagetConfiguration
    {
        public FileHash Hash { get; set; }

        public string Destination { get; set; }

        public PeerCollectorView Collector { get; set; }

        public MetagetCallback Callback { get; set; }
    }
}