using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Omnibus.Events;
using System;

namespace Leak.Core.Omnibus.Tasks
{
    public class SchedulePeerTask : LeakTask<OmnibusContext>
    {
        private readonly OmnibusStrategy strategy;
        private readonly PeerHash peer;
        private readonly int count;

        public SchedulePeerTask(OmnibusStrategy strategy, PeerHash peer, int count)
        {
            this.strategy = strategy;
            this.peer = peer;
            this.count = count;
        }

        public void Execute(OmnibusContext context)
        {
            DateTime now = default(DateTime);
            FileHash hash = context.Metainfo.Hash;

            context.Cache.Blocks.Clear();

            if (context.Bitfields.Contains(peer))
            {
                strategy.Next(context.Cache.Blocks, context, peer, count);

                if (context.Cache.Blocks.Count > 0)
                {
                    now = DateTime.Now;

                    foreach (OmnibusBlock block in context.Cache.Blocks)
                    {
                        PeerHash previous = context.Reservations.Add(peer, block, now);

                        if (previous != null)
                        {
                            context.Callback.OnBlockExpired(hash, previous, block);
                        }
                    }
                }
            }

            if (context.Cache.Blocks != null)
            {
                context.Callback.OnBlockReserved(hash, new OmnibusReservationEvent(peer, context.Cache.Blocks));
            }
        }
    }
}