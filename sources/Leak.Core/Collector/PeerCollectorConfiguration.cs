namespace Leak.Core.Collector
{
    public class PeerCollectorConfiguration
    {
        public PeerCollectorCallback Callback { get; set; }

        public PeerCollectorMetadata Metadata { get; set; }

        public PeerCollectorPeerExchange PeerExchange { get; set; }
    }
}