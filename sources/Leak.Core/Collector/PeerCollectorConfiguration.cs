using Leak.Core.Core;

namespace Leak.Core.Collector
{
    public class PeerCollectorConfiguration
    {
        public LeakBus Bus { get; set; }

        public PeerCollectorCallback Callback { get; set; }

        public PeerCollectorExtensionBuilder Extensions { get; set; }

        public string[] Countries { get; set; }
    }
}