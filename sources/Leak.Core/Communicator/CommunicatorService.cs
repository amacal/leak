using Leak.Core.Common;
using Leak.Core.Loop;
using System;

namespace Leak.Core.Communicator
{
    public class CommunicatorService
    {
        private readonly CommunicatorContext context;

        public CommunicatorService(Action<CommunicatorConfiguration> configurer)
        {
            context = new CommunicatorContext(configurer);
        }

        public void Add(ConnectionLoopChannel channel)
        {
            lock (context.Synchronized)
            {
                PeerHash peer = channel.Endpoint.Session.Peer;
                CommunicatorEntry entry = context.Collection.GetOrCreate(peer);

                entry.Internal = channel;
                entry.External = new CommunicatorChannel(entry);
            }
        }

        public void Remove(PeerHash peer)
        {
            lock (context.Synchronized)
            {
                context.Collection.Remove(peer);
            }
        }

        public CommunicatorChannel Get(PeerHash peer)
        {
            lock (context.Synchronized)
            {
                return context.Collection.GetOrCreate(peer).External;
            }
        }
    }
}