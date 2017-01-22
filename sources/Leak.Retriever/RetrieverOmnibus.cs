using Leak.Common;
using System;
using System.Collections.Generic;

namespace Leak.Retriever
{
    public interface RetrieverOmnibus
    {
        bool IsComplete(PieceInfo piece);

        void Complete(BlockIndex block);

        void Complete(PieceInfo piece);

        void Invalidate(PieceInfo piece);

        void Schedule(string strategy, PeerHash peer, int count);

        void Query(Action<PeerHash, Bitfield, PeerState> callback);

        IEnumerable<PeerHash> Find(int ranking, int count);
    }
}