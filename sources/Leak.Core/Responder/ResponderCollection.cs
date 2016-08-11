using Leak.Core.Common;
using System;
using System.Collections.Generic;

namespace Leak.Core.Responder
{
    public class ResponderCollection
    {
        private readonly Dictionary<PeerHash, ResponderEntry> byPeer;

        public ResponderCollection()
        {
            byPeer = new Dictionary<PeerHash, ResponderEntry>();
        }

        public ResponderEntry[] Find(DateTime elapsed)
        {
            List<ResponderEntry> entries = new List<ResponderEntry>();

            foreach (ResponderEntry entry in byPeer.Values)
            {
                if (entry.NextKeepAlive < elapsed)
                {
                    entries.Add(entry);
                }
            }

            return entries.ToArray();
        }

        public ResponderEntry GetOrCreate(PeerHash peer)
        {
            ResponderEntry entry;

            if (byPeer.TryGetValue(peer, out entry) == false)
            {
                entry = new ResponderEntry(peer);
                byPeer.Add(peer, entry);
            }

            return entry;
        }

        public void Remove(PeerHash peer)
        {
            byPeer.Remove(peer);
        }
    }
}