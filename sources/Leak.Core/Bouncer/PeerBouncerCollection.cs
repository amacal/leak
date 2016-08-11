using Leak.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Bouncer
{
    public class PeerBouncerCollection
    {
        private readonly Dictionary<long, PeerBouncerEntry> byIdentifier;
        private readonly Dictionary<string, PeerBouncerEntry> byRemote;
        private readonly Dictionary<PeerHash, PeerBouncerEntry> byPeer;

        public PeerBouncerCollection()
        {
            byIdentifier = new Dictionary<long, PeerBouncerEntry>();
            byRemote = new Dictionary<string, PeerBouncerEntry>();
            byPeer = new Dictionary<PeerHash, PeerBouncerEntry>();
        }

        public int Count(Func<PeerBouncerEntry, bool> predicate)
        {
            return byIdentifier.Values.Count(predicate);
        }

        public PeerBouncerEntry FindOrCreateByIdentifier(long identifier)
        {
            PeerBouncerEntry entry;

            if (byIdentifier.TryGetValue(identifier, out entry) == false)
            {
                entry = new PeerBouncerEntry();
                byIdentifier.Add(identifier, entry);
            }

            return entry;
        }

        public PeerBouncerEntry FindOrDefaultByRemote(string remote)
        {
            PeerBouncerEntry entry;
            byRemote.TryGetValue(remote, out entry);
            return entry;
        }

        public PeerBouncerEntry FindOrDefaultByPeer(PeerHash peer)
        {
            PeerBouncerEntry entry;
            byPeer.TryGetValue(peer, out entry);
            return entry;
        }

        public void AddByRemote(string remote, PeerBouncerEntry entry)
        {
            byRemote.Add(remote, entry);
        }

        public void AddByPeer(PeerHash peer, PeerBouncerEntry entry)
        {
            byPeer.Add(peer, entry);
        }

        public void RemoveByIdentifier(long identifier)
        {
            byIdentifier.Remove(identifier);
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