using Leak.Core.Common;
using Leak.Core.Loop;

namespace Leak.Core.Collector
{
    public class PeerCollectorStorageEntry
    {
        public PeerCollectorStorageEntry()
        {
            LocalState = new PeerCollectorStatus();
            RemoteState = new PeerCollectorStatus();
        }

        public string Remote { get; set; }

        public FileHash Hash { get; set; }

        public PeerHash Peer { get; set; }

        public ConnectionLoopChannel Loop { get; set; }

        public bool HasExtensions { get; set; }

        public PeerCollectorStatus LocalState { get; set; }

        public PeerCollectorStatus RemoteState { get; set; }
    }
}