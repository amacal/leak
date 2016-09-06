using Leak.Core.Core;

namespace Leak.Core.Omnibus.Tasks
{
    public class InvalidatePieceTask : LeakTask<OmnibusContext>
    {
        private readonly int piece;

        public InvalidatePieceTask(int piece)
        {
            this.piece = piece;
        }

        public void Execute(OmnibusContext context)
        {
            lock (context.Synchronized)
            {
                context.Pieces.Invalidate(piece);
            }
        }
    }
}