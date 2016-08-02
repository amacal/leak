using Leak.Core.Common;

namespace Leak.Core.Bouncer
{
    public class PeerBouncerStorage
    {
        private readonly PeerBouncerConfiguration configuration;
        private readonly PeerBouncerStorageEntryCollection collection;

        public PeerBouncerStorage(PeerBouncerConfiguration configuration)
        {
            this.configuration = configuration;
            this.collection = new PeerBouncerStorageEntryCollection();
        }

        public bool AddIdentifier(long identifier)
        {
            PeerBouncerStorageEntry entry = collection.FindOrCreateByIdentifier(identifier);

            if (entry.Released)
                return false;

            if (entry.Identifiers.Count > 0)
                return false;

            entry.Identifiers.Add(identifier);
            return true;
        }

        public bool AddRemote(long identifier, string remote)
        {
            PeerBouncerStorageEntry byIdentifier = collection.FindOrCreateByIdentifier(identifier);
            PeerBouncerStorageEntry byRemote = collection.FindOrDefaultByRemote(remote);

            if (byIdentifier.Released)
                return false;

            if (byIdentifier.Identifiers.Count == 0)
                return false;

            if (byRemote != null)
                return false;

            byIdentifier.Remotes.Add(remote);
            collection.AddByRemote(remote, byIdentifier);

            return true;
        }

        public bool AddPeer(long identifier, PeerHash peer)
        {
            PeerBouncerStorageEntry byIdentifier = collection.FindOrCreateByIdentifier(identifier);
            PeerBouncerStorageEntry byPeer = collection.FindOrDefaultByPeer(peer);

            if (byIdentifier.Released)
                return false;

            if (byIdentifier.Identifiers.Count == 0)
                return false;

            if (byIdentifier.Remotes.Count == 0)
                return false;

            if (byPeer != null)
                return false;

            byIdentifier.Peers.Add(peer);
            collection.AddByPeer(peer, byIdentifier);

            return true;
        }

        public void RemoveIdentifier(long identifier)
        {
            collection.FindOrCreateByIdentifier(identifier).Released = true;
        }
    }
}