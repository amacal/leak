using Leak.Tasks;

namespace Leak.Datamap.Tasks
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