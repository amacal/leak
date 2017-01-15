namespace Leak.Common
{
    public class PeerState
    {
        public PeerState()
        {
            IsLocalChokingRemote = true;
            IsRemoteChokingLocal = true;
        }

        public PeerState(PeerState other)
        {
            IsLocalInterestedInRemote = other.IsLocalInterestedInRemote;
            IsLocalChokingRemote = other.IsLocalChokingRemote;

            IsRemoteInterestedInLocal = other.IsRemoteInterestedInLocal;
            IsRemoteChokingLocal = other.IsRemoteChokingLocal;
        }

        public bool IsLocalInterestedInRemote;
        public bool IsLocalChokingRemote;

        public bool IsRemoteInterestedInLocal;
        public bool IsRemoteChokingLocal;
    }
}