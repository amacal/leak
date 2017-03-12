using Leak.Tasks;

namespace Leak.Data.Map.Tasks
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
            context.Pieces.Invalidate(piece);
        }
    }
}