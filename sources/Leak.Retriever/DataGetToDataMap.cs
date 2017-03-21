using System;
using System.Collections.Generic;
using Leak.Common;

namespace Leak.Data.Get
{
    public interface DataGetToDataMap
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