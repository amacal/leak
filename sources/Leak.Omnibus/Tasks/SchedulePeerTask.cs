using System;
using Leak.Common;
using Leak.Data.Map.Components;
using Leak.Tasks;

namespace Leak.Data.Map.Tasks
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
            if (context.Cache != null)
            {
                context.Cache.Blocks.Clear();

                if (context.Bitfields.Contains(peer))
                {
                    DateTime now = DateTime.Now;
                    FileHash hash = context.Metainfo.Hash;

                    int requested = count;
                    int taken = context.Reservations.Count(now);
                    int available = Math.Min(requested, context.Configuration.PoolSize - taken);

                    if (available > 0)
                    {
                        strategy.Next(context.Cache.Blocks, context, peer, available);

                        if (context.Cache.Blocks.Count > 0)
                        {
                            foreach (BlockIndex block in context.Cache.Blocks)
                            {
                                PeerHash previous = context.Reservations.Add(peer, block, now);

                                if (previous != null)
                                {
                                    UpdateRanking(context, previous, -4);
                                    context.Hooks.CallBlockExpired(hash, previous, block);
                                }

                                UpdateRanking(context, peer, -1);
                                context.Hooks.CallBlockReserved(hash, peer, block);
                            }
                        }
                    }
                }
            }
        }

        private void UpdateRanking(OmnibusContext context, PeerHash target, int value)
        {
            OmnibusStateEntry entry = context.States.ByPeer(target);

            if (entry != null)
            {
                entry.Ranking += value;
            }
        }
    }
}