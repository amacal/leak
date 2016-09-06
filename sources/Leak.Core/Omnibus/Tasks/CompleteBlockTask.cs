using Leak.Core.Core;

namespace Leak.Core.Omnibus.Tasks
{
    public class CompleteBlockTask : LeakTask<OmnibusContext>
    {
        private readonly OmnibusBlock block;

        public CompleteBlockTask(OmnibusBlock block)
        {
            this.block = block;
        }

        public void Execute(OmnibusContext context)
        {
            int blockSize = context.Metainfo.Properties.BlockSize;
            int blockIndex = block.Offset / blockSize;

            lock (context.Synchronized)
            {
                context.Reservations.Complete(block);
                context.Pieces.Complete(block.Piece, blockIndex);
            }
        }
    }
}