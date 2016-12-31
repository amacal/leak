using Leak.Common;

namespace Leak.Listener
{
    public class PeerListenerConfiguration
    {
        public PeerListenerConfiguration()
        {
            Port = new PeerListenerPortRandom();
            Peer = PeerHash.Random();
            Extensions = true;
        }

        public PeerListenerPort Port { get; set; }

        public PeerHash Peer { get; set; }

        public bool Extensions { get; set; }
    }
}