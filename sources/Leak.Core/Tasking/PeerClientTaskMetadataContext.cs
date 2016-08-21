using Leak.Core.Common;
using Leak.Core.Metaget;

namespace Leak.Core.Tasking
{
    public class PeerClientTaskMetadataContext
    {
        public FileHash Hash { get; set; }

        public string Destination { get; set; }

        public MetagetService Metaget { get; set; }

        public PeerClientTaskQueue Queue { get; set; }

        public PeerClientTask Task { get; set; }
    }
}