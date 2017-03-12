using System.Collections.Generic;
using Leak.Common;
using Leak.Data.Map.Components;
using Leak.Tasks;

namespace Leak.Data.Map.Tasks
{
    public class CompletePieceTask : LeakTask<OmnibusContext>
    {
        private readonly PieceInfo piece;

        public CompletePieceTask(PieceInfo piece)
        {
            this.piece = piece;
        }

        public void Execute(OmnibusContext context)
        {
            ICollection<PeerHash> involved;

            context.Pieces.Complete(piece);
            context.Reservations.Complete(piece, out involved);

            foreach (PeerHash peer in involved)
            {
                UpdateRanking(context, peer, 32);
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