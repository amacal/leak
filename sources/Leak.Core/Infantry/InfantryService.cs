using Leak.Core.Common;
using System;
using System.Collections.Generic;

namespace Leak.Core.Infantry
{
    public class InfantryService
    {
        private readonly InfantryContext context;

        public InfantryService(Action<InfantryConfiguration> configurer)
        {
            context = new InfantryContext(configurer);
        }

        public void Enlist(PeerHash peer, FileHash hash)
        {
            lock (context.Synchronized)
            {
                InfantryEntry entry = context.Collection.GetOrCreate(peer);

                context.Collection.Register(hash, entry);
                entry.Hash = hash;
            }
        }

        public void Dismiss(PeerHash peer)
        {
            lock (context.Synchronized)
            {
                context.Collection.Remove(peer);
            }
        }

        public bool Contains(PeerHash peer)
        {
            lock (context.Synchronized)
            {
                return context.Collection.Get(peer) != null;
            }
        }

        public bool Contains(PeerHash peer, FileHash hash)
        {
            lock (context.Synchronized)
            {
                return hash.Equals(context.Collection.Get(peer)?.Hash);
            }
        }

        public PeerHash[] Find(FileHash hash)
        {
            List<PeerHash> result = new List<PeerHash>();

            lock (context.Synchronized)
            {
                foreach (InfantryEntry entry in context.Collection.Get(hash))
                {
                    result.Add(entry.Peer);
                }
            }

            return result.ToArray();
        }
    }
}