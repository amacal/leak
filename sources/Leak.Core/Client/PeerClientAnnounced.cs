using Leak.Core.Common;
using Leak.Core.Tracker;

namespace Leak.Core.Client
{
    public class PeerClientAnnounced
    {
        private readonly TrackerAnnounce announce;

        public PeerClientAnnounced(TrackerAnnounce announce)
        {
            this.announce = announce;
        }

        public PeerHash Peer
        {
            get { return announce.Peer; }
        }

        public int Peers
        {
            get { return announce.Peers.Length; }
        }
    }
}