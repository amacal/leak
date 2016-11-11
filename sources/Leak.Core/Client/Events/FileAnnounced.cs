using Leak.Core.Common;

namespace Leak.Core.Client.Events
{
    public class FileAnnounced
    {
        public FileHash Hash { get; set; }

        public PeerHash Peer { get; set; }

        public int Count { get; set; }
    }
}