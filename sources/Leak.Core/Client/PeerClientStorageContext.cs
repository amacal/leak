using Leak.Core.Common;
using Leak.Core.Retriever;

namespace Leak.Core.Client
{
    public class PeerClientStorageContext : PeerClientExtensionContext
    {
        private readonly PeerClientStorage storage;

        public PeerClientStorageContext(PeerClientStorage storage)
        {
            this.storage = storage;
        }

        public FileHash GetHash(PeerHash peer)
        {
            return storage.GetHash(peer);
        }

        public ResourceRetriever GetRetriever(PeerHash peer)
        {
            return storage.GetRetriever(peer);
        }

        public PeerClientCallback GetCallback(PeerHash peer)
        {
            return storage.GetCallback(peer);
        }
    }
}