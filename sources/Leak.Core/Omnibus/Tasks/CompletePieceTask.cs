using Leak.Tasks;

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
            context.Pieces.Complete(piece);
        }
    }
}