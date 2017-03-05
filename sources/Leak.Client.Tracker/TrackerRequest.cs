using Leak.Common;

namespace Leak.Client.Tracker
{
    public class TrackerRequest
    {
        public FileHash Hash { get; set; }
        public PeerHash Peer { get; set; }
        public int? Port { get; set; }
    }
}