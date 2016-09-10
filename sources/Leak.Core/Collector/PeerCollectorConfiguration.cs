namespace Leak.Core.Collector
{
    public class PeerCollectorConfiguration
    {
        public PeerCollectorCallback Callback { get; set; }

        public PeerCollectorExtensionBuilder Extensions { get; set; }
    }
}