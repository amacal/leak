using Leak.Core.Common;

namespace Leak.Core.Listener
{
    public class PeerListenerConfiguration
    {
        public PeerListenerConfiguration()
        {
            Port = 8080;
            Peer = PeerHash.Random();
            Extensions = true;
        }

        public int Port { get; set; }

        public PeerHash Peer { get; set; }

        public bool Extensions { get; set; }
    }
}