using Leak.Core.Common;

namespace Leak.Core.Bouncer
{
    public class PeerBouncerStorage
    {
        private readonly PeerBouncerConfiguration configuration;
        private readonly PeerBouncerStorageEntryCollection collection;
        private readonly object synchronized;

        public PeerBouncerStorage(PeerBouncerConfiguration configuration)
        {
            this.configuration = configuration;
            this.collection = new PeerBouncerStorageEntryCollection();
            this.synchronized = new object();
        }

        public bool AddRemote(string remote)
        {
            lock (synchronized)
            {
                PeerBouncerStorageEntry entry = collection.FindByRemote(remote);

                if (entry.Remotes.Count > 0)
                    return false;

                entry.Remotes.Add(remote);
                return true;
            }
        }

        public bool AddPeer(string remote, PeerHash peer)
        {
            lock (synchronized)
            {
                PeerBouncerStorageEntry entry = collection.FindByRemote(remote);

                if (entry.Remotes.Count != 1)
                    return false;

                if (entry.Peers.Count > 0)
                    return false;

                entry.Peers.Add(peer);
                return true;
            }
        }

        public void RemoveRemote(string remote)
        {
            lock (synchronized)
            {
                PeerBouncerStorageEntry entry = collection.FindByRemote(remote);

                if (entry.Remotes.Count == 1)
                    collection.RemoveByRemote(remote);

                foreach (PeerHash peer in entry.Peers)
                    collection.RemoveByPeer(peer);
            }
        }
    }
}