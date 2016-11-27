using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Omnibus;
using Leak.Core.Omnibus.Events;
using Leak.Core.Retriever.Components;
using Leak.Core.Retriever.Tasks;
using System.Collections.Generic;

namespace Leak.Core.Retriever.Callbacks
{
    public class RetrieverToOmnibus : OmnibusCallbackBase
    {
        private readonly RetrieverContext context;

        public RetrieverToOmnibus(RetrieverContext context)
        {
            this.context = context;
        }

        public override void OnProgressChanged(FileHash hash, BitfieldInfo bitfield)
        {
            context.Hooks.CallDataChanged(hash, bitfield.Completed);
        }

        public override void OnPieceReady(FileHash hash, PieceInfo piece)
        {
            context.Repository.Verify(piece);
        }

        public override void OnBlockReserved(FileHash hash, OmnibusReservationEvent @event)
        {
            List<Request> requests = new List<Request>(@event.Count);

            foreach (OmnibusBlock block in @event.Blocks)
            {
                requests.Add(new Request(block.Piece, block.Offset, block.Size));
                //context.Collector.Decrease(@event.Peer, 1);
            }

            //context.Collector.SendPieceRequest(@event.Peer, requests.ToArray());
        }

        public override void OnBlockExpired(FileHash hash, PeerHash peer, OmnibusBlock block)
        {
            //context.Collector.Decrease(peer, 20);
        }

        public override void OnFileCompleted(FileHash hash)
        {
            context.Repository.Flush();
            context.Hooks.CallDataCompleted(hash);
        }

        public override void OnScheduleRequested(FileHash hash, PeerHash peer)
        {
            context.Queue.Add(new ScheduleItTask(peer));
        }
    }
}