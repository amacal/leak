using Leak.Core.Common;

namespace Leak.Core.Events
{
    public class PeerStateChanged
    {
        public PeerHash Peer;

        public bool IsLocalInterestedInRemote;
        public bool IsLocalChokingRemote;

        public bool IsRemoteInterestedInLocal;
        public bool IsRemoteChokingLocal;
    }
}