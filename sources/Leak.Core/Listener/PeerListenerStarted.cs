using Leak.Core.Common;

namespace Leak.Core.Listener
{
    public class PeerListenerStarted
    {
        private readonly PeerHash hash;
        private readonly int port;

        public PeerListenerStarted(PeerHash hash, int port)
        {
            this.hash = hash;
            this.port = port;
        }

        public PeerHash Hash
        {
            get { return hash; }
        }

        public int Port
        {
            get { return port; }
        }
    }
}