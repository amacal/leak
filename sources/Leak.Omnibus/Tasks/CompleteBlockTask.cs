using Leak.Common;
using Leak.Data.Map.Components;
using Leak.Tasks;

namespace Leak.Data.Map.Tasks
{
    public class CompleteBlockTask : LeakTask<OmnibusContext>
    {
        private readonly BlockIndex block;

        public CompleteBlockTask(BlockIndex block)
        {
            this.block = block;
        }

        public void Execute(OmnibusContext context)
        {
            PeerHash peer;

            int blockSize = context.Metainfo.Properties.BlockSize;
            int blockIndex = block.Offset / blockSize;

            int threshold = context.Configuration.SchedulerThreshold;
            int left = context.Reservations.Complete(block, out peer);

            context.Pieces.Complete(block.Piece.Index, blockIndex);

            if (peer != null)
            {
                UpdateRanking(context, peer, 2);
            }

            if (peer != null && left == context.Configuration.SchedulerThreshold)
            {
                context.Hooks.CallThresholdReached(context.Metainfo.Hash, peer, threshold, left);
            }
        }

        private void UpdateRanking(OmnibusContext context, PeerHash peer, int count)
        {
            OmnibusStateEntry entry = context.States.ByPeer(peer);

            if (entry != null)
            {
                entry.Ranking += count;
            }
        }
    }
}