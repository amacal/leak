using Leak.Common;
using Leak.Tasks;

namespace Leak.Omnibus.Tasks
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
            int blockSize = context.Metainfo.Properties.BlockSize;
            int blockIndex = block.Offset / blockSize;

            int left;
            PeerHash peer;

            left = context.Reservations.Complete(block, out peer);
            context.Pieces.Complete(block.Piece, blockIndex);

            if (peer != null && left == context.Configuration.SchedulerThreshold)
            {
                //context.Callback.OnScheduleRequested(context.Metainfo.Hash, peer);
            }
        }
    }
}