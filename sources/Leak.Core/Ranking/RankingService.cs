using System;
using Leak.Common;

namespace Leak.Core.Ranking
{
    public class RankingService
    {
        private readonly RankingContext context;

        public RankingService(Action<RankingConfiguration> configurer)
        {
            context = new RankingContext(configurer);
        }

        public int Get(PeerHash peer)
        {
            lock (context.Synchronized)
            {
                return context.Collection.GetOrCreate(peer).Value;
            }
        }

        public void Increase(PeerHash peer, int step)
        {
            lock (context.Synchronized)
            {
                RankingEntry entry = context.Collection.GetOrCreate(peer);
                int value = Math.Min(entry.Value + step, context.Configuration.Maximum);

                entry.Value = value;
            }
        }

        public void Decrease(PeerHash peer, int step)
        {
            lock (context.Synchronized)
            {
                RankingEntry entry = context.Collection.GetOrCreate(peer);
                int value = Math.Max(entry.Value - step, context.Configuration.Minimum);

                entry.Value = value;
            }
        }
    }
}