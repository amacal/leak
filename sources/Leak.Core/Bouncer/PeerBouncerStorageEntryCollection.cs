using Leak.Core.Common;
using System.Collections.Generic;

namespace Leak.Core.Bouncer
{
    public class PeerBouncerStorageEntryCollection
    {
        private readonly Dictionary<string, PeerBouncerStorageEntry> byRemote;
        private readonly Dictionary<PeerHash, PeerBouncerStorageEntry> byPeer;

        public PeerBouncerStorageEntryCollection()
        {
            byRemote = new Dictionary<string, PeerBouncerStorageEntry>();
            byPeer = new Dictionary<PeerHash, PeerBouncerStorageEntry>();
        }

        public PeerBouncerStorageEntry FindByRemote(string remote)
        {
            PeerBouncerStorageEntry entry;

            if (byRemote.TryGetValue(remote, out entry) == false)
            {
                entry = new PeerBouncerStorageEntry();
                byRemote.Add(remote, entry);
            }

            return entry;
        }

        public void RemoveByRemote(string remote)
        {
            byRemote.Remove(remote);
        }

        public void RemoveByPeer(PeerHash peer)
        {
            byPeer.Remove(peer);
        }
    }
}