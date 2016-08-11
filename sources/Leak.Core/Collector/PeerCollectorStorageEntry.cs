using Leak.Core.Common;

namespace Leak.Core.Collector
{
    public class PeerCollectorStorageEntry
    {
        public bool HasExtensions { get; set; }

        public PeerEndpoint Endpoint { get; set; }
    }
}