using Leak.Core.Common;
using Leak.Core.Messages;
using System;

namespace Leak.Core.Battlefield
{
    public class BattlefieldService
    {
        private readonly BattlefieldContext context;

        public BattlefieldService(Action<BattlefieldConfiguration> configurer)
        {
            context = new BattlefieldContext(configurer);
        }

        public void Handle(PeerHash peer, Bitfield bitfield)
        {
            lock (context.Synchronized)
            {
                context.Collection.GetOrCreate(peer).Bitfield = bitfield;
            }
        }

        public bool Contains(PeerHash peer)
        {
            lock (context.Synchronized)
            {
                return context.Collection.GetOrCreate(peer).Bitfield != null;
            }
        }

        public Bitfield Get(PeerHash peer)
        {
            lock (context.Synchronized)
            {
                return context.Collection.GetOrCreate(peer).Bitfield;
            }
        }

        public void Remove(PeerHash peer)
        {
            lock (context.Synchronized)
            {
                context.Collection.Remove(peer);
            }
        }
    }
}