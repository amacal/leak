using System;
using System.Collections.Generic;
using Leak.Common;
using Leak.Data.Get;
using Leak.Data.Map;

namespace Leak.Client.Adapters
{
    internal class DataGetToDataMapAdapter : DataGetToDataMap
    {
        private readonly OmnibusService service;

        public DataGetToDataMapAdapter(OmnibusService service)
        {
            this.service = service;
        }

        public bool IsComplete(PieceInfo piece)
        {
            return service.IsComplete(piece.Index);
        }

        public void Complete(BlockIndex block)
        {
            service.Complete(block);
        }

        public void Complete(PieceInfo piece)
        {
            service.Complete(piece);
        }

        public void Invalidate(PieceInfo piece)
        {
            service.Invalidate(piece.Index);
        }

        public void Schedule(string strategy, PeerHash peer, int count)
        {
            switch (strategy)
            {
                case "rarest-first":
                    service.Schedule(OmnibusStrategy.RarestFirst, peer, count);
                    return;

                case "sequential":
                    service.Schedule(OmnibusStrategy.Sequential, peer, count);
                    return;
            }
        }

        public void Query(Action<PeerHash, Bitfield, PeerState> callback)
        {
            service.Query(callback);
        }

        public IEnumerable<PeerHash> Find(int ranking, int count)
        {
            return service.Find(ranking, count);
        }
    }
}