using Leak.Core.Common;
using Leak.Core.Tracker;

namespace Leak.Core.Client.Events
{
    public class FileAnnouncedEvent
    {
        private readonly TrackerAnnounce announce;

        public FileAnnouncedEvent(TrackerAnnounce announce)
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