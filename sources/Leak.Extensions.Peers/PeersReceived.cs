using Leak.Common;

namespace Leak.Extensions.Peers
{
    public class PeersReceived
    {
        public FileHash Hash;

        public PeerHash Peer;

        public PeerAddress[] Remotes;
    }
}
