using System.Collections.Generic;
using Leak.Common;

namespace Leak.Retriever
{
    public interface RetrieverOmnibus
    {
        bool IsComplete(PieceInfo piece);

        void Complete(BlockIndex block);

        void Complete(PieceInfo piece);

        void Invalidate(PieceInfo piece);

        void Schedule(string strategy, PeerHash peer, int count);

        IEnumerable<PeerHash> Find(int ranking, int count);
    }
}
