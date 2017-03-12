using System.Text;

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

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (IsLocalInterestedInRemote)
                builder.Append(" local-interested");

            if (IsLocalChokingRemote)
                builder.Append(" local-choking");

            if (IsRemoteInterestedInLocal)
                builder.Append(" remote-interested");

            if (IsRemoteChokingLocal)
                builder.Append(" remote-choking");

            return builder.ToString().TrimStart();
        }
    }
}