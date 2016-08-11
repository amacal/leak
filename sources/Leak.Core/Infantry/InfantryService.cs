using Leak.Core.Common;
using System;
using System.Collections.Generic;

namespace Leak.Core.Infantry
{
    public class InfantryService
    {
        private readonly object synchronized;
        private readonly InfantryCollection collection;
        private readonly InfantryConfiguration configuration;

        public InfantryService(Action<InfantryConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Callback = new InfantryCallbackNothing();
            });

            synchronized = new object();
            collection = new InfantryCollection();
        }

        public void Enlist(PeerHash peer, FileHash hash)
        {
            lock (synchronized)
            {
                collection.Register(hash, collection.GetOrCreate(peer));
            }
        }

        public void Dismiss(PeerHash peer)
        {
            lock (synchronized)
            {
                collection.Remove(peer);
            }
        }

        public bool Contains(PeerHash peer)
        {
            lock (synchronized)
            {
                return collection.Get(peer) != null;
            }
        }

        public bool Contains(PeerHash peer, FileHash hash)
        {
            lock (synchronized)
            {
                return hash.Equals(collection.Get(peer)?.Hash);
            }
        }

        public PeerHash[] Find(FileHash hash)
        {
            List<PeerHash> result = new List<PeerHash>();

            lock (synchronized)
            {
                foreach (InfantryEntry entry in collection.Get(hash))
                {
                    result.Add(entry.Peer);
                }
            }

            return result.ToArray();
        }
    }
}