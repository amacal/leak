using System;
using System.Collections.Generic;

namespace Leak.Meta.Get
{
    public class MetamineStrategySequential : MetamineStrategy
    {
        public override IEnumerable<MetamineBlock> Next(MetamineStrategyContext context)
        {
            DateTime now = DateTime.Now;
            int left = context.Configuration.Size;

            for (int offset = 0; left > 0; offset += 16384, left -= 16384)
            {
                int size = Math.Min(left, 16384);
                MetamineBlock block = new MetamineBlock(offset / 16384, size);

                if (IsReservable(context, block, now))
                {
                    yield return block;
                    yield break;
                }
            }
        }

        private bool IsReservable(MetamineStrategyContext context, MetamineBlock block, DateTime now)
        {
            return IsCompleted(context, block) == false &&
                   IsReserved(context, block, now) == false &&
                   HasReservation(context) == false;
        }

        private bool IsCompleted(MetamineStrategyContext context, MetamineBlock block)
        {
            return context.Blocks.Contains(block);
        }

        private bool IsReserved(MetamineStrategyContext context, MetamineBlock block, DateTime now)
        {
            return context.Reservations.Contains(block, now) ||
                   context.Reservations.Contains(block, context.Peer);
        }

        private bool HasReservation(MetamineStrategyContext context)
        {
            return context.Reservations.Contains(context.Peer);
        }
    }
}