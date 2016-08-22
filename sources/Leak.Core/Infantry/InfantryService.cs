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

        public void Enlist(PeerSession session, PeerAddress address)
        {
            lock (context.Synchronized)
            {
                InfantryEntry entry = context.Collection.GetOrCreate(session.Peer);

                entry.Session = session;
                entry.Address = address;

                context.Collection.Register(entry);
            }
        }

        public void Dismiss(PeerHash peer)
        {
            lock (context.Synchronized)
            {
                context.Collection.Remove(peer);
            }
        }

        public PeerSession Dismiss(PeerAddress address)
        {
            lock (context.Synchronized)
            {
                InfantryEntry entry = context.Collection.Get(address);

                if (entry != null)
                {
                    context.Collection.Remove(entry.Peer);
                }

                return entry?.Session;
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
                return hash.Equals(context.Collection.Get(peer)?.Session.Hash);
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