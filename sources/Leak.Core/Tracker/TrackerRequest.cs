using Leak.Common;

namespace Leak.Core.Tracker
{
    public class TrackerRequest
    {
        public FileHash Hash { get; set; }

        public PeerHash Peer { get; set; }

        public int Port { get; set; }
    }
}