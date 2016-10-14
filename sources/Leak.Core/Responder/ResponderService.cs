using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Loop;
using Leak.Core.Messages;
using System;

namespace Leak.Core.Responder
{
    public class ResponderService
    {
        private readonly ResponderContext context;

        public ResponderService(Action<ResponderConfiguration> configurer)
        {
            context = new ResponderContext(configurer);
        }

        public void Start(LeakPipeline pipeline)
        {
            pipeline.Register(TimeSpan.FromMinutes(1), OnTick);
        }

        public void Register(ConnectionLoopChannel channel)
        {
            lock (context.Synchronized)
            {
                PeerHash peer = channel.Endpoint.Session.Peer;
                ResponderEntry entry = context.Collection.GetOrCreate(peer);

                entry.Channel = new ResponderChannel(channel);
                entry.NextKeepAlive = DateTime.Now.AddMinutes(1);
            }
        }

        public void Handle(PeerHash peer, KeepAliveMessage message)
        {
            lock (context.Synchronized)
            {
                context.Collection.GetOrCreate(peer).LastKeepAlive = DateTime.Now;
            }
        }

        public void Remove(PeerHash peer)
        {
            lock (context.Synchronized)
            {
                context.Collection.Remove(peer);
            }
        }

        private void OnTick()
        {
            lock (context.Synchronized)
            {
                ResponderEntry[] entries;
                DateTime now = DateTime.Now;

                lock (context.Synchronized)
                {
                    entries = context.Collection.Find(now);
                }

                foreach (ResponderEntry entry in entries)
                {
                    entry.NextKeepAlive = DateTime.Now.AddMinutes(1);
                    entry.Channel.SendKeepAlive();
                }
            }
        }
    }
}