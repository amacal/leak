using Leak.Common;

namespace Leak.Events
{
    public class PeerChanged
    {
        public PeerHash Peer;
        public Bitfield Bitfield;

        public bool IsLocalInterestedInRemote;
        public bool IsLocalChokingRemote;

        public bool IsRemoteInterestedInLocal;
        public bool IsRemoteChokingLocal;
    }
}