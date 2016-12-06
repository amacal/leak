using Leak.Common;

namespace Leak.Core.Listener
{
    public class PeerListenerConfiguration
    {
        public PeerListenerConfiguration()
        {
            Port = new PeerListenerPortValue(8080);
            Peer = PeerHash.Random();
            Extensions = true;
        }

        public PeerListenerPort Port { get; set; }

        public PeerHash Peer { get; set; }

        public bool Extensions { get; set; }
    }
}