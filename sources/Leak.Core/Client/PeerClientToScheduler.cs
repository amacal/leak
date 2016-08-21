using Leak.Core.Common;
using Leak.Core.Retriever;
using Leak.Core.Scheduler;

namespace Leak.Core.Client
{
    public class PeerClientToScheduler : SchedulerCallbackBase
    {
        private readonly PeerClientContext context;

        public PeerClientToScheduler(PeerClientContext context)
        {
            this.context = context;
        }

        public override void OnCompleted(FileHash hash)
        {
            context.Callback.OnCompleted(hash);
        }

        public override void OnVerified(FileHash hash, RetrieverPiece piece)
        {
            context.Callback.OnPieceVerified(hash, new PeerClientPieceVerification(piece));
        }
    }
}