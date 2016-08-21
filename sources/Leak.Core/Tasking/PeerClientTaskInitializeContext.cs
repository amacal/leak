using Leak.Core.Metadata;
using Leak.Core.Repository;

namespace Leak.Core.Tasking
{
    public class PeerClientTaskInitializeContext
    {
        public Metainfo Metainfo { get; set; }

        public string Destination { get; set; }

        public RepositoryService Repository { get; set; }

        public PeerClientTaskQueue Queue { get; set; }

        public PeerClientTask Task { get; set; }
    }
}