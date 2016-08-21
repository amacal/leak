using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Scheduler
{
    public class SchedulerTaskDownloadTaskCallback : SchedulerTaskCallbackBase
    {
        private readonly SchedulerTaskDownloadContext context;

        public SchedulerTaskDownloadTaskCallback(SchedulerTaskDownloadContext context)
        {
            this.context = context;
        }

        public override void OnPeerBitfield(PeerHash peer, Bitfield bitfield)
        {
            context.Retriever.OnBitfield(peer, bitfield);
        }

        public override void OnPeerPiece(PeerHash peer, Piece piece)
        {
            context.Retriever.OnPiece(peer, piece);
        }
    }
}