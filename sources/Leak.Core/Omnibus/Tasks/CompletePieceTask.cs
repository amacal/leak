using Leak.Core.Core;

namespace Leak.Core.Omnibus.Tasks
{
    public class CompletePieceTask : LeakTask<OmnibusContext>
    {
        private readonly int piece;

        public CompletePieceTask(int piece)
        {
            this.piece = piece;
        }

        public void Execute(OmnibusContext context)
        {
            lock (context.Synchronized)
            {
                context.Pieces.Complete(piece);
            }
        }
    }
}