using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Extensions.Peers
{
    public class PeersReceived
    {
        public FileHash Hash;

        public PeerHash Peer;

        public NetworkAddress[] Remotes;
    }
}