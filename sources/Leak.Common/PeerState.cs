namespace Leak.Common
{
    public class PeerState
    {
        public PeerState()
        {
            IsLocalChokingRemote = true;
            IsRemoteChokingLocal = true;
        }

        public bool IsLocalInterestedInRemote;
        public bool IsLocalChokingRemote;

        public bool IsRemoteInterestedInLocal;
        public bool IsRemoteChokingLocal;
    }
}