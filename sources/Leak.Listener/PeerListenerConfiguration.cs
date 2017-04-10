using Leak.Common;

namespace Leak.Listener
{
    public class PeerListenerConfiguration
    {
        public PeerListenerConfiguration()
        {
            Port = new PeerListenerPortRandom();
        }

        public PeerListenerPort Port { get; set; }
    }
}